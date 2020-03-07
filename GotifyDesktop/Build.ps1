$path = Split-Path -parent $MyInvocation.MyCommand.Definition
$folderToDelete = $path + '\bin\Release\netcoreapp3.0'

Remove-Item $folderToDelete -Recurse -Force

Set-Location -Path $path

dotnet publish -f netcoreapp3.0 -r win-x64 -c Release /p:PublishSingleFile=true
dotnet publish -f netcoreapp3.0 -r linux-x64 -c Release
dotnet publish -f netcoreapp3.0 -r osx-x64 -c Release