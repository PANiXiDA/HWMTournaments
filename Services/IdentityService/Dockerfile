# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8000


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Services/IdentityService/IdentityService.csproj", "Services/IdentityService/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
COPY ["Common/Common.Helpers/Common.Helpers.csproj", "Common/Common.Helpers/"]
COPY ["DAL/DAL.DependencyInjection/DAL.DependencyInjection.csproj", "DAL/DAL.DependencyInjection/"]
COPY ["DAL/DAL.Implementations/DAL.Implementations.csproj", "DAL/DAL.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["DAL/DAL.EF/DAL.EF.csproj", "DAL/DAL.EF/"]
COPY ["DAL/DAL.DbModels/DAL.DbModels.csproj", "DAL/DAL.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["DAL/DAL.Interfaces/DAL.Interfaces.csproj", "DAL/DAL.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Domain/Contracts/Contracts.csproj", "Domain/Contracts/"]
COPY ["Infrastructure/RedisClient/RedisClient.csproj", "Infrastructure/RedisClient/"]
RUN dotnet restore "./Services/IdentityService/IdentityService.csproj"
COPY . .
WORKDIR "/src/Services/IdentityService"
RUN dotnet build "./IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]