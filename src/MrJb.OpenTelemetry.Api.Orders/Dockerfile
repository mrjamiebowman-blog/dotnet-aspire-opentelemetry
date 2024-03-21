#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj", "src/MrJb.OpenTelemetry.Api.Orders/"]
RUN dotnet restore "./src/MrJb.OpenTelemetry.Api.Orders/MrJb.OpenTelemetry.Api.Orders.csproj"
COPY . .
WORKDIR "/src/src/MrJb.OpenTelemetry.Api.Orders"
RUN dotnet build "./MrJb.OpenTelemetry.Api.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJb.OpenTelemetry.Api.Orders.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJb.OpenTelemetry.Api.Orders.dll"]