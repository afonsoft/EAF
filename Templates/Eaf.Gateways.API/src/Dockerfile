#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Eaf.Gateways.API.csproj", "src/"]
RUN dotnet restore "src/Eaf.Gateways.API.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "src/Eaf.Gateways.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Eaf.Gateways.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN groupadd -r app && useradd -r -g app app && chown -R app:app /app

USER app

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 CMD dotnet --info || exit 1

ENTRYPOINT ["dotnet", "Eaf.Gateways.API.dll"]