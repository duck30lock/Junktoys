# 🎉 What's New in Junktoys v1.0

## ✅ Fixed Issues

- **Settings Crash**: Fixed the crash when opening Settings page
- **Performance**: Reduced from 177MB to ~2.6MB - loads instantly now!
- **Auto Dark Mode**: Now automatically detects and applies your Windows theme
- **Bloatware Removal**: NOW ACTUALLY WORKS! Permanently removes apps for all users
- **Edge Removal**: FULLY DELETES Edge even after restart with registry blocks

## 🔥 AGGRESSIVE Bloatware Removal

The Bloatware Detector now ACTUALLY removes apps permanently:

### What's Different:
- ✅ **Removes for ALL users** (not just current user)
- ✅ **Blocks reinstallation** (removes provisioned packages)  
- ✅ **Permanent deletion** (not just uninstall)
- ✅ **Dedicated Edge removal** with folder deletion & registry blocks
- ✅ **Takes ownership** of system files to delete them
- ✅ **Prevents Windows Update** from reinstalling

### Edge Removal Features:
- 🔥 Kills all Edge processes
- 🔥 Deletes Edge folders (Program Files, LocalAppData, ProgramData)
- 🔥 Removes Edge AppX packages for all users
- 🔥 Registry block: `DoNotUpdateToEdgeWithChromium`
- 🔥 Persists after restart!

## 🚀 New Advanced Features

Click the **⚡ Advanced** tab in the sidebar for powerful optimizations:

### One-Click Optimization
- **FULL OPTIMIZATION PACK**: Apply all tweaks with one button

### Registry Tweaks
- ✓ Disable Windows Telemetry completely
- ✓ Disable Cortana permanently  
- ✓ Control Windows Update (set to manual)
- ✓ Disable GameDVR/Game Bar (Xbox bloat)
- ✓ Optimize Visual Effects for performance

### Performance Enhancements
- ⚡ Enable Ultimate Performance Power Plan (hidden Windows mode)
- ⚡ Disable SuperFetch/Prefetch
- ⚡ Optimize Network Settings (gaming/low-latency)
- ⚡ Clear ALL Temp Files & Cache
- ⚡ Optimize SSD with TRIM command

### Privacy & Security
- 🔒 Disable Location Tracking
- 🔒 Disable Activity History
- 🔒 Block Microsoft Telemetry IPs (hosts file)
- 🔒 Disable Advertising ID

## 📦 Portable Distribution

Junktoys is now a **portable application** - no installation required!

### How it Works:

- **Self-contained**: Includes .NET runtime (~80-100MB)
- **No dependencies**: Users don't need to install anything
- **Extract & run**: Just unzip and launch
- **GitHub-ready**: Perfect for releases

### Creating a Release Package:

```powershell
.\build-release.ps1
```

This creates `Junktoys-v1.0-win-x64.zip` with everything needed.

## 🎨 UI Improvements

- **Simplified Navigation**: Cleaner sidebar, easier to use
- **⚡ Quick Clean Button**: One-click optimization in sidebar
- **Auto Theme Detection**: Matches Windows light/dark mode automatically
- **Modern Design**: Clean, professional interface

## 📍 How to Run

**For Development:**
- Navigate to: `bin\Release\net8.0-windows\win-x64\publish\`
- Right-click `Junktoys.exe` → Run as administrator

**For Distribution:**
1. Run `.\build-release.ps1` to create release ZIP
2. Upload `Junktoys-v1.0-win-x64.zip` to GitHub Releases
3. Users download, extract, and run - that's it!

## ⚠️ Important Notes

- Always **create a system restore point** before using Advanced features
- Some features require **administrator privileges**
- **Restart may be required** after applying optimizations
- Use Advanced features carefully - they make system-level changes

## 🔥 Most Powerful Features

1. **Full Optimization Pack** - All tweaks in one click
2. **Telemetry Disabling** - Complete privacy from Microsoft tracking
3. **Ultimate Performance Mode** - Hidden Windows power plan
4. **Network Optimization** - Perfect for gaming
5. **⚡ Quick Clean** - Instant memory/temp cleanup

## 📖 Documentation

- `README.md` - Full feature list and build instructions
- `INSTALLER_GUIDE.md` - How to create the installer
- `LICENSE.txt` - MIT License

Enjoy your optimized, cleaner, faster Windows! 🚀
