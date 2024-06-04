using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Scraper.UseCases.ExtractProductFromHtml;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors;
using Scraper.UseCases.ExtractProductFromHtml.PropertyExtractors.Ah;

namespace Scraper.UseCases;

public class ExtractProductFromHtmlUseCase(
    FoodContext db,
    ProductExtractor extractor,
    IEnumerable<IProductPropertyExtractor> propertyExtractors,
    ILogger<ExtractProductFromHtmlUseCase> logger) : IInvocable, ICancellableTask
{
    public CancellationToken Token { get; set; }

    public async Task Invoke()
    {
        try
        {
            logger.LogInformation("Selecting html content that needs extracting");

            var jobIds = await db.ScrapeJobs
                .Where(j => j.ErrorMessage == null && j.Content != null && j.Product == null)
                .Select(j => j.Id)
                .ToListAsync(Token);

            logger.LogInformation("{Count} need extracting", jobIds.Count);

            foreach (var jobId in jobIds)
            {
                // trade-off here: to prevent fetching ALL html contents (big memory) at once, only fetch IDs first and query in a loop
                var job = await db.ScrapeJobs.FindAsync(jobId);
                ArgumentNullException.ThrowIfNull(job);

                if (Token.IsCancellationRequested)
                {
                    logger.LogInformation("Job {Id}: aborted due to cancellation", job.Id);
                    return;
                }

                List<IProductPropertyExtractor> propertyExtractorsToUse = [];

                if (job.Url.Contains("ah.nl", StringComparison.InvariantCultureIgnoreCase))
                {
                    propertyExtractorsToUse = propertyExtractors
                        .Where(p => p is IAhPropertyExtractor)
                        .ToList();
                }


                if (propertyExtractorsToUse.Count == 0)
                {
                    logger.LogError("No extractors available for Job {JobId}", job.Id);
                    job.ErrorMessage = "Unable to determine property extractors for this job";
                }
                else
                {
                    logger.LogInformation("Job {Id}: Extracting product..", job.Id);
                    var result = await extractor.ExtractAsync(job.Content!, propertyExtractorsToUse);

                    switch (result)
                    {
                        case ProductSuccess productResult:
                            var productNew = productResult.Result;
                            var productDb = db.Products
                                .Include(p => p.NutritionInfo)
                                .Include(p => p.Ingredients)
                                .FirstOrDefault(p => p.Title == productNew.Title);

                            if (productDb is null)
                            {
                                productDb = productNew;
                            }
                            else
                            {
                                productDb.Nutriscore = productNew.Nutriscore;
                                productDb.Price = productNew.Price;
                                productDb.Summary = productNew.Summary;
                                productDb.UnitSize = productNew.UnitSize;
                                if (productDb.NutritionInfo is null || productNew.NutritionInfo is null)
                                {
                                    productDb.NutritionInfo = productNew.NutritionInfo;
                                }
                                else
                                {
                                    productDb.NutritionInfo.Sugars = productNew.NutritionInfo.Sugars;
                                    productDb.NutritionInfo.Salts = productNew.NutritionInfo.Salts;
                                    productDb.NutritionInfo.Fats = productNew.NutritionInfo.Fats;
                                    productDb.NutritionInfo.FatsSaturated = productNew.NutritionInfo.FatsSaturated;
                                    productDb.NutritionInfo.Calories = productNew.NutritionInfo.Calories;
                                    productDb.NutritionInfo.PortionRecommended = productNew.NutritionInfo.PortionRecommended;
                                    productDb.NutritionInfo.Carbs = productNew.NutritionInfo.Carbs;
                                    productDb.NutritionInfo.FatsUnsaturated = productNew.NutritionInfo.FatsUnsaturated;
                                    productDb.NutritionInfo.Fibres = productNew.NutritionInfo.Fibres;
                                    productDb.NutritionInfo.Per = productNew.NutritionInfo.Per;
                                    productDb.NutritionInfo.PerUnit = productNew.NutritionInfo.PerUnit;
                                    productDb.NutritionInfo.Proteines = productNew.NutritionInfo.Proteines;
                                    productDb.NutritionInfo.PreparationState = productNew.NutritionInfo.PreparationState;
                                }
                            }

                            if (productDb.Ingredients?.Count > 0 && productNew.Ingredients?.Count > 0)
                            {
                                var ingredientsRequired = productNew.Ingredients.Select(i => i.Name).Distinct().ToList();

                                var ingredientsInDb = await db.Ingredients.Where(i => ingredientsRequired.Contains(i.Name)).ToListAsync();
                                var ingredientsUnknown = productDb.Ingredients.ExceptBy(ingredientsInDb.Select(i => i.Name), i => i.Name).ToList();
                                // var completelyNew = productNew.Ingredients
                                //         .ExceptBy(ingredientsDb.Select(i => i.Name), i => i.Name)
                                //         .ExceptBy(ingredientsDbNew.Select(i => i.Name), i => i.Name)
                                //         .ToList();
                                var ingredientsToRemove = productDb.Ingredients.ExceptBy(ingredientsRequired, i => i.Name)
                                    .ToList();


                                // TODO: Hier gaat nog iets mis... Maar ik weet nog niet wat
                                // Aardappelbollen is aan zet: er zitten ingredienten in DB die ik kan gebruiken
                                // maar dat lijkt nog niet helemaal goed te gaan
                                // het gaat dus mis zodra het product helemaal nieuw is... ... Alles in productdb.Ingredients is nog niet tracked

                                if (productDb.Id == default)
                                {
                                    productDb.Ingredients.Clear();
                                    ingredientsInDb.ForEach(productDb.Ingredients.Add);
                                    ingredientsUnknown.ForEach(productDb.Ingredients.Add);
                                }

                                // ingredientsDbNew.ForEach(productDb.Ingredients.Add);
                                // completelyNew.ForEach(productDb.Ingredients.Add);
                                ingredientsToRemove.ForEach(i => productDb.Ingredients.Remove(i));
                            }

                            if (productDb.Id == default)
                            {
                                await db.Products.AddAsync(productDb, Token);
                            }

                            job.Product = productDb;
                            break;

                        case ProductFailResult failResult:
                            job.ErrorMessage = failResult.ErrorMessage;
                            logger.LogWarning("Job {Id}: Job failed with extractor error", job.Id);
                            break;

                        default:
                            throw new NotImplementedException("unknown result: " + result.GetType().Name);
                    }
                }

                await db.SaveChangesAsync(Token);
                logger.LogInformation("Job {Id}: Saved product successfully", job.Id);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong while extracting product from html");
            throw;
        }
    }
}