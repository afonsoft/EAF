param (
	[string] $NUGET_TOKEN,
	[string] $GITHUB_TOKEN,
    [string] $packagesPath
)

if ([string]::IsNullOrEmpty($packagesPath)) { $packagesPath = ".\packages\*.nupkg" }
$packages = Get-ChildItem  $packagesPath

foreach ($package in $packages) {
	if (-not [string]::IsNullOrEmpty($GITHUB_TOKEN)) {
		& dotnet.exe nuget push "$package" --api-key  $GITHUB_TOKEN --source "https://nuget.pkg.github.com/afonsoft/index.json" --skip-duplicate
	}
	if (-not [string]::IsNullOrEmpty($NUGET_TOKEN)) {
		& dotnet.exe nuget push  "$package" --api-key $NUGET_TOKEN --source "nuget.org"  --skip-duplicate
	}
}