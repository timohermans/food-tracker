using Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Xml;
using System.Xml.Linq;

namespace App.Workers;

internal class ProductScrapeJobCreator(FoodContext db, ISchedulerFactory schedulerFactory, ILogger<ProductScrapeJobCreator> logger) : IJob
{
    public static readonly JobKey JobKey = new("AH_get_products_to_scrape");

    public async Task Execute(IJobExecutionContext context)
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

        logger.LogInformation("Found {ProductNewCount} new products to scrape", urlsNew.Count);

        var scheduler = await schedulerFactory.GetScheduler();
        foreach (var url in urlsNew)
        {
            await scheduler.TriggerJob(ProductScraper.JobKey, new JobDataMap(new Dictionary<string, string> { { nameof(ProductScraper.Url), url } }));
            logger.LogInformation("Schedules job for {Url}", url);
        }

        logger.LogInformation("Done scheduling product scrape jobs");
    }
}
