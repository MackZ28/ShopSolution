# Инструкция по запуску Kafka

## Шаг 1: Запустить Docker Desktop
1. Найдите Docker Desktop в меню Пуск
2. Запустите приложение
3. Дождитесь, пока Docker полностью запустится (иконка в трее станет зеленой)

## Шаг 2: Запустить Kafka и Zookeeper
Откройте PowerShell в корневой папке проекта и выполните:

```powershell
docker-compose up -d
```

## Шаг 3: Проверить, что контейнеры запущены
```powershell
docker ps
```

Должны быть запущены контейнеры:
- zookeeper (порт 2181)
- kafka (порт 9092)

## Шаг 4: Создать топик (опционально, т.к. auto-create включен)
```powershell
docker exec -it kafka kafka-topics --create --topic order-created --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```

## Шаг 5: Проверить топики
```powershell
docker exec -it kafka kafka-topics --list --bootstrap-server localhost:9092
```


