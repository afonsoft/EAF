param (
    [string] $packagesPath
)

.\CredentialProvider.VSS.exe -U https://dev.azure.com/golhub -I

if ([string]::IsNullOrEmpty($packagesPath)) { $packagesPath = ".\packages\*.nupkg" }
$packages = Get-ChildItem  $packagesPath

foreach ($package in $packages) {

	& .\nuget.exe push -Source "EAF" -ApiKey VSTS "$package"
  
}