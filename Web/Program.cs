using Coravel;
using Coravel.Cache.SQLServer;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Web;
using Web.Data;
using Web.UseCases;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddRazorComponents();
builder.Services.AddEndpointsFrom(typeof(Program).Assembly);
builder.Services.AddUseCases();

builder.Services.AddDbContext<FoodContext>(
    opt => opt.UseSqlServer(config.GetConnectionString("Default")));
builder.Services.AddScheduler();
builder.Services.AddQueue();
builder.Services.AddSQLServerCache(config.GetConnectionString("Default"));

var app = builder.Build();

var scheduler = app.Services.GetRequiredService<IScheduler>();
// scheduler.Schedule<ProductsFindToScrapeUseCase>()
//     .DailyAt(0, 0)
//     .PreventOverlapping(nameof(ProductsFindToScrapeUseCase));

scheduler.Schedule<ProductScrapeUseCase>()
    .EveryMinute()
    .PreventOverlapping(nameof(ProductScrapeUseCase));

// if (env.IsDevelopment())
// {
    var queue = app.Services.GetRequiredService<IQueue>();
    queue.QueueCancellableInvocable<ProductsFindToScrapeUseCase>();
// }

// Configure the HTTP request pipeline.

app.UseStaticFiles();

app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

app.UseAntiforgery();

//app.UseMiddleware<LogUsernameMiddleware>();

app.MapEndpoints();

app.Run();
