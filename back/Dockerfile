FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TimescaleDataProcessor.API/*.csproj TimescaleDataProcessor.API/
COPY TimescaleDataProcessor.Core/*.csproj TimescaleDataProcessor.Core/
COPY TimescaleDataProcessor.Infrastructure/*.csproj TimescaleDataProcessor.Infrastructure/

RUN dotnet restore TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj

COPY . .

WORKDIR /src/TimescaleDataProcessor.API
RUN dotnet build TimescaleDataProcessor.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish TimescaleDataProcessor.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimescaleDataProcessor.API.dll"]