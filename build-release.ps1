# Junktoys Release Build Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Junktoys Release Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get version from project file
$projectFile = "WindowsDebloater.csproj"
$version = "1.0"
if (Test-Path $projectFile) {
    $xml = [xml](Get-Content $projectFile)
    $version = $xml.Project.PropertyGroup.Version
    if ([string]::IsNullOrEmpty($version)) {
        $version = "1.0.0"
    }
}

Write-Host "Building Junktoys v$version" -ForegroundColor Yellow
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "bin") {
    Remove-Item "bin" -Recurse -Force
}
if (Test-Path "obj") {
    Remove-Item "obj" -Recurse -Force
}

# Build
Write-Host "Building single-file portable application..." -ForegroundColor Yellow
& "C:\Program Files\dotnet\dotnet.exe" publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

# Create release folder
$publishPath = "bin\Release\net8.0-windows\win-x64\publish"
$releasePath = "release"
$exeName = "Junktoys.exe"

Write-Host "Creating release package..." -ForegroundColor Yellow

if (Test-Path $releasePath) {
    Remove-Item $releasePath -Recurse -Force
}
New-Item -ItemType Directory -Path $releasePath | Out-Null

# Copy only the EXE (single-file portable)
Write-Host "Copying portable EXE..." -ForegroundColor Yellow
Copy-Item "$publishPath\$exeName" -Destination $releasePath

# Create README for release
$releaseReadme = @"
# Junktoys v$version - Windows Optimizer

## 🚀 Quick Start

1. **Run** ``Junktoys.exe`` as administrator
2. Use **⚡ Quick Clean** button for instant optimization
3. Explore **⚡ Advanced** tab for powerful tweaks

## ⚠️ Important

- Always create a system restore point before using Advanced features
- Requires administrator privileges for most features
- Restart may be required after optimizations

## ✨ Features

- ⚡ Quick Clean (one-click optimization)
- Background Apps Manager
- Bloatware Detector
- Startup Manager
- Services Optimizer
- **Advanced Optimizations** (telemetry, performance, privacy)
- Auto Dark Mode (matches Windows theme)

## 📖 Documentation

Full documentation: https://github.com/yourusername/junktoys

Enjoy your optimized Windows! 🚀
"@

$releaseReadme | Out-File -FilePath "$releasePath\README.txt" -Encoding UTF8

# Get EXE size
$exeSize = (Get-Item "$releasePath\$exeName").Length / 1MB

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Portable EXE Created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "File: $exeName" -ForegroundColor White
Write-Host "Size: $([math]::Round($exeSize, 2)) MB" -ForegroundColor Cyan
Write-Host "Location: $(Get-Location)\$releasePath\$exeName" -ForegroundColor White
Write-Host ""
Write-Host "✅ Single portable EXE with icon!" -ForegroundColor Green
Write-Host "✅ No dependencies needed!" -ForegroundColor Green
Write-Host "✅ Ready to upload to GitHub Releases!" -ForegroundColor Green
Write-Host ""

# Open folder
$openFolder = Read-Host "Open release folder? (Y/N)"
if ($openFolder -eq "Y" -or $openFolder -eq "y") {
    Start-Process "explorer.exe" -ArgumentList "/select,`"$((Get-Item "$releasePath\$exeName").FullName)`""
}
