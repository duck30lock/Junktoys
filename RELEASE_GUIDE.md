# 📦 Creating GitHub Releases for Junktoys

## Quick Build for Release

Run this command to create a release-ready portable package:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```

This creates a self-contained folder at:
```
bin\Release\net8.0-windows\win-x64\publish\
```

## Creating a Release Package

### Option 1: Automated (Recommended)

Run the build script:
```powershell
.\build-release.ps1
```

This will:
- Build the app
- Create a `Junktoys-v1.0-win-x64.zip` file
- Ready to upload to GitHub Releases

### Option 2: Manual

1. **Build the app:**
   ```powershell
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

2. **Create ZIP file:**
   - Navigate to `bin\Release\net8.0-windows\win-x64\publish\`
   - Select all files
   - Right-click → Send to → Compressed (zipped) folder
   - Rename to `Junktoys-v1.0-win-x64.zip`

3. **Upload to GitHub:**
   - Go to your GitHub repo
   - Click "Releases" → "Create a new release"
   - Tag: `v1.0`
   - Title: `Junktoys v1.0 - Windows Optimizer`
   - Attach the ZIP file
   - Publish release

## What's Included

The release ZIP contains:
- ✅ `Junktoys.exe` - Main application
- ✅ All .NET runtime files (self-contained)
- ✅ ModernWPF UI libraries
- ✅ Dependencies

**No installation required!** Users just:
1. Download the ZIP
2. Extract anywhere
3. Run `Junktoys.exe` as administrator

## File Size

The complete package is approximately **~80-100MB** because it includes:
- .NET 8 runtime (self-contained, no install needed)
- All dependencies
- Multiple language packs

This ensures users don't need to install .NET separately!

## User Instructions

Include this in your release notes:

```markdown
## 📥 How to Use

1. **Download** `Junktoys-v1.0-win-x64.zip`
2. **Extract** to any folder (e.g., Desktop, Downloads)
3. **Right-click** `Junktoys.exe` → **Run as administrator**
4. Enjoy your optimized Windows!

### Requirements
- Windows 10 version 1809 or later
- Windows 11 (recommended)
- No .NET installation needed (self-contained)

### ⚠️ Important
- Always create a system restore point before using Advanced features
- Some features require administrator privileges
- Restart may be required after optimizations
```

## Version Updates

When creating new versions:

1. Update version in `WindowsDebloater.csproj`:
   ```xml
   <Version>1.1.0</Version>
   ```

2. Update `WHATS_NEW.md` with changes

3. Rebuild and create new ZIP:
   ```powershell
   .\build-release.ps1
   ```

4. Create new GitHub release with updated version

## Tips

- Use semantic versioning (1.0.0, 1.1.0, 2.0.0)
- Include changelog in release notes
- Test the ZIP on a clean machine before publishing
- Consider creating a separate "lite" version without language packs to reduce size

That's it! Your app is now ready for GitHub Releases. 🚀
