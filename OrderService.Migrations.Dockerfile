FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
RUN dotnet tool install --global dotnet-ef --version 8.0.2
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy csproj and restore dependencies
COPY BookStore.Authentication.Jwt/BookStore.Authentication.Jwt.csproj ./BookStore.Authentication.Jwt/
COPY BookStore.Authentication.Jwt.KafkaLoggedOut/BookStore.Authentication.Jwt.KafkaLoggedOut.csproj ./BookStore.Authentication.Jwt.KafkaLoggedOut/
COPY BookStore.Authentication.Jwt.Redis/BookStore.Authentication.Jwt.Redis.csproj ./BookStore.Authentication.Jwt.Redis/
COPY BookStore.Contracts/BookStore.Contracts.csproj ./BookStore.Contracts/
COPY BookStore.EventLog.Kafka/BookStore.EventLog.Kafka.csproj ./BookStore.EventLog.Kafka/
COPY BookStore.EventObserver/BookStore.EventObserver.csproj ./BookStore.EventObserver/
COPY BookStore.RedisLock/BookStore.RedisLock.csproj ./BookStore.RedisLock/
COPY BookStore.Repository/BookStore.Repository.csproj ./BookStore.Repository/


COPY OrderService/OrderService.csproj ./OrderService/
RUN dotnet restore ./OrderService/OrderService.csproj

# Copy the remaining files and build the application
COPY . ./

RUN dotnet ef migrations bundle --project OrderService/OrderService.csproj -o migrations-bundle



# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/migrations-bundle ./migrations-bundle
COPY --from=build-env /App/OrderService/appsettings.json ./appsettings.json