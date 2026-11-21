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
builder.AddDotnetProject("console", "../../src/MrJb.OpenTelemetry.Console/MrJb.OpenTelemetry.Console.csproj")
    .WaitFor(jaeger);

builder.AddDotnetProject("api-customers", "../../src/MrJb.OpenTelemetry.Api.Customers/MrJb.OpenTelemetry.Api.Customers.csproj")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(jaeger);

builder.AddDotnetProject("api-orders", "../../src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(jaeger);

builder.Build().Run();
