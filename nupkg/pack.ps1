# Paths
$packFolder = (Get-Item -Path "./packages" -Verbose).FullName
$slnPath = Join-Path $packFolder "../../"
$srcPath = Join-Path $slnPath "src"

# List of projects
$projects = (
    "Eaf",
    "Eaf.AspNetCore",
    "Eaf.AspNetCore.OData",
    "Eaf.AspNetCore.PerRequestRedisCache",
    "Eaf.AspNetCore.SignalR",
    "Eaf.AspNetCore.TestBase",
    "Eaf.AutoMapper",
    "Eaf.Castle.Log4Net",
    "Eaf.Castle.Serilog",
    "Eaf.Dapper",
    "Eaf.EntityFramework.Common",
    "Eaf.EntityFrameworkCore",
	"Eaf.EntityFrameworkCore.EFPlus",
    "Eaf.FluentValidation",
    "Eaf.HangFire.AspNetCore",
    "Eaf.Log4NetServiceBus",
    "Eaf.MailKit",
    "Eaf.MemoryDb",
    "Eaf.Middleware.Application",
    "Eaf.Middleware.AzureActiveDirectory",
    "Eaf.Middleware.Common",
    "Eaf.Middleware.Core",
    "Eaf.Middleware.Ldap",
    "Eaf.Middleware.Web.Core",
    "Eaf.MiddlewareCore",
    "Eaf.MiddlewareCore.EntityFrameworkCore",
    "Eaf.Middleware.Worker",
    "Eaf.OpenTelemetry",
    "Eaf.OracleCache",
    "Eaf.Polly",
    "Eaf.RedisCache",
    "Eaf.RedisCache.ProtoBuf",
    "Eaf.SqliteCache",
    "Eaf.TestBase",
    "Eaf.TextTemplating",
    "Eaf.Web.Common"
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder

    $path = (Join-Path $projectFolder "bin/Release")
    if([System.IO.File]::Exists($path)){
        Remove-Item -Recurse $path
    }

    dotnet msbuild /p:Configuration=Release
    dotnet msbuild /t:pack /p:Configuration=Release /p:ContinuousIntegrationBuild=true
    
    # Copy nuget package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.nupkg")
    $projectSymbolPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.snupkg")
    
    Move-Item $projectPackPath $packFolder -Force
    Move-Item $projectSymbolPath $packFolder -Force
}

# Go back to the pack folder
Set-Location $packFolder