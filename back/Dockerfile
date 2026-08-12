FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY . .

RUN dotnet restore "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj"

RUN dotnet publish "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /out .

RUN ls -la /app

ENTRYPOINT ["dotnet", "TimescaleDataProcessor.API.dll"]
