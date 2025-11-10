# Скрипт проверки статуса Kafka и сервисов

Write-Host "`n=== Проверка Docker ===" -ForegroundColor Cyan
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker установлен: $dockerVersion" -ForegroundColor Green
    
    $containers = docker ps 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Docker работает" -ForegroundColor Green
        
        # Проверка контейнеров
        Write-Host "`n=== Проверка контейнеров ===" -ForegroundColor Cyan
        
        $kafkaRunning = docker ps --filter "name=kafka" --filter "status=running" -q
        if ($kafkaRunning) {
            Write-Host "✅ Kafka запущен" -ForegroundColor Green
        } else {
            Write-Host "❌ Kafka НЕ запущен" -ForegroundColor Red
            Write-Host "   Запустите: docker-compose up -d" -ForegroundColor Yellow
        }
        
        $zookeeperRunning = docker ps --filter "name=zookeeper" --filter "status=running" -q
        if ($zookeeperRunning) {
            Write-Host "✅ Zookeeper запущен" -ForegroundColor Green
        } else {
            Write-Host "❌ Zookeeper НЕ запущен" -ForegroundColor Red
            Write-Host "   Запустите: docker-compose up -d" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ Docker НЕ работает" -ForegroundColor Red
        Write-Host "   Запустите Docker Desktop!" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Docker не установлен или не найден" -ForegroundColor Red
}

Write-Host "`n=== Проверка портов ===" -ForegroundColor Cyan

# Проверка порта Kafka
$kafkaPort = netstat -an | Select-String ":9092"
if ($kafkaPort) {
    Write-Host "✅ Порт 9092 (Kafka) занят" -ForegroundColor Green
} else {
    Write-Host "❌ Порт 9092 (Kafka) свободен - Kafka не запущен" -ForegroundColor Red
}

# Проверка порта Zookeeper
$zookeeperPort = netstat -an | Select-String ":2181"
if ($zookeeperPort) {
    Write-Host "✅ Порт 2181 (Zookeeper) занят" -ForegroundColor Green
} else {
    Write-Host "❌ Порт 2181 (Zookeeper) свободен - Zookeeper не запущен" -ForegroundColor Red
}

# Проверка Order Service
$orderServicePort = netstat -an | Select-String ":5252"
if ($orderServicePort) {
    Write-Host "✅ Порт 5252 (Order Service) занят" -ForegroundColor Green
} else {
    Write-Host "⚠️  Порт 5252 (Order Service) свободен - сервис не запущен" -ForegroundColor Yellow
}

# Проверка Notification Service
$notificationServicePort = netstat -an | Select-String ":5297"
if ($notificationServicePort) {
    Write-Host "✅ Порт 5297 (Notification Service) занят" -ForegroundColor Green
} else {
    Write-Host "⚠️  Порт 5297 (Notification Service) свободен - сервис не запущен" -ForegroundColor Yellow
}

Write-Host "`n=== Проверка PostgreSQL ===" -ForegroundColor Cyan
$postgresService = Get-Service -Name "*postgres*" -ErrorAction SilentlyContinue
if ($postgresService -and $postgresService.Status -eq 'Running') {
    Write-Host "✅ PostgreSQL запущен" -ForegroundColor Green
} else {
    Write-Host "❌ PostgreSQL не запущен" -ForegroundColor Red
}

Write-Host "`n=== Проверка топиков Kafka ===" -ForegroundColor Cyan
try {
    if ($kafkaRunning) {
        Write-Host "Топики в Kafka:" -ForegroundColor White
        docker exec kafka kafka-topics --list --bootstrap-server localhost:9092 2>$null
    } else {
        Write-Host "⚠️  Kafka не запущен - невозможно проверить топики" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Не удалось получить список топиков" -ForegroundColor Red
}

Write-Host "`n=== Резюме ===" -ForegroundColor Cyan
Write-Host "Для запуска проекта необходимо:" -ForegroundColor White
Write-Host "1. Запустить Docker Desktop" -ForegroundColor White
Write-Host "2. Выполнить: docker-compose up -d" -ForegroundColor White
Write-Host "3. Запустить Order Service: cd 'Order Service' && dotnet run" -ForegroundColor White
Write-Host "4. Запустить Notification Service: cd 'Notification Service' && dotnet run" -ForegroundColor White
Write-Host ""


