# 🧹 Junktoys - Ultimate Windows Optimizer

A powerful Windows debloating and optimization tool with auto dark mode and advanced features.

## ✨ Features

### Basic Tools
- **⚡ Quick Clean**: One-click optimization (kills heavy apps + clears temp files)
- **Background Apps Manager**: Auto-kill processes using >500MB RAM
- **Bloatware Detector**: Find and remove pre-installed bloatware & Edge
- **Startup Manager**: Control what programs launch at startup
- **Services Optimizer**: Disable unnecessary Windows services

### 🚀 Advanced Optimizations (NEW!)
- **Full Optimization Pack**: Apply all tweaks with one click
- **Disable Telemetry**: Stop Windows data collection completely
- **Disable Cortana**: Remove Cortana permanently
- **Control Windows Update**: Set to manual mode
- **Disable GameDVR/Game Bar**: Remove Xbox bloat
- **Ultimate Performance Power Plan**: Unlock hidden power mode
- **Network Optimization**: Low-latency gaming optimizations
- **SSD Optimization**: TRIM command for better SSD performance
- **Privacy Controls**: Disable location, activity history, advertising ID
- **Visual Effects**: Optimize for maximum performance
- **Telemetry IP Blocking**: Block Microsoft tracking servers

### UI Features
- **Auto Dark Mode**: Automatically matches your Windows theme
- **Modern Interface**: Clean, easy-to-use design
- **Fast Performance**: Optimized build (~2.6MB, loads instantly)

## 📥 Download & Run

### For Users (GitHub Releases)

1. **Download** `Junktoys.exe` from [Releases](https://github.com/goenvim/junktoys/releases)
2. **Right-click** `Junktoys.exe` → **Run as administrator**
3. Enjoy!

**That's it!** Single portable EXE with:
- ✅ Self-contained .NET runtime
- ✅ No installation needed
- ✅ No dependencies
- ✅ ~75MB single file

### Requirements
- Windows 10 version 1809+ or Windows 11
- Administrator privileges (for most features)
- No .NET installation required

## 🔨 Building from Source

### Prerequisites
- .NET 8.0 SDK
- Windows 10/11

### Build for Development
```powershell
dotnet build -c Release
dotnet publish -c Release
```

### Build for Release (GitHub)
```powershell
.\build-release.ps1
```

This creates a single portable `Junktoys.exe` (~75MB) in the `release\` folder.

## Usage

1. Run `WindowsDebloater.exe` (may require administrator privileges)
2. Navigate through different sections using the sidebar
3. Use the scan/refresh buttons to detect issues
4. Select items to remove/disable and confirm actions

## ⚠️ Important Warnings

- **Create a system restore point** before making major changes
- Some features require **administrator privileges**
- Removing system apps may cause **instability**
- Disabling critical services can **break functionality**

## System Requirements

- Windows 10 version 1809 or later
- Windows 11 (recommended for best UI experience)
- Administrator rights for some features

## License

This software is provided as-is without warranty. Use at your own risk.
