FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Local
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_GCServer=1
ENV DOTNET_TieredCompilation=1
ENV DOTNET_ReadyToRun=true
ENV DOTNET_EventPipeEnable=true
ENV DOTNET_JitStackTraceDepth=1
ENV DOTNET_ThreadPool_UnfairSemaphoreSpinLimit=5000
ENV DOTNET_GCHeapCount=2
ENV DOTNET_GCHeapAffinitize=0
ENV DOTNET_GCLatencyMode=1
ENV DOTNET_GCConcurrent=1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Eaf.ProjectName.WorkerService/Eaf.ProjectName.WorkerService.csproj", "Eaf.ProjectName.WorkerService/"]
RUN dotnet restore "Eaf.ProjectName.WorkerService/Eaf.ProjectName.WorkerService.csproj"
COPY . .
WORKDIR "/src/Eaf.ProjectName.WorkerService"
RUN dotnet build "Eaf.ProjectName.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Eaf.ProjectName.WorkerService.csproj" -c Release -o /app/publish

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
	
USER app 

ENTRYPOINT ["dotnet", "Eaf.ProjectName.WorkerService.dll"]