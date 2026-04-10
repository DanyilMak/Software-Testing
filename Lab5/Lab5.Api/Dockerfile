# Етап 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо лише csproj для кешування restore
COPY Lab5.Api/Lab5.Api.csproj Lab5.Api/
COPY Lab5.Data/Lab5.Data.csproj Lab5.Data/
COPY Lab5.Tests/Lab5.Tests.csproj Lab5.Tests/

# Restore залежності для API (підтягне Data і Tests через reference)
RUN dotnet restore Lab5.Api/Lab5.Api.csproj

# Копіюємо весь код
COPY . .

# Publish API
RUN dotnet publish Lab5.Api/Lab5.Api.csproj -c Release -o /app/out

# Етап 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Lab5.Api.dll"]
