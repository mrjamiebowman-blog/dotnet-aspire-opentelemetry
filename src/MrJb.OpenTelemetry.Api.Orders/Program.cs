using MrJb.NetAspire.OpenTelemetry;
using MrJB.OpenTelemetry.Domain.Models;
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
Log.Information("Starting API Orders...");

// otel service defaults
builder.AddServiceDefaults();

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

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

builder.Services.BootstrapApplication(builder.Configuration, builder.Environment);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// serilog
app.UseSerilogRequestLogging();

app.MapPost("/orders", async (ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    using var activity = OTel.ActivitySource.StartActivity("Orders.GetOrders");

    try
    {
        // do work...
        Random rnd = new Random();
        var delay = rnd.Next(100, 800);
        await Task.Delay(delay);

        var order = new Order();
        order.CustomerId = Faker.RandomNumber.Next();
        order.OrderId = Faker.RandomNumber.Next();
        order.Total = Faker.RandomNumber.Next(100, 500);
        order.SubTotal = (order.Total.Value * 0.8M);
        order.Taxes = (order.Total - order.SubTotal);

        // logger
        logger.LogInformation("Retrieved Order: (#{OrderId})", order.OrderId);

        // metric
        var tagList = new TagList();
        tagList.Add("order.id", order.OrderId);
        tagList.Add("customer.id", order.CustomerId);

        OTel.Meters.AddGetOrders(1, tagList);

        // set tags
        activity?.SetTag("order.id", order.OrderId);
        activity?.SetTag("customer.id", order.CustomerId);

        // event
        var tags = new ActivityTagsCollection();
        tags["order.id"] = order.OrderId;
        tags["customer.id"] = order.CustomerId;

        var e = new ActivityEvent("MrJB.OTel.Orders.API.Get", DateTimeOffset.Now, tags);
        activity?.AddEvent(e);

        activity?.SetStatus(ActivityStatusCode.Ok);

        return order;

    } catch (Exception ex)
    {
        //_logger.LogError(ex.Message);
        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
        activity?.RecordException(ex);
        throw new Exception("Unable to load orders.");
    }
}).WithName("GetOrders")
.WithOpenApi();

app.Run();
