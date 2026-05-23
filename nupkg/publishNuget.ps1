param (
	[string] $NUGET_TOKEN,
	[string] $GITHUB_TOKEN,
    [string] $packagesPath
)

if ($packagesPath -eq $null -or $packagesPath -eq "") { $packagesPath = ".\packages\*.nupkg" }
$packages = Get-ChildItem  $packagesPath

foreach ($package in $packages) {
	if ($GITHUB_TOKEN -ne $null -and $GITHUB_TOKEN -ne "") {
		& dotnet.exe nuget push "$package" --api-key  $GITHUB_TOKEN --source "https://nuget.pkg.github.com/afonsoft/index.json" --skip-duplicate
	}
	if ($NUGET_TOKEN -ne $null -and $NUGET_TOKEN -ne "") {
		& dotnet.exe nuget push  "$package" --api-key $NUGET_TOKEN --source "nuget.org"  --skip-duplicate
	}
}