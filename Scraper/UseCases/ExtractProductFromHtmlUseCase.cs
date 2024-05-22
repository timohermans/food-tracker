using AngleSharp;
using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Core.Data;
using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Scraper.UseCases.ProductScrape.ProductExtraction;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors;
using Scraper.UseCases.ProductScrape.ProductExtraction.PropertyExtractors.Ah;

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
        logger.LogInformation("Selecting html content that needs extracting");

        var jobIds = await db.ScrapeJobs
            .Where(j => j.ErrorMessage == null && j.Content != null)
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
                        await db.Products.AddAsync(productResult.Result, Token);
                        job.Product = productResult.Result;
                        logger.LogInformation("Job {Id}: Saved product successfully", job.Id);
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
        }
    }
}