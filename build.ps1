# Windows Debloater Build Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Junktoys Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Checking for .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET SDK not found. Please install .NET 8.0 SDK or later." -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}
Write-Host "Found .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Restore dependencies
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages." -ForegroundColor Red
    exit 1
}
Write-Host "Packages restored successfully." -ForegroundColor Green
Write-Host ""

# Build in Release mode
Write-Host "Building project in Release mode..." -ForegroundColor Yellow
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed." -ForegroundColor Red
    exit 1
}
Write-Host "Build completed successfully." -ForegroundColor Green
Write-Host ""

# Publish as single-file executable
Write-Host "Publishing as standalone executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Publish failed." -ForegroundColor Red
    exit 1
}
Write-Host "Publish completed successfully." -ForegroundColor Green
Write-Host ""

# Show output location
$outputPath = "bin\Release\net8.0-windows\win-x64\publish\Junktoys.exe"
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Executable location:" -ForegroundColor Yellow
Write-Host $outputPath -ForegroundColor White
Write-Host ""

# Check if file exists and show size
if (Test-Path $outputPath) {
    $fileSize = (Get-Item $outputPath).Length / 1MB
    Write-Host "File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
    Write-Host ""
    
    # Offer to open the folder
    $open = Read-Host "Open output folder? (Y/N)"
    if ($open -eq "Y" -or $open -eq "y") {
        Start-Process "explorer.exe" -ArgumentList "/select,`"$((Get-Item $outputPath).FullName)`""
    }
} else {
    Write-Host "Warning: Output file not found at expected location." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "You can now run the application with administrator privileges." -ForegroundColor Green
