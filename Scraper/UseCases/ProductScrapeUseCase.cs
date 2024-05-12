using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Core.Data;
using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;

namespace Scraper.UseCases;

public class ProductScrapeUseCase(FoodContext db, ILogger<ProductScrapeUseCase> logger) : IInvocable, ICancellableTask
{
    public CancellationToken Token { get; set; }

    public async Task Invoke()
    {
        logger.LogInformation("Looking for scrape jobs that have not been scraped yet.");

        var jobs = await db.ScrapeJobs.Where(s => s.Content == null).ToListAsync(Token);
        if (jobs.Count == 0)
        {
            logger.LogInformation("No products to scrape. Skipping..");
            return;
        }

        foreach (var job in jobs)
        {
            if (Token.IsCancellationRequested)
            {
                logger.LogInformation("Cancelling job scraping");
                return;
            }

            logger.LogInformation("Job {Url}: Start scraping", job.Url);
            job.Content = await TryScrapeAsync(job.Url);
            await db.SaveChangesAsync(Token);
            logger.LogInformation("Job {Url}: Content successfully saved", job.Url);

            await Throttle(job);
        }

        logger.LogInformation("Done scraping all jobs");
    }

    private async Task<string?> TryScrapeAsync(string url)
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            IPage page = await CreatePage(playwright);

            logger.LogInformation("Job {Url}: Navigating to url", url);

            await page.GotoAsync(url);

            var body = await page.QuerySelectorAsync("body");

            if (body is null)
            {
                logger.LogInformation("Job {Url}: Unable to locate body element", url);
                return null;
            }

            var content = await body.InnerHTMLAsync();

            logger.LogInformation("Job {Url}: Content successfully scraped", url);

            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job {Url}: Something went wrong scraping", url);
            throw ex;
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

    private async Task Throttle(ProductScrapeJob job)
    {
        var random = new Random();
        var delay = random.Next(5000, 10000);
        logger.LogInformation("Job {Url}: Throttling for {Delay} milliseconds", job.Url, delay);
        await Task.Delay(delay);
    }
}
