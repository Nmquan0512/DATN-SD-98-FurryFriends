Write-Host "Running Unit Tests for Color Management..." -ForegroundColor Green
Write-Host ""

Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build

Write-Host ""
Write-Host "Running all tests..." -ForegroundColor Yellow
dotnet test --verbosity normal

Write-Host ""
Write-Host "Running specific test cases..." -ForegroundColor Yellow
dotnet test --filter "MS001_AddValidColor_ShouldReturnCreated"
dotnet test --filter "MS003_AddColorWithEmptyName_ShouldReturnBadRequest"
dotnet test --filter "MS005_AddColorWithInvalidColorCode_ShouldReturnBadRequest"

Write-Host ""
Write-Host "Test execution completed!" -ForegroundColor Green 