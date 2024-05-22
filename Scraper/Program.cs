using Coravel;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Scraper;
using Scraper.UseCases;
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

builder.Services.AddPropertyExtractors();
builder.Services.AddUseCases();

builder.Services.AddDbContext<FoodContext>(
    opt => opt.UseSqlServer(config.GetConnectionString("Default")));

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "App: FoodTracker";
});
builder.Services.AddScheduler();
builder.Services.AddQueue();

var host = builder.Build();

var scheduler = host.Services.GetRequiredService<IScheduler>();
// scheduler.Schedule<ProductsFindToScrapeUseCase>()
//     .DailyAt(0, 0)
//     .PreventOverlhosting(nameof(ProductsFindToScrapeUseCase));

scheduler.Schedule<ProductScrapeUseCase>()
    .EveryMinute()
    .PreventOverlapping(nameof(ProductScrapeUseCase));

// if (env.IsDevelopment())
// {
var queue = host.Services.GetRequiredService<IQueue>();
queue.QueueCancellableInvocable<ProductsFindToScrapeUseCase>();

// queue.QueueCancellableInvocable<ExtractProductFromHtmlUseCase>();
// }

host.Run();
