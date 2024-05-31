using Core.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Web;

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseStaticFiles();

app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

app.UseAntiforgery();

//app.UseMiddleware<LogUsernameMiddleware>();

app.MapEndpoints();

app.Run();
