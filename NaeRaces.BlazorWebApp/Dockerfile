# syntax=docker/dockerfile:1
FROM nginx:mainline-alpine3.23-perl AS base
EXPOSE 80


# This stage is used to build and publish the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NaeRaces.BlazorWebApp/NaeRaces.BlazorWebApp.csproj", "NaeRaces.BlazorWebApp/"]
COPY ["NaeRaces.WebAPI.Shared/NaeRaces.WebAPI.Shared.csproj", "NaeRaces.WebAPI.Shared/"]
RUN dotnet restore "./NaeRaces.BlazorWebApp/NaeRaces.BlazorWebApp.csproj"
COPY . .
WORKDIR "/src/NaeRaces.BlazorWebApp"
RUN dotnet publish "./NaeRaces.BlazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production
FROM base AS final
COPY --from=publish /app/publish /app
COPY NaeRaces.BlazorWebApp/default.conf.template /etc/nginx/templates/default.conf.template
