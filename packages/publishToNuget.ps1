$packages = Get-ChildItem $PSScriptRoot\dist\*.nupkg -Exclude *symbols*
$packages | ForEach-Object {dotnet nuget push $_.FullName -k oy2lcktkoj34iiwb762awvosgdcadhie5azl3yid4d7bz4 -s https://api.nuget.org/v3/index.json}

$exist = Read-Host("按任意键删除所有包");
del dist\*.nupkg
del dist\*.snupkg