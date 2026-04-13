FROM nginx:mainline-alpine3.23-perl AS base
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NaeRaces.BlazorWebApp/NaeRaces.BlazorWebApp.csproj", "NaeRaces.BlazorWebApp/"]
COPY ["NaeRaces.WebAPI.Shared/NaeRaces.WebAPI.Shared.csproj", "NaeRaces.WebAPI.Shared/"]
RUN dotnet restore "./NaeRaces.BlazorWebApp/NaeRaces.BlazorWebApp.csproj"
COPY . .
WORKDIR "/src/NaeRaces.BlazorWebApp"
RUN dotnet build "./NaeRaces.BlazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NaeRaces.BlazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
COPY --from=publish /app/publish /app
COPY NaeRaces.BlazorWebApp/nginx.conf /etc/nginx/nginx.conf