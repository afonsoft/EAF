#!/bin/bash
#
# Script para build e teste do EAF Framework
# EAF Framework - .NET 10.0
#
echo "🔨 Build e Teste do EAF Framework"
echo "================================="
#
# Build dos projetos principais
echo "🔨 Fazendo build dos projetos principais..."
#
# Lista de projetos principais que compilam sem problemas
main_projects=(
    "src/Eaf.Castle.Serilog/Eaf.Castle.Serilog.csproj"
    "src/Eaf.KeyVault/Eaf.KeyVault.csproj"
    "src/Eaf.KeyVault.AspNetCore/Eaf.KeyVault.AspNetCore.csproj"
    "src/Eaf.Log4NetServiceBus/Eaf.Log4NetServiceBus.csproj"
    "src/Eaf.Middleware.Application/Eaf.Middleware.Application.csproj"
    "src/Eaf.Middleware.AzureActiveDirectory/Eaf.Middleware.AzureActiveDirectory.csproj"
    "src/Eaf.Middleware.Core/Eaf.Middleware.Core.csproj"
    "src/Eaf.Middleware.Ldap/Eaf.Middleware.Ldap.csproj"
    "src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj"
    "src/Eaf.Middleware.Worker/Eaf.Middleware.Worker.csproj"
    "src/Eaf.OpenTelemetry/Eaf.OpenTelemetry.csproj"
    "src/Eaf.SqliteCache/Eaf.SqliteCache.csproj"
    "src/Eaf.SqlServerCache/Eaf.SqlServerCache.csproj"
)
#
failed_builds=()
passed_builds=()
#
for project in "${main_projects[@]}"; do
    echo "▶️  Compilando: $project"
    if dotnet build "$project" --verbosity quiet; then
        echo "✅ Build OK: $project"
        passed_builds+=("$project")
    else
        echo "❌ Build falhou: $project"
        failed_builds+=("$project")
    fi
done
#
echo ""
echo "📊 RESUMO DO BUILD"
echo "=================="
echo "✅ Builds que passaram: ${#passed_builds[@]}"
for build in "${passed_builds[@]}"; do
    echo "   - $build"
done
#
if [[ ${#failed_builds[@]} -gt 0 ]]; then
    echo ""
    echo "❌ Builds que falharam: ${#failed_builds[@]}"
    for build in "${failed_builds[@]}"; do
        echo "   - $build"
    done
fi

echo ""
echo "🧪 Executando testes com cobertura..."
./run-tests-with-coverage.sh
#
echo ""
echo "✅ Build e teste concluídos!"
echo "📊 Relatório de cobertura: TestResults/CoverageReport/index.html"