using Core.Data;
using Core.Data.Types;
using Microsoft.EntityFrameworkCore;

namespace Scraper.UseCases.ExtractProductFromHtml;

public class ProductPersister(FoodContext db)
{
    private readonly FoodContext _db = db;

    public async Task<Product> Persist(Product productNew)
    {
        var productDb = _db.Products
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

            var ingredientsInDb = await _db.Ingredients.Where(i => ingredientsRequired.Contains(i.Name)).ToListAsync();
            var ingredientsUnknown = productNew.Ingredients.ExceptBy(ingredientsInDb.Select(i => i.Name), i => i.Name).ToList();

            productDb.Ingredients.Clear();
            ingredientsInDb.ForEach(productDb.Ingredients.Add);
            ingredientsUnknown.ForEach(productDb.Ingredients.Add);
        }

        if (productDb.Id == default)
        {
            await _db.Products.AddAsync(productDb);
        }

        return productDb;
    }
}
