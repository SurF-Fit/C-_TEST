# fix-dockerfile.ps1
Write-Host "🔧 Исправление Dockerfile" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Yellow

# 1. Создать правильный Dockerfile
Write-Host "`n📝 Создание Dockerfile..." -ForegroundColor Yellow
@'
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Копируем все файлы проекта
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj"

# Публикуем приложение
RUN dotnet publish "TimescaleDataProcessor.API/TimescaleDataProcessor.API.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=build /out .

# Проверяем содержимое
RUN ls -la /app

# Точка входа
ENTRYPOINT ["dotnet", "TimescaleDataProcessor.API.dll"]
'@ | Out-File -FilePath "back/Dockerfile" -Encoding UTF8

# 2. Остановить старый контейнер
Write-Host "`n⏹️ Остановка старого контейнера..." -ForegroundColor Yellow
docker stop timescale-api 2>$null
docker rm timescale-api 2>$null

# 3. Удалить старый образ
Write-Host "`n🗑️ Удаление старого образа..." -ForegroundColor Yellow
docker rmi c_test-api -f 2>$null

# 4. Очистить кэш
Write-Host "`n🧹 Очистка кэша Docker..." -ForegroundColor Yellow
docker system prune -f

# 5. Пересобрать образ
Write-Host "`n🔨 Пересборка образа..." -ForegroundColor Yellow
cd back
docker build -f Dockerfile -t c_test-api .
cd ..

# 6. Запустить контейнер
Write-Host "`n🚀 Запуск контейнера..." -ForegroundColor Yellow
docker run -d \
  --name timescale-api \
  -p 5000:80 \
  --network c_test_app-network \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Database=timescaledb;Username=postgres;Password=postgres" \
  c_test-api

# 7. Подождать запуска
Write-Host "`n⏳ Ожидание запуска (5 секунд)..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# 8. Проверить логи
Write-Host "`n📋 Логи API:" -ForegroundColor Cyan
docker logs timescale-api --tail=30

# 9. Проверить API
Write-Host "`n🌐 Проверка API..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/data/results" -UseBasicParsing -ErrorAction Stop
    Write-Host "✅ API работает! Код: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ API не отвечает" -ForegroundColor Red
    Write-Host "Ошибка: $($_.Exception.Message)" -ForegroundColor Yellow
}

# 10. Информация
Write-Host "`n📌 Информация:" -ForegroundColor Cyan
Write-Host "📍 Swagger UI: http://localhost:5000/swagger" -ForegroundColor Green
Write-Host "📍 API: http://localhost:5000/api" -ForegroundColor Green
Write-Host "`n🐳 Команды:" -ForegroundColor Yellow
Write-Host "  docker logs -f timescale-api  # Смотреть логи" -ForegroundColor White
Write-Host "  docker stop timescale-api     # Остановить" -ForegroundColor White
Write-Host "  docker start timescale-api    # Запустить" -ForegroundColor White