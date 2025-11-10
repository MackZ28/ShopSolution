# 🚀 Быстрый старт

## Минимальные шаги для запуска проекта

### 1️⃣ Запустить Docker Desktop
- Откройте Docker Desktop и дождитесь его полного запуска

### 2️⃣ Запустить Kafka
```powershell
docker-compose up -d
```

### 3️⃣ Запустить Order Service
```powershell
cd "Order Service"
dotnet run
```
📍 Откроется на http://localhost:5252

### 4️⃣ Запустить Notification Service (в новом терминале)
```powershell
cd "Notification Service"
dotnet run
```
📍 Откроется на http://localhost:5297

---

## ✅ Проверка работы

1. Откройте Swagger: http://localhost:5252/swagger
2. Создайте тестового пользователя в БД (см. README.md)
3. Создайте заказ через API
4. Проверьте логи Notification Service - должно появиться уведомление

---

## 🔍 Проверка статуса

Order Service:
```powershell
curl http://localhost:5252/api/order/status
```

Notification Service:
```powershell
curl http://localhost:5297/api/health/status
```

Kafka топики:
```powershell
docker exec -it kafka kafka-topics --list --bootstrap-server localhost:9092
```

---

## 🛑 Остановка

1. Остановите сервисы: `Ctrl+C` в терминалах
2. Остановите Kafka: `docker-compose down`



