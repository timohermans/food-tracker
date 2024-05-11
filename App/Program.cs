using App.Workers;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

builder.Services.AddSerilog();
builder.Services.AddDbContext<FoodContext>(
    opt => opt.UseSqlServer(config.GetConnectionString("Default")));

builder.Services.AddQuartz(q =>
{
    q.AddJob<ProductScrapeJobCreator>(opts => opts.WithIdentity(ProductScrapeJobCreator.JobKey));
    q.AddTrigger(opts => opts
                .ForJob(ProductScrapeJobCreator.JobKey)
                .StartNow()
                .WithIdentity("AH_get_product_to_scrape__trigger")
                .WithCronSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0)));

    if (true || env.IsDevelopment())
    {
        q.AddTrigger(opts => opts
                    .ForJob(ProductScrapeJobCreator.JobKey)
                    .StartNow()
                    .WithIdentity("AH_get_product_to_scrape__trigger_dev"));
    }


    q.AddJob<ProductScraper>(opts => opts
        .WithIdentity(ProductScraper.JobKey)
        .StoreDurably(true)
        .RequestRecovery(true));
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.Run();