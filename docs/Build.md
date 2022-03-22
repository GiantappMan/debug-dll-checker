# Pack
```
dotnet pack

```
# Local Test
```
dotnet pack
dotnet tool uninstall -g DDC
dotnet tool install --global --add-source ./packages/ DDC

```
# Upload 
dotnet pack DDC -o ../../LocalNuget/packages -c Release -p:PackageID=DDC