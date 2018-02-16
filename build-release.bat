cls

rmdir /Q /S %CD%\release

dotnet clean
dotnet publish -c \"Release\" Elastic.Search.Web.sln -v "m" --no-restore --output "%CD%\release"