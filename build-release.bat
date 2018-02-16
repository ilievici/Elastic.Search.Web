cls
echo off

rmdir /Q /S release

dotnet clean
dotnet publish -c \"Release\" Elastic.Search.Web.sln -v "m" --no-restore --output "%CD%\release"