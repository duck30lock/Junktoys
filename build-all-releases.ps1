# Junktoys Multi-Platform Release Build Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Junktoys Multi-Platform Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$version = "2.0.0"
$releasesFolder = "releases"

Write-Host "Building Junktoys v$version for x64 and ARM64" -ForegroundColor Yellow
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "bin") { Remove-Item "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item "obj" -Recurse -Force }
if (Test-Path $releasesFolder) { Remove-Item $releasesFolder -Recurse -Force }
New-Item -ItemType Directory -Path $releasesFolder | Out-Null

# Build x64
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building x64 version..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "x64 build failed!" -ForegroundColor Red
    exit 1
}

# Build ARM64
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building ARM64 version..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

dotnet publish -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "ARM64 build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Both builds successful!" -ForegroundColor Green
Write-Host ""

# Create release packages
Write-Host "Creating release packages..." -ForegroundColor Yellow

# x64 Package
$x64Folder = "$releasesFolder\Junktoys-v$version-x64"
New-Item -ItemType Directory -Path $x64Folder | Out-Null
Copy-Item "bin\Release\net8.0-windows\win-x64\publish\WindowsDebloater.exe" -Destination "$x64Folder\Junktoys.exe"

# ARM64 Package
$arm64Folder = "$releasesFolder\Junktoys-v$version-ARM64"
New-Item -ItemType Directory -Path $arm64Folder | Out-Null
Copy-Item "bin\Release\net8.0-windows\win-arm64\publish\WindowsDebloater.exe" -Destination "$arm64Folder\Junktoys.exe"

# Create README for releases
$releaseReadme = @"
# Junktoys v$version - Ultimate Windows Optimizer

🔥 **MASSIVE UPDATE: 10 NEW CRAZY FEATURES!** 🔥

## 🚀 Quick Start

1. **Run** Junktoys.exe as administrator
2. Use **⚡ Quick Clean** for instant optimization
3. Explore the new features in the sidebar!

## ✨ NEW Features

### 🎮 Game Mode Booster
- One-click gaming optimization
- Kill background apps automatically
- High CPU priority for games
- Network & GPU optimization

### 💾 RAM Turbo
- Real-time RAM monitoring with graphs
- Auto-clear at threshold
- Memory compression
- Top consumers list

### ❄️ Process Freezer
- Suspend/resume processes
- Save resources without closing apps
- Batch freeze/resume
- Memory tracking

### 🌐 Network Tools Suite
- Quick DNS switcher (Cloudflare, Google, Quad9)
- Real-time speed monitor
- Ping test utility
- Gaming optimizations

### 📊 System Monitor Dashboard
- CPU/GPU/RAM/Disk monitoring
- Temperature monitoring
- System information
- Live performance tracking

### 🤖 Auto-Pilot Mode
- Scheduled daily/weekly optimizations
- Smart resource management
- Auto-kill on low battery
- Auto-clear RAM at high usage

### 🧹 Registry Turbo Cleaner
- Deep registry scan
- Remove invalid entries
- Auto backup creation
- Registry compaction

### 💼 Custom Profiles
- Gaming, Work, Battery Saver presets
- Save custom profiles
- Quick switching
- Export/Import

### 📁 Disk Space Analyzer
- Visual storage analysis
- Largest folders detection
- Quick clean options
- Drive statistics

### ⚡ System Tweaker Pro
- Advanced Windows tweaks
- Hidden settings unlocked
- Performance optimizations
- Privacy enhancements

## 📋 Original Features

- Background Apps Manager
- Bloatware Detector & Remover
- Startup Manager
- Services Optimizer
- Advanced Optimizations Pack
- Auto Dark Mode

## ⚠️ Important

- **Create a system restore point** before major changes
- Requires **administrator privileges** for most features
- Restart may be required after some optimizations

## 💻 System Requirements

- Windows 10 version 1809+ or Windows 11
- .NET 8.0 Runtime (self-contained - no installation needed!)
- Administrator rights

## 🆘 Support

Report issues: https://github.com/duck30lock/Junktoys/issues

Enjoy your supercharged Windows! 🚀
"@

$releaseReadme | Out-File -FilePath "$x64Folder\README.txt" -Encoding UTF8
$releaseReadme | Out-File -FilePath "$arm64Folder\README.txt" -Encoding UTF8

# Create ZIPs
Write-Host "Creating ZIP archives..." -ForegroundColor Yellow

Compress-Archive -Path "$x64Folder\*" -DestinationPath "$releasesFolder\Junktoys-v$version-x64.zip" -Force
Compress-Archive -Path "$arm64Folder\*" -DestinationPath "$releasesFolder\Junktoys-v$version-ARM64.zip" -Force

# Get file sizes
$x64Size = (Get-Item "$x64Folder\Junktoys.exe").Length / 1MB
$arm64Size = (Get-Item "$arm64Folder\Junktoys.exe").Length / 1MB
$x64ZipSize = (Get-Item "$releasesFolder\Junktoys-v$version-x64.zip").Length / 1MB
$arm64ZipSize = (Get-Item "$releasesFolder\Junktoys-v$version-ARM64.zip").Length / 1MB

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📦 x64 Release:" -ForegroundColor Yellow
Write-Host "   EXE Size: $([math]::Round($x64Size, 2)) MB" -ForegroundColor White
Write-Host "   ZIP Size: $([math]::Round($x64ZipSize, 2)) MB" -ForegroundColor White
Write-Host ""
Write-Host "📦 ARM64 Release:" -ForegroundColor Yellow
Write-Host "   EXE Size: $([math]::Round($arm64Size, 2)) MB" -ForegroundColor White
Write-Host "   ZIP Size: $([math]::Round($arm64ZipSize, 2)) MB" -ForegroundColor White
Write-Host ""
Write-Host "Location: $(Get-Location)\$releasesFolder" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Portable single-file EXEs created!" -ForegroundColor Green
Write-Host "✅ No dependencies needed!" -ForegroundColor Green
Write-Host "✅ Ready for distribution!" -ForegroundColor Green
Write-Host ""

# Open folder
Start-Process "explorer.exe" -ArgumentList "/select,`"$((Get-Item "$releasesFolder").FullName)`""
