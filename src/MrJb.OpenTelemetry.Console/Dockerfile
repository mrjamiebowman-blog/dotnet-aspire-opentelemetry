FROM mcr.microsoft.com/dotnet/runtime:9.0-preview AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MrJb.NetAspire.OpenTelemetry/MrJb.NetAspire.OpenTelemetry.csproj", "src/MrJb.NetAspire.OpenTelemetry/"]
COPY ["aspire/MrJb.NetAspire.ServiceDefaults/MrJb.NetAspire.ServiceDefaults.csproj", "aspire/MrJb.NetAspire.ServiceDefaults/"]
RUN dotnet restore "./src/MrJb.NetAspire.OpenTelemetry/MrJb.NetAspire.OpenTelemetry.csproj"
COPY . .
WORKDIR "/src/src/MrJb.NetAspire.OpenTelemetry"
RUN dotnet build "./MrJb.NetAspire.OpenTelemetry.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJb.NetAspire.OpenTelemetry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJb.NetAspire.OpenTelemetry.dll"]