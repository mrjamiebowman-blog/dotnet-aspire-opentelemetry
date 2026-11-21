using MrJB.OTel.Api.Products;
using MrJB.OTel.Api.Products.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// otel service defaults (otlp exporter, health checks, service discovery)
builder.AddServiceDefaults();

// custom activity source + meter for this service
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource(OTel.ActivitySourceName));
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter(OTel.Meters.Meter.Name));

builder.Services.AddControllers();

// OpenAPI document (/openapi/v1.json) rendered by Scalar (/scalar)
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IProductsService, ProductsService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();
