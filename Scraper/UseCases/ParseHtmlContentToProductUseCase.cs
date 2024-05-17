using AngleSharp;
using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Scraper.UseCases;

public class ParseHtmlContentToProductUseCase(FoodContext db, ILogger<ParseHtmlContentToProductUseCase> logger) : IInvocable, ICancellableTask
{
    public CancellationToken Token { get; set; }

    public async Task Invoke()
    {
        logger.LogInformation("Selecting html content that needs extracting");

        var jobs = await db.ScrapeJobs
            .Where(j => j.HasNutritionInfo == null && j.Content != null)
            .ToListAsync();

        logger.LogInformation("{Count} need extracting", jobs.Count);

        foreach (var job in jobs)
        {
            if (Token.IsCancellationRequested)
            {
                logger.LogInformation("Job aborted");
            }








            logger.LogInformation("Going to extract {Job}", job);
        }
    }
}
