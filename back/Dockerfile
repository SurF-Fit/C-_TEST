FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# РљРѕРїРёСЂСѓРµРј РІСЃРµ С„Р°Р№Р»С‹ РїСЂРѕРµРєС‚Р°
COPY . .

# Р’РѕСЃСЃС‚Р°РЅР°РІР»РёРІР°РµРј Р·Р°РІРёСЃРёРјРѕСЃС‚Рё
RUN dotnet restore "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj"

# РџСѓР±Р»РёРєСѓРµРј РїСЂРёР»РѕР¶РµРЅРёРµ
RUN dotnet publish "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# РљРѕРїРёСЂСѓРµРј РѕРїСѓР±Р»РёРєРѕРІР°РЅРЅРѕРµ РїСЂРёР»РѕР¶РµРЅРёРµ
COPY --from=build /out .

# РџСЂРѕРІРµСЂСЏРµРј СЃРѕРґРµСЂР¶РёРјРѕРµ
RUN ls -la /app

# РўРѕС‡РєР° РІС…РѕРґР°
ENTRYPOINT ["dotnet", "TimescaleDataProcessor.API.dll"]
