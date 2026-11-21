var builder = DistributedApplication.CreateBuilder(args);

// components
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// jaeger for distributed tracing (UI: http://localhost:16686)
// the services export OTLP to http://localhost:4317 (gRPC), so the host ports are fixed
var jaeger = builder.AddContainer("jaeger", "jaegertracing/jaeger", "2.5.0")
    .WithHttpEndpoint(port: 16686, targetPort: 16686, name: "ui")
    .WithEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc", scheme: "http")
    .WithHttpEndpoint(port: 4318, targetPort: 4318, name: "otlp-http");

// services
builder.AddDotnetProject("console", "../../src/MrJB.OTel.Console/MrJB.OTel.Console.csproj")
    .WaitFor(jaeger);

var customers = builder.AddDotnetProject("api-customers", "../../src/MrJB.OTel.Api.Customers/MrJB.OTel.Api.Customers.csproj")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(jaeger);

builder.AddDotnetProject("api-orders", "../../src/MrJB.OTel.Api.Orders/MrJB.OTel.Api.Orders.csproj")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(jaeger);

var products = builder.AddDotnetProject("api-products", "../../src/MrJB.OTel.Api.Products/MrJB.OTel.Api.Products.csproj")
    .WaitFor(jaeger);

// bff aggregates customers + products (resolved via service discovery as http://api-customers / http://api-products)
builder.AddDotnetProject("api-bff", "../../src/MrJB.OTel.Api.BFF/MrJB.OTel.Api.BFF.csproj")
    .WithReference(customers)
    .WithReference(products)
    .WaitFor(customers)
    .WaitFor(products);

builder.Build().Run();
