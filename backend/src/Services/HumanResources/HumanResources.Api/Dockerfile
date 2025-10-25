#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/HumanResources/HumanResources.Api/HumanResources.Api.csproj", "Services/HumanResources/HumanResources.Api/"]
COPY ["Services/HumanResources/HumanResources.Infrastructure/HumanResources.Infrastructure.csproj", "Services/HumanResources/HumanResources.Infrastructure/"]
COPY ["Services/HumanResources/HumanResources.AppLogic/HumanResources.AppLogic.csproj", "Services/HumanResources/HumanResources.AppLogic/"]
COPY ["Services/HumanResources/HumanResources.Domain/HumanResources.Domain.csproj", "Services/HumanResources/HumanResources.Domain/"]
RUN dotnet restore "./Services/HumanResources/HumanResources.Api/HumanResources.Api.csproj"
COPY . .
WORKDIR "/src/Services/HumanResources/HumanResources.Api"
RUN dotnet build "./HumanResources.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HumanResources.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HumanResources.Api.dll"]