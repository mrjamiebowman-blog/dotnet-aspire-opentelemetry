FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# opentelemetry
ENV OTEL_EXPORTER_OTLP_ENDPOINT="http://host.docker.internal:4318"
ENV OTEL_SERVICE_NAME="mrjb.otel.api.orders"

# opentelemetry
#ENV OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:8200
#ENV OTEL_EXPORTER_OTLP_HEADERS="Authorization=Bearer <secret-token>"
ENV OTEL_METRICS_EXPORTER=otlp
ENV OTEL_LOGS_EXPORTER=otlp
ENV OTEL_RESOURCE_ATTRIBUTES=service.name=mrjb.otel.api.orders,service.version=1.0.1,deployment.environment=development

ENV OTEL_DOTNET_AUTO_HOME=/otel-ai

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
USER root
RUN apt update -y
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj", "src/MrJb.OpenTelemetry.Api.Orders/"]
RUN dotnet restore "./src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj"
COPY . .
WORKDIR "/src/src/MrJb.OpenTelemetry.Api.Orders"
RUN dotnet build "./MrJb.OpenTelemetry.Api.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ENV OTEL_DOTNET_AUTO_HOME=/otel-ai

ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJb.OpenTelemetry.Api.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# dotnet opentelemetry
RUN apt-get install -y unzip
RUN curl -sSfL https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v1.4.0/otel-dotnet-auto-install.sh -O
RUN sh otel-dotnet-auto-install.sh

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Add OTEL binaries to the published app
COPY --from=publish $OTEL_DOTNET_AUTO_HOME $OTEL_DOTNET_AUTO_HOME
ENTRYPOINT ["dotnet", "MrJb.OpenTelemetry.Api.Orders.dll"]