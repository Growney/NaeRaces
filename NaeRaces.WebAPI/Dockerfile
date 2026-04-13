# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NaeRaces.WebAPI/NaeRaces.WebAPI.csproj", "NaeRaces.WebAPI/"]
COPY ["EventDbLite/EventDbLite.KurrentDb/EventDbLite.KurrentDb.csproj", "EventDbLite/EventDbLite.KurrentDb/"]
COPY ["EventDbLite/EventDbLite/EventDbLite.csproj", "EventDbLite/EventDbLite/"]
COPY ["EventDbLite/EventDbLite.Handlers/EventDbLite.Handlers.csproj", "EventDbLite/EventDbLite.Handlers/"]
COPY ["EventDbLite/EventDbLite.Serialization/EventDbLite.Serialization.csproj", "EventDbLite/EventDbLite.Serialization/"]
COPY ["EventDbLite/EventDbLite.Abstractions/EventDbLite.Abstractions.csproj", "EventDbLite/EventDbLite.Abstractions/"]
COPY ["EventDbLite/EventDbLite.Reactions.Abstractions/EventDbLite.Reactions.Abstractions.csproj", "EventDbLite/EventDbLite.Reactions.Abstractions/"]
COPY ["NaeRaces.Command/NaeRaces.Command.csproj", "NaeRaces.Command/"]
COPY ["NaeRaces.Events/NaeRaces.Events.csproj", "NaeRaces.Events/"]
COPY ["NaeRaces.Query.EntityFrameworkCore.SqlServer/NaeRaces.Query.EntityFrameworkCore.SqlServer.csproj", "NaeRaces.Query.EntityFrameworkCore.SqlServer/"]
COPY ["NaeRaces.Query.EntityFrameworkCore/NaeRaces.Query.EntityFrameworkCore.csproj", "NaeRaces.Query.EntityFrameworkCore/"]
COPY ["NaeRaces.Query/NaeRaces.Query.csproj", "NaeRaces.Query/"]
COPY ["NaeRaces.WebAPI.Shared/NaeRaces.WebAPI.Shared.csproj", "NaeRaces.WebAPI.Shared/"]
RUN dotnet restore "./NaeRaces.WebAPI/NaeRaces.WebAPI.csproj"
COPY . .
WORKDIR "/src/NaeRaces.WebAPI"
RUN dotnet build "./NaeRaces.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NaeRaces.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NaeRaces.WebAPI.dll"]