@echo off
REM dotnet build lots.sln > nul
dotnet run --project lots\lots.csproj -- %*