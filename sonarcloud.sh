#!/bin/bash
# SonarCloud Scan Script for EAF Framework
# https://pumpingco.de/blog/how-to-run-a-sonarcloud-scan-during-docker-builds-for-dotnet-core/
# Updated for modern SonarScanner and .NET 10.0

set -e

echo "🔍 Starting SonarCloud scan for EAF Framework"
echo "============================================"

# Install required tools
echo "📦 Installing required tools..."
dotnet tool install --global --ignore-failed-sources dotnet-sonarscanner || true
dotnet tool install --global --ignore-failed-sources coverlet.console || true

# Export PATH for dotnet tools
export PATH="$PATH:$HOME/.dotnet/tools"

# Restore dependencies
echo "🔧 Restoring dependencies..."
dotnet restore Eaf.sln --ignore-failed-sources --configfile nuget.config || true
dotnet restore Eaf.sln --ignore-failed-sources

# Begin SonarCloud analysis
echo "🚀 Starting SonarCloud analysis..."
dotnet-sonarscanner begin \
    /o:"afonsoft" \
    /k:"afonsoft_EAF2" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.token="${SONAR_TOKEN}" \
    /d:sonar.scm.provider="git" \
    /d:sonar.coverage.exclusions="**Test*.cs" \
    /d:sonar.exclusions="Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/metronic/assets/vendors/base/scripts.bundle.js,Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/mdbootstrap/mdb.min.js,**/*.Designer.cs,**/service-proxies.ts,dotnet-install.sh,Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/primeng/**,Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/ngx-bootstrap/**,**/node_modules/**,**/dist/**,**/bin/**,**/obj/**,**/*.min.js,**/*.bundle.js" \
    /d:sonar.cpd.exclusions="Templates/**" \
    /d:sonar.cs.vstest.reportsPaths="resultTest/**/*.trx" \
    /d:sonar.cs.opencover.reportsPaths="resultTest/**/coverage.opencover.xml"

# Build solution
echo "🔨 Building solution..."
dotnet build Eaf.sln --configuration Release --verbosity minimal --no-incremental --ignore-failed-sources

# Run tests with coverage
echo ""
echo "🧪 Running tests with coverage..."
echo "==============================="

# Clean previous test results
rm -rf resultTest/
rm -f coverage.opencover.xml

# Test projects that exist in EAF
test_projects=(
    "test/Eaf.Castle.Serilog.Tests/Eaf.Castle.Serilog.Tests.csproj"
    "test/Eaf.KeyVault.Tests/Eaf.KeyVault.Tests.csproj"
    "test/Eaf.KeyVault.AspNetCore.Tests/Eaf.KeyVault.AspNetCore.Tests.csproj"
    "test/Eaf.Log4NetServiceBus.Tests/Eaf.Log4NetServiceBus.Tests.csproj"
    "test/Eaf.Middleware.Application.Tests/Eaf.Middleware.Application.Tests.csproj"
    "test/Eaf.Middleware.AzureActiveDirectory.Tests/Eaf.Middleware.AzureActiveDirectory.Tests.csproj"
    "test/Eaf.Middleware.Ldap.Tests/Eaf.Middleware.Ldap.Tests.csproj"
    "test/Eaf.Middleware.Worker.Tests/Eaf.Middleware.Worker.Tests.csproj"
    "test/Eaf.Middleware.Web.Core.Tests/Eaf.Middleware.Web.Core.Tests.csproj"
    "test/Eaf.MiddlewareCore.Tests/Eaf.MiddlewareCore.Tests.csproj"
    "test/Eaf.OpenTelemetry.Tests/Eaf.OpenTelemetry.Tests.csproj"
    "test/Eaf.SqliteCache.Tests/Eaf.SqliteCache.Tests.csproj"
    "test/Eaf.SqlServerCache.Tests/Eaf.SqlServerCache.Tests.csproj"
)

for project in "${test_projects[@]}"; do
    echo "▶️  Testing: $project"
    project_name=$(basename "$project" .csproj)
    
    if dotnet test "$project" \
        --collect:"XPlat Code Coverage" \
        --logger "trx;LogFileName=${project_name}.trx" \
        --results-directory resultTest/ \
        --no-build \
        --no-restore \
        --configuration Release \
        --settings coverlet.runsettings \
        --verbosity quiet; then
        echo "✅ Success: $project"
    else
        echo "⚠️  Failed: $project (continuing...)"
    fi
done

# Coverage reports are collected by the dotnet test --collect step and reported via wildcards.

# End SonarCloud analysis
echo ""
echo "🏁 Ending SonarCloud analysis..."
dotnet-sonarscanner end /d:sonar.token="${SONAR_TOKEN}"

echo ""
echo "✅ SonarCloud scan completed!"
echo "📊 Check results at: https://sonarcloud.io/dashboard?id=afonsoft_EAF2"
