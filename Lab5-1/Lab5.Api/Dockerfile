# build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# копіюємо solution + csproj
COPY Lab5.sln ./
COPY Lab5.Api/Lab5.Api.csproj Lab5.Api/
COPY Lab5.Data/Lab5.Data.csproj Lab5.Data/

RUN dotnet restore Lab5.Api/Lab5.Api.csproj

# копіюємо все
COPY . .

WORKDIR /src/Lab5.Api
RUN dotnet publish -c Release -o /app

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Lab5.Api.dll"]