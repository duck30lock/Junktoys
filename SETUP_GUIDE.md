# Setup Guide - Windows Debloater

## Step 1: Install .NET 8.0 SDK

Before building, you need the .NET 8.0 SDK:

1. Download from: **https://dotnet.microsoft.com/download/dotnet/8.0**
2. Run the installer (choose SDK, not Runtime)
3. Restart your terminal/PowerShell after installation
4. Verify installation: `dotnet --version`

## Step 2: Build the Application

### Option A: Using the Build Script (Recommended)
```powershell
.\build.ps1
```

### Option B: Manual Build Commands
```powershell
# Restore packages
dotnet restore

# Build the project
dotnet build -c Release

# Create standalone EXE
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Step 3: Run the Application

The compiled executable will be at:
```
bin\Release\net8.0-windows\win-x64\publish\WindowsDebloater.exe
```

**Right-click** the EXE and select **"Run as administrator"** for full functionality.

## Features Available

✅ **Background Apps Manager** - Kill memory-hogging processes
✅ **Bloatware Detector** - Remove pre-installed apps including Edge
✅ **Startup Manager** - Disable auto-start programs
✅ **Services Optimizer** - Turn off unnecessary Windows services
✅ **Modern UI** - PowerToys-inspired Windows 11 design

## Troubleshooting

### "dotnet not recognized"
- Install .NET 8.0 SDK from the link above
- Restart your PowerShell/terminal
- Make sure SDK (not just Runtime) is installed

### "Access Denied" errors
- Run the app as Administrator
- Some features require elevated privileges

### App won't start
- Make sure you're on Windows 10 1809+ or Windows 11
- Install any pending Windows updates
- Check Windows Defender hasn't quarantined the file

## Safety Tips

⚠️ **Always create a system restore point first**
⚠️ Be careful when removing system apps
⚠️ Don't disable services you don't understand
⚠️ Test changes before making them permanent

## Next Steps

After building successfully:
1. Create a system restore point
2. Run WindowsDebloater.exe as administrator
3. Start with the Background Apps tab
4. Review items before removing/disabling them
5. Use the Settings page to configure preferences

Enjoy your cleaner, faster Windows! 🚀
