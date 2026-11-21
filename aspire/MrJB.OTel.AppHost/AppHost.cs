var builder = DistributedApplication.CreateBuilder(args);

// components (just for example...)
//builder.AddRabbitMQ("rabbitmq");

// jaeger for distributed tracing
//builder.AddContainer("jaeger", "jaegertracing/all-in-one");

//$ docker run -d --name jaeger \
//  -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 \
//  -p 5775:5775 / udp \
//  -p 6831:6831 / udp \
//  -p 6832:6832 / udp \
//  -p 5778:5778 \
//  -p 16686:16686 \
//  -p 14268:14268 \
//  -p 9411:9411 \
//  jaegertracing / all -in-one:1.6

// services
builder.AddDotnetProject("console", "../../src/MrJb.OpenTelemetry.Console/MrJb.OpenTelemetry.Console.csproj");

builder.AddDotnetProject("api-customers", "../../src/MrJb.OpenTelemetry.Api.Customers/MrJb.OpenTelemetry.Api.Customers.csproj");

builder.AddDotnetProject("api-orders", "../../src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj");


builder.Build().Run();
