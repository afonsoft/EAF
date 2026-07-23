#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV ASPNETCORE_ENVIRONMENT=Local
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_gcServer=true
ENV DOTNET_TieredCompilation=1
ENV DOTNET_ReadyToRun=true
ENV DOTNET_EventPipeEnable=true
ENV DOTNET_JitStackTraceDepth=1
ENV DOTNET_ThreadPool_UnfairSemaphoreSpinLimit=5000
ENV DOTNET_GCHeapCount=2
ENV DOTNET_GCHeapAffinitize=0
ENV DOTNET_GCLatencyMode=1
ENV DOTNET_gcConcurrent=true
ENV DOTNET_PROCESSOR_COUNT=2
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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

ENV TZ="America/Sao_Paulo"

RUN apt-get update && \
    apt-get install -yq --no-install-recommends \
        libc6-dev \
        libgdiplus \
        tzdata \
        zlib1g-dev && \
    ln -fs /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    rm -rf /var/lib/apt/lists/* && \
    chown -R app:app /app

# set to use the non-root USER here
USER app

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 CMD dotnet --info || exit 1

ENTRYPOINT ["dotnet", "Eaf.Gateways.API.dll"]