FROM mcr.microsoft.com/dotnet/runtime:11.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MrJB.OTel.Console/MrJB.OTel.Console.csproj", "src/MrJB.OTel.Console/"]
COPY ["src/MrJB.OTel.Domain/MrJB.OTel.Domain.csproj", "src/MrJB.OTel.Domain/"]
COPY ["aspire/MrJB.OTel.ServiceDefaults/MrJB.OTel.ServiceDefaults.csproj", "aspire/MrJB.OTel.ServiceDefaults/"]
RUN dotnet restore "./src/MrJB.OTel.Console/MrJB.OTel.Console.csproj"
COPY . .
WORKDIR "/src/src/MrJB.OTel.Console"
RUN dotnet build "./MrJB.OTel.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJB.OTel.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJB.OTel.Console.dll"]
