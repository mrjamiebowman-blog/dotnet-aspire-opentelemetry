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
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter(OTel.ActivitySourceName));

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

var featuredRequests = OTel.Meter.CreateCounter<int>("mrjb.otel.products.featured.get", description: "Tracks when featured products are retrieved.");

app.MapGet("/products/featured", async (IProductsService products, CancellationToken cancellationToken) =>
{
    var featured = await products.GetFeaturedProductsAsync(cancellationToken);
    featuredRequests.Add(1);
    return TypedResults.Ok(featured);
})
.WithName("GetFeaturedProducts");

app.Run();
