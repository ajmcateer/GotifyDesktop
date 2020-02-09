dotnet publish -f netcoreapp3.0 -r win-x64 -c Release /p:PublishSingleFile=true
dotnet publish -f netcoreapp3.0 -r linux-x64 -c Release /p:PublishSingleFile=true
dotnet publish -f netcoreapp3.0 -r osx-x64 -c Release /p:PublishSingleFile=true