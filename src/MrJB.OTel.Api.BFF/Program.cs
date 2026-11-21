using MrJB.OTel.Api.BFF;
using MrJB.OTel.Api.BFF.Clients;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// otel service defaults (otlp exporter, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// custom activity source + meter for this service
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource(OTel.ActivitySourceName));
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter(OTel.Meters.Meter.Name));

builder.Services.AddControllers();

// OpenAPI document (/openapi/v1.json) rendered by Scalar (/scalar)
builder.Services.AddOpenApi();

// downstream apis (resolved via service discovery)
builder.Services.AddHttpClient<CustomersClient>(client => client.BaseAddress = new Uri("http://api-customers"));
builder.Services.AddHttpClient<ProductsClient>(client => client.BaseAddress = new Uri("http://api-products"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();
