#!/bin/bash
#
# Script para executar todos os testes com cobertura de código
# EAF Framework - .NET 10.0
#
echo "🧪 Executando testes com cobertura de código para EAF Framework"
echo "================================================================"
#
# Limpar resultados anteriores
echo "🧹 Limpando resultados anteriores..."
rm -rf TestResults/
#
# Executar testes individuais com cobertura
echo "🔬 Executando testes individuais..."
#
# Testes que funcionam corretamente
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
#
failed_tests=()
passed_tests=()
#
for project in "${test_projects[@]}"; do
    echo "▶️  Executando: $project"
    if dotnet test "$project" --collect:"XPlat Code Coverage" --settings coverlet.runsettings --logger trx --results-directory ./TestResults --verbosity quiet; then
        echo "✅ Sucesso: $project"
        passed_tests+=("$project")
    else
        echo "❌ Falhou: $project"
        failed_tests+=("$project")
    fi
    echo ""
done
#
# Gerar relatório consolidado
echo "📊 Gerando relatório consolidado de cobertura..."
export PATH="$PATH:$HOME/.dotnet/tools"
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;Badges;TextSummary;JsonSummary"
# Exibir resumo
echo ""
echo "📈 RESUMO DOS TESTES"
echo "==================="
echo "✅ Testes que passaram: ${#passed_tests[@]}"
for test in "${passed_tests[@]}"; do
    echo "   - $test"
done
#
if [[ ${#failed_tests[@]} -gt 0 ]]; then
    echo ""
    echo "❌ Testes que falharam: ${#failed_tests[@]}"
    for test in "${failed_tests[@]}"; do
        echo "   - $test"
    done
fi
#
echo ""
echo "📊 COBERTURA DE CÓDIGO"
echo "====================="
cat TestResults/CoverageReport/Summary.txt
#
echo ""
echo "🌐 Relatório HTML disponível em: TestResults/CoverageReport/index.html"
echo "📋 Resumo completo em: TestResults/CoverageReport/Summary.txt"
#
# Verificar se atingiu o threshold mínimo
coverage_line=$(grep "Line coverage:" TestResults/CoverageReport/Summary.txt | awk '{print $3}' | sed 's/%//')
if (( $(echo "$coverage_line >= 30" | bc -l) )); then
    echo "✅ Cobertura de linha ($coverage_line%) atende ao mínimo esperado (30%)"
    exit 0
else
    echo "⚠️  Cobertura de linha ($coverage_line%) abaixo do mínimo esperado (30%)"
    exit 1
fi
