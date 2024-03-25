using MrJb.OpenTelemetry.Api.Customers;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .CreateLogger();

// starting identity server
Log.Information("Starting API Customers...");

builder.AddServiceDefaults();

// configuration
builder.Configuration
    .AddJsonFile("appsettings.mrjb.json", optional: true, reloadOnChange: true);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.BootstrapApplication(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/customers", () =>
{
    using var activity = OTel.ActivitySource.StartActivity("Customers.GetCustomers");

    try
    {
        // metric
        var tagList = new TagList();
        tagList.Add("customer.id", "");
        tagList.Add("customer.email", "");

        //OTel.Meters.AddGetOrder(1, TagList);

        // set tags
        activity?.SetTag("customer.id", "");
        activity?.SetTag("customer.email", "");

        // event
        var tags = new ActivityTagsCollection();
        tags["customer.id"] = "1234";

        var e = new ActivityEvent("MrJB.OTel.Customers.API.Get", DateTimeOffset.Now, tags);
        activity?.AddEvent(e);

        //var data = GetOrders();

        activity?.SetStatus(ActivityStatusCode.Ok);

        return "";

    }
    catch (Exception ex)
    {
        //_logger.LogError(ex.Message);
        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
        activity?.RecordException(ex);
        throw new Exception("Unable to load customers.");
    }
}).WithName("GetCustomers")
.WithOpenApi();

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
