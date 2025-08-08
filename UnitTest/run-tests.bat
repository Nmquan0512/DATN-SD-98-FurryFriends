@echo off
echo Running Unit Tests for Color Management...
echo.

echo Building solution...
dotnet build

echo.
echo Running all tests...
dotnet test --verbosity normal

echo.
echo Running specific test cases...
dotnet test --filter "MS001_AddValidColor_ShouldReturnCreated"
dotnet test --filter "MS003_AddColorWithEmptyName_ShouldReturnBadRequest"
dotnet test --filter "MS005_AddColorWithInvalidColorCode_ShouldReturnBadRequest"

echo.
echo Test execution completed!
pause 