# 12 — Adicionar Response Compression (Brotli + Gzip)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 4 — Performance API Template |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Middleware aditivo, não altera lógica existente |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 1 arquivo de produção |

## Objetivo

Adicionar response compression (Brotli + Gzip) ao pipeline HTTP da API template para reduzir tamanho de payloads JSON.

## Motivo

- **Payload size**: APIs REST retornam JSON não comprimido — ~3x maior que comprimido
- **Latência**: Reduz tempo de transferência, especialmente para listas grandes
- **Best practice**: Brotli tem ~20% melhor compressão que Gzip para JSON

## Arquivos Afetados

### Produção

**`Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs`**

```csharp
// ── ADICIONAR import ──
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

// ── ADICIONAR em ConfigureServices (após services.AddControllersWithViews) ──
// Response Compression
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json",
        "application/javascript",
        "text/css",
        "text/html",
        "text/json",
        "text/plain",
        "text/xml"
    });
});

services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// ── ADICIONAR em Configure (ANTES de UseRouting) ──
// Response Compression — must be before UseRouting
app.UseResponseCompression();
```

**NOTA sobre posição no pipeline**:
```
UseAbp()
UseResponseCompression()  // ← AQUI: antes de UseRouting
UseRouting()
UseEndpoints()
```

### Teste

## Cenários de Teste

```csharp
// Não é necessário teste unitário — verificação funcional:
// 1. Iniciar API
// 2. Enviar request com header: Accept-Encoding: br, gzip
// 3. Verificar response header: Content-Encoding: br (ou gzip)
```

## Comandos de Verificação

```bash
dotnet build Templates/Api/src/Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj --configuration Release
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. `services.AddResponseCompression()` configurado com Brotli + Gzip
2. `app.UseResponseCompression()` adicionado ANTES de `UseRouting()`
3. `CompressionLevel.Fastest` para ambos (latência > taxa de compressão)
4. Build compila sem erros
5. `EnableForHttps = true` habilitado

## Notas para o Sub-Agent

- Posição no pipeline é CRÍTICA: `UseResponseCompression()` ANTES de `UseRouting()`
- `CompressionLevel.Fastest` é recomendado para APIs — menor latência
- `EnableForHttps = true` é necessário pois maioria das APIs usam HTTPS
- Não adicionar `ResponseCompressionDefaults.MimeTypes` sem `.Concat()` — sobreescreve os defaults
- O package `Microsoft.AspNetCore.ResponseCompression` é incluído no ASP.NET Core metapackage — não precisa adicionar NuGet separado
