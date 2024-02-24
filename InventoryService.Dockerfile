FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy csproj and restore dependencies
COPY BookStore.Authentication.Jwt/BookStore.Authentication.Jwt.csproj ./BookStore.Authentication.Jwt/
COPY BookStore.Authentication.Jwt.KafkaLoggedOut/BookStore.Authentication.Jwt.KafkaLoggedOut.csproj ./BookStore.Authentication.Jwt.KafkaLoggedOut/
COPY BookStore.Authentication.Jwt.Redis/BookStore.Authentication.Jwt.Redis.csproj ./BookStore.Authentication.Jwt.Redis/
COPY BookStore.Contracts/BookStore.Contracts.csproj ./BookStore.Contracts/
COPY BookStore.EventLog.Kafka/BookStore.EventLog.Kafka.csproj ./BookStore.EventLog.Kafka/
COPY BookStore.EventObserver/BookStore.EventObserver.csproj ./BookStore.EventObserver/
COPY BookStore.RedisLock/BookStore.RedisLock.csproj ./BookStore.RedisLock/
COPY BookStore.Repository/BookStore.Repository.csproj ./BookStore.Repository/


COPY InventoryService/InventoryService.csproj ./InventoryService/
RUN dotnet restore ./InventoryService/InventoryService.csproj

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish ./InventoryService/InventoryService.csproj -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "InventoryService.dll"]