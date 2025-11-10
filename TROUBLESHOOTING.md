# 🔧 Решение проблем с Kafka

## ❌ Проблема: Сообщения не приходят в ProcessMessageAsync

### 🎯 **Главная причина:**
**Docker Desktop не запущен → Kafka не работает → сообщения не передаются**

---

## ✅ **Пошаговое решение:**

### Шаг 1: Запустите Docker Desktop

1. Найдите **Docker Desktop** в меню Пуск
2. Запустите приложение
3. **Дождитесь**, пока иконка Docker в трее станет зеленой (это важно!)
4. Это может занять 30-60 секунд

### Шаг 2: Проверьте, что Docker работает

```powershell
docker --version
docker ps
```

Если видите ошибку `pipe/dockerDesktopLinuxEngine` - Docker еще не запустился полностью.

### Шаг 3: Запустите Kafka и Zookeeper

```powershell
# В корневой папке проекта
docker-compose up -d
```

Ожидаемый результат:
```
✔ Container zookeeper  Started
✔ Container kafka      Started
```

### Шаг 4: Проверьте, что контейнеры работают

```powershell
docker ps
```

Должны быть запущены:
```
CONTAINER ID   IMAGE                             STATUS         PORTS
xxxxx          confluentinc/cp-kafka:7.4.0       Up X seconds   0.0.0.0:9092->9092/tcp
xxxxx          confluentinc/cp-zookeeper:7.4.0   Up X seconds   0.0.0.0:2181->2181/tcp
```

### Шаг 5: Проверьте логи Kafka

```powershell
docker logs kafka -f
```

Ищите строку: `[KafkaServer id=1] started`

### Шаг 6: Перезапустите Order Service

```powershell
# Ctrl+C для остановки
cd "Order Service"
dotnet run
```

### Шаг 7: Перезапустите Notification Service

```powershell
# В новом терминале
cd "Notification Service"
dotnet run
```

**Важно!** Смотрите на логи при запуске. Должны увидеть:
```
info: NotificationService.Services.KafkaConsumerService[0]
      Kafka Consumer Service is starting.
info: NotificationService.Services.KafkaConsumerService[0]
      Subscribed to topics: order-created
```

### Шаг 8: Создайте тестовый заказ

Откройте http://localhost:5252/swagger

Или через curl:
```powershell
curl -X POST http://localhost:5252/api/order/create `
  -H "Content-Type: application/json" `
  -d '{
    "productId": "650e8400-e29b-41d4-a716-446655440000",
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "productName": "Test Product",
    "quantity": 5
  }'
```

---

## 📋 **Чек-лист для проверки:**

- [ ] Docker Desktop запущен и работает
- [ ] `docker ps` показывает zookeeper и kafka
- [ ] Order Service запущен на порту 5252
- [ ] Notification Service запущен на порту 5297
- [ ] При запуске Notification Service видно "Subscribed to topics: order-created"
- [ ] PostgreSQL работает (база ShopDB существует)
- [ ] В базе есть тестовый пользователь

---

## 🔍 **Дополнительная диагностика:**

### Проверить, что топик создан:

```powershell
docker exec -it kafka kafka-topics --list --bootstrap-server localhost:9092
```

Должен быть топик: `order-created`

### Посмотреть сообщения в топике:

```powershell
docker exec -it kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic order-created --from-beginning
```

### Проверить, что Producer отправляет сообщения:

В логах Order Service после создания заказа должно быть:
```
info: OrderService.Services.KafkaProducerService[0]
      Delivered message to [order-created [0] @0] | Key=<order-id>
```

### Проверить, что Consumer получает сообщения:

В логах Notification Service должно появиться:
```
info: NotificationService.Services.KafkaConsumerService[0]
      Received message: Topic=order-created, Partition=0, Offset=X, Key=<order-id>
info: NotificationService.Services.KafkaConsumerService[0]
      Processing order: OrderId=<order-id>, Product=Test Product, Quantity=5
info: NotificationService.Services.EmailNotificationService[0]
      ✅ Notification sent successfully: Order #<order-id>
```

---

## ⚠️ **Типичные ошибки:**

### 1. "Connection refused" в логах
**Причина:** Kafka не запущен
**Решение:** Запустите `docker-compose up -d`

### 2. "Broker may not be available"
**Причина:** Kafka еще загружается
**Решение:** Подождите 10-20 секунд и перезапустите сервисы

### 3. "Group coordinator not available"
**Причина:** Zookeeper не готов
**Решение:** Проверьте `docker logs zookeeper`, подождите и перезапустите

### 4. Consumer не получает старые сообщения
**Причина:** AutoOffsetReset настроен неправильно
**Решение:** Проверьте `appsettings.json` → должно быть `"AutoOffsetReset": "earliest"`

### 5. Дублирование сообщений
**Причина:** Consumer не коммитит offset
**Решение:** Проверьте, что `_consumer.Commit()` вызывается после обработки

---

## 🧪 **Тестирование потока:**

### 1. Отправить тестовое сообщение напрямую в Kafka:

```powershell
docker exec -it kafka kafka-console-producer --bootstrap-server localhost:9092 --topic order-created
```

Введите:
```json
{"Id":"550e8400-e29b-41d4-a716-446655440000","ProductName":"Manual Test","Quantity":1,"CreatedAt":"2025-09-30T12:00:00Z"}
```

Notification Service должен получить и обработать это сообщение.

### 2. Проверить количество сообщений в топике:

```powershell
docker exec -it kafka kafka-run-class kafka.tools.GetOffsetShell --broker-list localhost:9092 --topic order-created
```

---

## 🆘 **Если ничего не помогает:**

### Полная перезагрузка:

```powershell
# 1. Остановите все сервисы (Ctrl+C)

# 2. Остановите и удалите Kafka
docker-compose down -v

# 3. Запустите заново
docker-compose up -d

# 4. Подождите 30 секунд

# 5. Запустите сервисы
cd "Order Service"
dotnet run

# В новом терминале:
cd "Notification Service"
dotnet run
```

### Проверка портов:

```powershell
# Убедитесь, что порты свободны:
netstat -an | findstr :9092   # Kafka
netstat -an | findstr :2181   # Zookeeper
netstat -an | findstr :5252   # Order Service
netstat -an | findstr :5297   # Notification Service
```

---

## 📞 **Где искать логи:**

1. **Order Service** - терминал, где запущен `dotnet run`
2. **Notification Service** - терминал, где запущен `dotnet run`
3. **Kafka** - `docker logs kafka -f`
4. **Zookeeper** - `docker logs zookeeper -f`


