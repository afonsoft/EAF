#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
    apt-get install -yq tzdata libc6-dev libgdiplus zlib1g-dev && \
    ln -fs /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata

USER app

ENTRYPOINT ["dotnet", "Eaf.ProjectName.WorkerService.dll"]