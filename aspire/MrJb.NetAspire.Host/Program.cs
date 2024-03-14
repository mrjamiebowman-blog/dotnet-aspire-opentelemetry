using Aspire.Hosting;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;

var builder = DistributedApplication.CreateBuilder(args);

// components (just for example...)
builder.AddRabbitMQ("rabbitmq");

// jaeger for distributed tracing
builder.AddContainer("jaeger", "jaegertracing/all-in-one");

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
builder.AddProject<Projects.MrJb_NetAspire_Console>("console");

builder.Build().Run();