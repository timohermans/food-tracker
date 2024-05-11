using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Xml.Linq;
using Web.Data;

namespace Web.UseCases;

public class ProductsFindToScrapeUseCase(FoodContext db, ILogger<ProductsFindToScrapeUseCase> logger) : IInvocable, ICancellableTask
{
    public CancellationToken Token { get; set; }

    public async Task Invoke()
    {
        logger.LogInformation("Reading XML 📄...");

        var productUrls = new List<string>();

        using var xmlStream = File.OpenRead("Data/ah product links.xml");
        using var xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
        {
            Async = true
        });

        await xmlReader.MoveToContentAsync();
        while (await xmlReader.ReadAsync())
        {
            var element = xmlReader.Name;

            if (element == "loc")
            {
                XElement? elementValue = await XNode.ReadFromAsync(xmlReader, CancellationToken.None) as XElement;
                if (elementValue is null) continue;
                productUrls.Add(elementValue.Value);
            }
        }

        logger.LogInformation("Found {ProductCount} products 🛒...", productUrls.Count);

        logger.LogInformation("Fetching existing product scrape jobs");

        var urlsInDb = await db.ScrapeJobs.Select(j => j.Url).ToListAsync();

        var urlsNew = productUrls.Where(u => !urlsInDb.Contains(u)).ToList();

        await db.ScrapeJobs.AddRangeAsync(urlsNew.Select(u => new Data.Types.ProductScrapeJob { Url = u }), Token);
        await db.SaveChangesAsync(Token);

        logger.LogInformation("Found {ProductNewCount} new products to scrape", urlsNew.Count);
    }
}
