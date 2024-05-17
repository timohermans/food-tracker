using Core.Data;
using Core.Data.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Quartz;

namespace App.Workers;

[DisallowConcurrentExecution]
internal class ProductScraper(FoodContext db, ILogger<ProductScraper> logger) : IJob
{
    public static readonly JobKey JobKey = new("AH_product_scrape");

    public required string Url { private get; set; }

    public async Task Execute(IJobExecutionContext context)
    {
        if (db.ScrapeJobs.Any(s => s.Url == Url))
        {
            logger.LogInformation("Job {Url}: Already exists. Skipping...", Url);
            return;
        }

        var scrapeJob = new ProductScrapeJob { Url = Url };

        scrapeJob.Content = await TryScrapeAsync();

        await db.ScrapeJobs.AddAsync(scrapeJob);
        await db.SaveChangesAsync();

        logger.LogInformation("Job {Url}: Content successfully saved", Url);

        var random = new Random();
        var delay = random.Next(5000, 10000);
        logger.LogInformation("Job {Url}: Throttling for {Delay} milliseconds", Url, delay);

        await Task.Delay(delay);
    }


    private async Task<string?> TryScrapeAsync()
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            IPage page = await CreatePage(playwright);

            logger.LogInformation("Job {Url}: Navigating to url", Url);

            await page.GotoAsync(Url);

            var body = await page.QuerySelectorAsync("body");

            if (body is null)
            {
                logger.LogInformation("Job {Url}: Unable to locate body element", Url);
                return null;
            }

            var content = await body.InnerHTMLAsync();

            logger.LogInformation("Job {Url}: Content successfully scraped", Url);

            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong with scraping AH");
            throw;
        }
    }

    private static async Task<IPage> CreatePage(IPlaywright playwright)
    {
        var random = new Random();
        List<string> userAgentStrings = [
  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.2227.0 Safari/537.36",
  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36",
  "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.3497.92 Safari/537.36",
  "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36",
];
        var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = userAgentStrings[random.Next(0, userAgentStrings.Count)],
        });
        await context.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})"); // disable automation indicator

        var page = await context.NewPageAsync();
        return page;
    }
}
