# ShopSolution
Микросервисная система для управления заказами, аутентификацией и уведомлениями с использованием .NET 8, PostgreSQL, Docker, Apache Kafka

# Основные компоненты
* Order Service — прием и обработка заказов (REST API, Kafka Producer)
* Notification Service — отправка уведомлений (Kafka Consumer, Email/SMS логика по событиям)
* Authentication Service — аутентификация и авторизация (JWT, Identity)
* Common — общий проект с моделями
* PostgreSQL — СУБД для сервисов
* Apache Kafka + Zookeeper — брокер сообщений для событийных коммуникаций

# Что нужно для запуска: 
* .NET 8.0 SDK
* Docker(необходимо развернуть Kafka в docker) 
* PostgreSQL 15+
  
# Запуск 
* ```sh docker-compose up -d ``` ( docker ps - Должны быть: zookeeper и kafka)

# Настройка БД 
* Надо создать 2 БД :  ShopDB (Order/Notification) и AuthDB (Authentication)
* Миграции накатить вручную если необходимо:
  ```sh
cd "Order Service"
dotnet ef database update

cd "../Authentication Service"
dotnet ef database update
```
