param (
    [string] $packagesPath
)

.\CredentialProvider.VSS.exe -U https://dev.azure.com/golhub -I

if ($packagesPath -eq $null -or $packagesPath -eq "") { $packagesPath = ".\packages\*.nupkg" }
$packages = Get-ChildItem  $packagesPath

foreach ($package in $packages) {

	& .\nuget.exe push -Source "EAF" -ApiKey VSTS "$package"
  
}