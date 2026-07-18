FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=Production
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
ENV ConnectionStrings__Default=""
ENV Database__Provider=SqlServer
ENV Hangfire__IsEnabled=false
ENV SqlServerCache__IsEnabled=false
ENV RedisCache__IsEnabled=false

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Templates/Worker/src/Eaf.ProjectName.WorkerService"
RUN dotnet restore "Eaf.ProjectName.WorkerService.csproj"
RUN dotnet build "Eaf.ProjectName.WorkerService.csproj" \
    -c Release \
    -o /app/build \
    --no-restore

FROM build AS publish
RUN dotnet publish "Eaf.ProjectName.WorkerService.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    -p:PublishTrimmed=false \
    -p:PublishSingleFile=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV TZ="America/Sao_Paulo"

RUN apt-get update && \
    apt-get install -yq --no-install-recommends \
        libc6-dev \
        libgdiplus \
        procps \
        tzdata \
        zlib1g-dev && \
    ln -fs /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    rm -rf /var/lib/apt/lists/* && \
    chown -R app:app /app

USER app

ENTRYPOINT ["dotnet", "Eaf.ProjectName.WorkerService.dll"]
