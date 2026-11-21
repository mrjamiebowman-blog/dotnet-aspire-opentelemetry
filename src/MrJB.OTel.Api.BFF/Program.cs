using MrJB.OTel.Api.BFF;
using MrJB.OTel.Api.BFF.Clients;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// otel service defaults (otlp exporter, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// custom activity source + meter for this service
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource(OTel.ActivitySourceName));
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter(OTel.ActivitySourceName));

// OpenAPI document (/openapi/v1.json) rendered by Scalar (/scalar)
builder.Services.AddOpenApi();

// downstream apis (resolved via service discovery)
builder.Services.AddHttpClient<CustomersClient>(client => client.BaseAddress = new Uri("http://api-customers"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var customerRequests = OTel.Meter.CreateCounter<int>("mrjb.otel.bff.customers.get", description: "Tracks when the BFF retrieves a customer.");

app.MapGet("/customers", async (CustomersClient customers, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    using var activity = OTel.ActivitySource.StartActivity("BFF.GetCustomer");

    var customer = await customers.GetCustomerAsync(cancellationToken);
    if (customer is null)
    {
        activity?.SetStatus(ActivityStatusCode.Error, "Customers API returned no customer.");
        return Results.Problem("Customers API returned no customer.", statusCode: StatusCodes.Status502BadGateway);
    }

    activity?.SetTag("customer.id", customer.CustomerId);
    customerRequests.Add(1);
    logger.LogInformation("BFF retrieved customer {CustomerId}", customer.CustomerId);

    return Results.Ok(customer);
})
.WithName("GetCustomer");

app.Run();
