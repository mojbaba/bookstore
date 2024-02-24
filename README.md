# bookstore
```
Microservices
│
├── UserService/
│
├── TokenService/
│
├── InventoryService/
│
└── OrderService/
```

## Requirements

- [Docker](https://www.docker.com/)

- [Docker Compose](https://docs.docker.com/compose/)

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Running the tests

### 1. Clone the repository

```bash
git clone https://github.com/mojbaba/bookstore.git
```

### 2. Change into the directory

```bash
cd bookstore
```

There are integration tests which test the services in isolation but with the real database, redis and kafka. They test each service use case and the flow of the services.

*there should be lots of unit tests for each service, but I didn't have enough time to implement them.*

```bash
dotnet test
```


## Running the application

### 1. Clone the repository

```bash
git clone https://github.com/mojbaba/bookstore.git
```

### 2. Change into the directory

```bash
cd bookstore
```

### 3. Run the infrastructure

```bash
docker compose --file docker-compose.infra.yml up -d
```

### 4. Run the migrations

```bash
docker compose --file docker-compose.migrations.yml up -d
```

### 5. Run the application

```bash
docker compose --file docker-compose.apis.yml up -d
```

### 6. Open your browser 
    
- User Service : [http://localhost:8081](http://localhost:8081/swagger/index.html)

- Inventory Service: [http://localhost:808](http://localhost:8080/swagger/index.html)

- Token Service: [http://localhost:808](http://localhost:8082/swagger/index.html)

- Order Service: [http://localhost:808](http://localhost:8083/swagger/index.html)

*it was better to have a single entry point for all services, but I didn't have enough time to implement it. (Nginx, Ocelot, etc.)*

### 7. Register a user

on Swagger UI (UserService -> register) 

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

### 8. Login and get a token

on Swagger UI (UserService -> login) 

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

and get a token

```json
{
  "email": "user@example.com",
  "token": "{JWT_TOKEN}"
}
```

### 9. Create a book

on Swagger UI (InventoryService -> create)

```json
{
  "title": "Book store micoservice architecure",
  "author": "Mojbaba",
  "price": 20,
  "amount": 1
}
```

get the book id

```json
{
  "bookId": "{GUID}"
}
```

### 10. Authorize to the Token Service

on Swagger UI use `Authorize` button and add the token `{JWT_TOKEN}` got from step 8.

### 11. Add Book Purchase Token

on Swagger UI (TokenService -> add)

```json
{
  "amount": 1500
}
```

### 12. Authorize to the Order Service

on Swagger UI use `Authorize` button and add the token `{JWT_TOKEN}` got from step 8.

### 13. Create an order

on Swagger UI (OrderService -> create)

```json
{
  "bookIds": [
    "{the book id got from step 9}"
  ]
}
```

### 14. Get the order

on Swagger UI (OrderService -> Admin Orders)

you can track the order statuses.

`"isPaymentProcessed": true` and `"isInventoryProcessed": true` means the order is processed successfully.

`"isPaymentProcessed": true` means the TokenService got the OrderCreatedKafkaEvent and deducted the amount from the user's account



```json
[
  {
    "id": "string",
    "userId": "string",
    "status": 0,
    "createdAt": "2024-02-24T03:44:56.287Z",
    "isPaymentProcessed": true,
    "isInventoryProcessed": true,
    "failReason": "string"
  }
]
```

### 15. Track the user balance

on Swagger UI (TokenService -> balance) (must authorize with the token got from step 8)

```json
{
  "userId": "{GUID}",
  "balance": {NEW_BALANCE}
}
```

the user's balance is deducted by 1500 after the order is processed.

