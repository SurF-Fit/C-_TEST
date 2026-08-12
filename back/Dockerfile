FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY TimescaleDataProcessor.API/*.csproj ./TimescaleDataProcessor.API/
COPY TimescaleDataProcessor.Core/*.csproj ./TimescaleDataProcessor.Core/
COPY TimescaleDataProcessor.Infrastructure/*.csproj ./TimescaleDataProcessor.Infrastructure/

RUN dotnet restore ./TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj

COPY . .

RUN dotnet publish ./TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TimescaleDataProcessor.API.dll"]