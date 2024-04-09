using MrJb.OpenTelemetry.Api.Customers;
using MrJB.OpenTelemetry.Domain.Models;
using OpenTelemetry.Trace;
using RestSharp;
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

app.MapPost("/customers", async (ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    using var activity = OTel.ActivitySource.StartActivity("Customers.GetCustomers");

    try
    {
        // do work...
        Random rnd = new Random();
        var delay = rnd.Next(100, 800);
        await Task.Delay(delay);

        var customer = new Customer();
        customer.CustomerId = Faker.RandomNumber.Next();
        customer.FirstName = Faker.Name.First();
        customer.LastName = Faker.Name.Last();
        customer.Email = Faker.Internet.Email();

        // logger
        logger.LogInformation("Retrieved Customer: ({FirstName} {LastName})", customer.FirstName, customer.LastName);

        try
        {
            // get orders (http://localhost:5179/swagger)
            var options = new RestClientOptions("http://localhost:5179/");
            var client = new RestClient(options);
            var request = new RestRequest("orders");

            // post request
            var order = await client.PostAsync<Order>(request, cancellationToken);

            customer.Orders.Add(order);
        } catch (Exception ex) {
            throw ex;
        }

        // metric
        var tagList = new TagList();
        tagList.Add("customer.id", customer.CustomerId);
        tagList.Add("customer.email", customer.Email);

        OTel.Meters.AddGetCustomer(1, tagList);

        // set tags
        activity?.SetTag("customer.id", customer.CustomerId);
        activity?.SetTag("customer.email", customer.Email);

        // event
        var tags = new ActivityTagsCollection();
        tags["customer.id"] = customer.CustomerId;
        tags["customer.email"] = customer.Email;

        var e = new ActivityEvent("MrJB.OTel.Customers.API.Get", DateTimeOffset.Now, tags);
        activity?.AddEvent(e);

        activity?.SetStatus(ActivityStatusCode.Ok);

        return customer;
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

app.Run();
