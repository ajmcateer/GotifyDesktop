$path = Split-Path -parent $MyInvocation.MyCommand.Definition
$releaseFolder = $path + '\bin\Release\netcoreapp3.1'

$linuxPath = $path + '\bin\Release\netcoreapp3.1\linux-x64\publish'
$osxPath = $path + '\bin\Release\netcoreapp3.1\osx-x64\publish'
$winPath = $path + '\bin\Release\netcoreapp3.1\win-x64\publish'

$linuxZipPath = $linuxPath + '\GotifyDesktop-Linux.zip'
$osxZipPath = $osxPath + '\GotifyDesktop-Mac.zip'
$winZipPath = $winPath + '\GotifyDesktop-Win.zip'

Remove-Item $releaseFolder -Recurse -Force

Set-Location -Path $path

dotnet publish -f netcoreapp3.1 -r win-x64 -c Release /p:PublishSingleFile=true
dotnet publish -f netcoreapp3.1 -r linux-x64 -c Release
dotnet publish -f netcoreapp3.1 -r osx-x64 -c Release
dotnet publish -f netcoreapp3.1 -r linux-arm64 -c Release

#if (!(Get-Module "Microsoft.PowerShell.Archive")) {
#    Write-Output "Microsoft.PowerShell.Archive not installed please install and rerun the script"
#    Exit 
#}

$module = Get-InstalledModule "Microsoft.PowerShell.Archive"
if($module.Version.Major -eq 1 -And $module.Version.Minor -eq 2 -And $module.Version.Build -lt 3){
    "Please upgrade Microsoft.PowerShell.Archive to 1.2.3 or higher"
    "You can use this command Install-Module -Name Microsoft.PowerShell.Archive -force"
    Exit 
}

Get-ChildItem $linuxPath *.pdb -Recurse | foreach { Remove-Item -Path $_.FullName }
Get-ChildItem $osxPath *.pdb -Recurse | foreach { Remove-Item -Path $_.FullName }
Get-ChildItem $winPath *.pdb -Recurse | foreach { Remove-Item -Path $_.FullName }

Compress-Archive -Path $linuxPath -DestinationPath $linuxZipPath
Compress-Archive -Path $winPath -DestinationPath $osxZipPath
Compress-Archive -Path $osxPath -DestinationPath $winZipPath

Move-Item -Path $linuxZipPath -Destination ($releaseFolder + '\GotifyDesktop-Linux.zip')
Move-Item -Path $osxZipPath -Destination ($releaseFolder + '\GotifyDesktop-Mac.zip')
Move-Item -Path $winZipPath -Destination ($releaseFolder + '\GotifyDesktop-Win.zip')