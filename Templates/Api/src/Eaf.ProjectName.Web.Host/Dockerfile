FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8001
ENV ASPNETCORE_ENVIRONMENT=Local
ENV ASPNETCORE_URLS=http://+:8001
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
COPY ["./nuget.config", "./"]
COPY ["src/Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj", "src/Eaf.ProjectName.Web.Host/"]
COPY ["src/Eaf.ProjectName.Application/Eaf.ProjectName.Application.csproj", "src/Eaf.ProjectName.Application/"]
COPY ["src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj", "src/Eaf.ProjectName.EntityFrameworkCore/"]
COPY ["src/Eaf.ProjectName.Core/Eaf.ProjectName.Core.csproj", "src/Eaf.ProjectName.Core/"]
RUN dotnet restore "src/Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/Eaf.ProjectName.Web.Host"
RUN dotnet build "Eaf.ProjectName.Web.Host.csproj" -c Release -o /app/build --no-restore -p:PublishTrimmed=false -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:PublishReadyToRunComposite=true

FROM build AS publish
RUN dotnet publish "Eaf.ProjectName.Web.Host.csproj" -c Release -o /app/publish /p:PublishTrimmed=false -p:PublishSingleFile=false -p:PublishReadyToRun=true -p:PublishReadyToRunComposite=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV TZ="America/Sao_Paulo"

RUN apt-get update && \
    apt-get install -yq tzdata libc6-dev libgdiplus zlib1g-dev && \
    ln -fs /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata

ENTRYPOINT ["dotnet", "Eaf.ProjectName.Web.Host.dll"]
