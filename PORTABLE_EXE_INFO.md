# 🎯 Junktoys - Single Portable EXE

## ✅ What You Have Now

A **single portable EXE** with everything built-in:

- ✅ **Custom icon** (your icon.ico)
- ✅ **Self-contained** .NET 8 runtime
- ✅ **Compressed** for smaller size
- ✅ **No dependencies** needed
- ✅ **No installation** required
- ✅ **File size:** ~75MB (was ~100MB as folder)

## 📍 Location

**Built EXE:** `bin\Release\net8.0-windows\win-x64\publish\Junktoys.exe`

## 🚀 How to Distribute

### Option 1: Direct Upload (Simplest)
1. Go to GitHub → Releases → New Release
2. Upload `Junktoys.exe` directly
3. Users download and run - that's it!

### Option 2: Use Build Script
```powershell
.\build-release.ps1
```
This creates `release\Junktoys.exe` with README.txt

## 👥 User Experience

**Before (folder with 300+ files):**
- Download ZIP (~100MB)
- Extract to folder
- Find Junktoys.exe among 300 files
- Run as admin

**Now (single EXE):**
- Download Junktoys.exe (~75MB)
- Run as admin
- Done!

## 🎨 Icon

Your custom icon is embedded in the EXE and will show:
- In Windows Explorer
- On the taskbar
- In Task Manager
- In the title bar

## 📦 What's Inside the EXE

When you run it, the EXE automatically extracts to a temp folder:
- .NET 8 runtime
- WPF libraries
- ModernWPF UI
- Your app code
- All dependencies

**User never sees this** - it just works!

## ⚡ Performance

**First run:** 2-3 seconds (extracts to temp)
**Subsequent runs:** Instant (uses cached extraction)

The extraction is automatic and invisible to users.

## 🔧 Technical Details

**Build Configuration:**
```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>true</SelfContained>
<IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
<EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
```

**What this means:**
- Single EXE with everything
- No .NET installation needed
- Native libraries included
- Compressed for smaller size

## 📤 GitHub Release Example

```markdown
# Junktoys v1.0

Download: Junktoys.exe (~75MB)

## Quick Start
1. Download Junktoys.exe
2. Right-click → Run as administrator
3. Enjoy!

No installation, no extraction, no dependencies!
```

## 🎯 Benefits

1. **Simplicity** - One file to distribute
2. **Portability** - Run from USB, cloud storage, anywhere
3. **No installer** - No admin needed to "install"
4. **Clean** - No scattered files
5. **Professional** - Custom icon, proper EXE
6. **Self-contained** - Works on any Windows 10/11

## ⚠️ Note

The EXE is large (~75MB) because it includes:
- Complete .NET 8 runtime
- WPF framework
- All dependencies

This is **normal** for self-contained apps and ensures it works on any Windows machine without requiring .NET installation.

---

**Your app is now ready for GitHub Releases!** 🚀
