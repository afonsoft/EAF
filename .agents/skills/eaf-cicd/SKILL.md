---
name: eaf-cicd
description: Expert guidance for CI/CD pipelines in EAF (Enterprise Application Foundation) using GitHub Actions, shell scripts, and Docker. Covers build automation, test execution, code coverage, NuGet packaging, deployment to Azure, and EAF-specific CI/CD patterns. Use this skill when configuring GitHub Actions workflows, setting up build scripts, implementing automated testing, or troubleshooting CI/CD failures. Do NOT use for general DevOps tasks, non-EAF projects, or infrastructure as code outside CI/CD context.
---

# EAF CI/CD Skill

You are an expert in CI/CD pipelines for EAF (Enterprise Application Foundation) using GitHub Actions, shell scripts, and Docker. You configure, maintain, and troubleshoot build, test, and deployment pipelines following DevOps best practices.

## Project Context

EAF is an open source middleware platform built on ASP.NET Boilerplate (ABP). CI/CD is critical for ensuring code quality and automating the release process.

### Technology Stack
- **CI Platform**: GitHub Actions
- **Build Tool**: .NET CLI (dotnet)
- **Test Framework**: xUnit with coverlet
- **Package Manager**: NuGet
- **Container**: Docker
- **Deployment**: Azure (optional)

## GitHub Actions Workflows

### Build and Test Workflow

```yaml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore Eaf.sln
    
    - name: Build
      run: dotnet build Eaf.sln --configuration Release --no-restore
    
    - name: Test
      run: dotnet test Eaf.sln --configuration Release --no-build --collect:"XPlat Code Coverage"
    
    - name: Upload coverage
      uses: codecov/codecov-action@v4
      with:
        files: '**/coverage.cobertura.xml'
```

### NuGet Package Workflow

```yaml
name: Publish NuGet Package

on:
  push:
    tags:
      - 'v*'

jobs:
  publish:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release
    
    - name: Pack
      run: dotnet pack src/Eaf.ModuleName/Eaf.ModuleName.csproj --configuration Release --output ./nupkg
    
    - name: Publish to NuGet
      run: dotnet nuget push ./nupkg/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

## Shell Scripts

### Build and Test Script

```bash
#!/bin/bash
# build-and-test.sh

set -e

echo "Restoring dependencies..."
dotnet restore Eaf.sln

echo "Building solution..."
dotnet build Eaf.sln --configuration Release

echo "Running tests..."
dotnet test Eaf.sln --configuration Release --collect:"XPlat Code Coverage" --logger "trx;LogFileName=test-results.trx"

echo "Build and test completed successfully!"
```

### Test with Coverage Script

```bash
#!/bin/bash
# run-tests-with-coverage.sh

set -e

echo "Running tests with coverage..."
dotnet test Eaf.sln \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --settings coverlet.runsettings \
    --logger "trx;LogFileName=test-results.trx" \
    --results-directory TestResults

echo "Generating coverage report..."
reportgenerator -reports:TestResults/**/coverage.cobertura.xml -targetdir:TestResults/CoverageReport -reporttypes:Html

echo "Coverage report generated in TestResults/CoverageReport"
```

## Docker Configuration

### Dockerfile for Module

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Eaf.ModuleName/Eaf.ModuleName.csproj", "Eaf.ModuleName/"]
RUN dotnet restore "Eaf.ModuleName/Eaf.ModuleName.csproj"
COPY . .
WORKDIR "/src/Eaf.ModuleName"
RUN dotnet build "Eaf.ModuleName.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Eaf.ModuleName.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eaf.ModuleName.dll"]
```

### Docker Compose

```yaml
version: '3.8'

services:
  eaf-app:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=Server=sqlserver;Database=EafDb;User Id=sa;Password=YourPassword123
    depends_on:
      - sqlserver
  
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
```

## Code Coverage

### coverlet.runsettings

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>opencover,cobertura</Format>
          <Exclude>[Eaf.*Tests]*,[*.Module]*</Exclude>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### Coverage Report Generation

```bash
# Install reportgenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
    -reports:TestResults/**/coverage.cobertura.xml \
    -targetdir:TestResults/CoverageReport \
    -reporttypes:Html
```

## NuGet Packaging

### Module .csproj Configuration

```xml
<PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageId>Eaf.ModuleName</PackageId>
    <Version>1.0.0</Version>
    <Authors>EAF Team</Authors>
    <Description>EAF Module Description</Description>
    <PackageTags>asp.net;asp.net mvc;application framework;Eaf;Boilerplate</PackageTags>
    <PackageProjectUrl>https://github.com/afonsoft/EAF</PackageProjectUrl>
    <RepositoryUrl>https://github.com/afonsoft/EAF.git</RepositoryUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageReadmeFile>README.md</PackageReadmeFile>
</PropertyGroup>

<ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
</ItemGroup>
```

### Pack and Publish

```bash
# Pack
dotnet pack src/Eaf.ModuleName/Eaf.ModuleName.csproj --configuration Release --output ./nupkg

# Publish to NuGet.org
dotnet nuget push ./nupkg/Eaf.ModuleName.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Publish to custom feed
dotnet nuget push ./nupkg/Eaf.ModuleName.1.0.0.nupkg --api-key YOUR_API_KEY --source https://your-feed.com/nuget/v3/index.json
```

## Quality Gates

### Coverage Threshold

```yaml
- name: Check coverage
  run: |
    coverage=$(grep -oP 'Line coverage: \K[0-9.]+' TestResults/coverage.txt)
    coverage_int=$(echo $coverage | cut -d. -f1)
    if [ $coverage_int -lt 90 ]; then
      echo "Coverage is $coverage%, which is below the 90% threshold"
      exit 1
    fi
```

### Linting

```yaml
- name: Install dotnet-format
  run: dotnet tool install -g dotnet-format

- name: Check code formatting
  run: dotnet format --verify-no-changes Eaf.sln
```

## Deployment Patterns

### Azure Web App Deployment

```yaml
- name: Publish to Azure Web App
  uses: azure/webapps-deploy@v2
  with:
    app-name: 'your-app-name'
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
    package: ./publish
```

### Docker Deployment

```yaml
- name: Build Docker image
  run: docker build -t eaf-module:latest .

- name: Push to Docker Hub
  run: |
    echo ${{ secrets.DOCKER_PASSWORD }} | docker login -u ${{ secrets.DOCKER_USERNAME }} --password-stdin
    docker push your-registry/eaf-module:latest
```

## Common Issues and Solutions

### Build Failures

```bash
# Clean build
dotnet clean Eaf.sln
dotnet restore Eaf.sln
dotnet build Eaf.sln --configuration Release
```

### Test Failures in CI

```yaml
- name: Run tests with retry
  uses: nick-invision/retry@v2
  with:
    timeout_minutes: 10
    max_attempts: 3
    command: dotnet test Eaf.sln --configuration Release
```

### NuGet Package Conflicts

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore with specific version
dotnet restore Eaf.sln --source https://api.nuget.org/v3/index.json
```

## Best Practices

### Workflow Organization
- Separate workflows for different concerns (build, test, deploy)
- Use matrix strategy for multiple configurations
- Cache dependencies for faster builds
- Use secrets for sensitive data

### Build Performance
- Use `--no-restore` and `--no-build` flags
- Parallelize test execution
- Use incremental builds
- Cache NuGet packages

### Quality Assurance
- Enforce code coverage thresholds
- Run static analysis
- Use branch protection rules
- Require status checks before merging

## When in Doubt

- Use GitHub Actions for CI/CD
- Keep workflows simple and maintainable
- Use secrets for sensitive data
- Test workflows locally before pushing
- Monitor pipeline performance
- Keep dependencies up to date
- Document pipeline changes
