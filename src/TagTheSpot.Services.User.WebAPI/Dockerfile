# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TagTheSpot.Services.User.WebAPI/TagTheSpot.Services.User.WebAPI.csproj", "TagTheSpot.Services.User.WebAPI/"]
COPY ["TagTheSpot.Services.User.Application/TagTheSpot.Services.User.Application.csproj", "TagTheSpot.Services.User.Application/"]
COPY ["TagTheSpot.Services.User.Domain/TagTheSpot.Services.User.Domain.csproj", "TagTheSpot.Services.User.Domain/"]
COPY ["TagTheSpot.Services.User.Infrastructure/TagTheSpot.Services.User.Infrastructure.csproj", "TagTheSpot.Services.User.Infrastructure/"]
RUN dotnet restore "./TagTheSpot.Services.User.WebAPI/TagTheSpot.Services.User.WebAPI.csproj"
COPY . .
WORKDIR "/src/TagTheSpot.Services.User.WebAPI"
RUN dotnet build "./TagTheSpot.Services.User.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TagTheSpot.Services.User.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TagTheSpot.Services.User.WebAPI.dll"]