using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Scraper;
using Scraper.UseCases;
using Scraper.UseCases.ExtractProductFromHtml;
using Scraper.UseCases.ProductScrape;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

builder.Services.AddSingleton(_ => TimeProvider.System);
builder.Services.AddSerilog((services, lc) =>
{
    lc.ReadFrom.Configuration(config)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddTransient<ProductExtractor>();
builder.Services.AddPropertyExtractors();
builder.Services.AddUseCases();

builder.Services.AddDbContext<FoodContext>(
    opt => opt.UseNpgsql(config.GetConnectionString("Default")));

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "App: FoodTracker";
});
builder.Services.AddScheduler();
builder.Services.AddQueue();

var host = builder.Build();

var scheduler = host.Services.GetRequiredService<IScheduler>();

var findJob = scheduler.Schedule<ProductsFindToScrapeUseCase>()
     .DailyAt(0, 0)
     .PreventOverlapping(nameof(ProductsFindToScrapeUseCase));

var websiteScrapeJob = scheduler.Schedule<WebsiteScrapeUseCase>()
     .EveryMinute()
     .PreventOverlapping(nameof(WebsiteScrapeUseCase));

var extractJob = scheduler.Schedule<ExtractProductFromHtmlUseCase>()
    .HourlyAt(0)
    .PreventOverlapping(nameof(ExtractProductFromHtmlUseCase));

if (env.IsDevelopment())
{
    // findJob.Once().RunOnceAtStart();
    // websiteScrapeJob.Once().RunOnceAtStart();
    extractJob.Once().RunOnceAtStart();
}

host.Run();
