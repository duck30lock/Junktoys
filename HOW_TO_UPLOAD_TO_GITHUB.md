# 📤 How to Upload Junktoys to GitHub

Since both your GitHub accounts are suspended, here's how to upload once you create a new account:

## Option 1: Push via Git (Recommended)

### Step 1: Create New GitHub Account
1. Go to https://github.com/signup
2. Create a new account (use a different email)
3. Verify your email

### Step 2: Create New Repository
1. Click the **+** icon → **New repository**
2. Repository name: `Junktoys`
3. Description: "Ultimate Windows Optimizer with 10+ Advanced Features"
4. Set to **Public** (or Private if you prefer)
5. **DO NOT** initialize with README (we already have one)
6. Click **Create repository**

### Step 3: Update Git Remote & Push
Open PowerShell in `c:\Users\vern\Documents\Junktoys` and run:

```powershell
# Replace YOUR_NEW_USERNAME with your actual GitHub username
git remote set-url origin https://github.com/YOUR_NEW_USERNAME/Junktoys.git

# Push all code
git push -u origin main --force
```

### Step 4: Create GitHub Release
1. Go to your repo → **Releases** → **Create a new release**
2. Tag version: `v2.0.0`
3. Release title: `🔥 Junktoys v2.0.0 - 10 NEW FEATURES!`
4. Copy the contents from `RELEASE_NOTES_v2.0.0.md` into the description
5. Upload these files from `releases` folder:
   - `Junktoys-v2.0.0-x64.zip`
   - `Junktoys-v2.0.0-ARM64.zip`
6. Click **Publish release**

---

## Option 2: Manual Upload (If Git Doesn't Work)

### Step 1: Create ZIP of Source Code
Right-click the `Junktoys` folder → Send to → Compressed (zipped) folder

### Step 2: Upload to GitHub
1. Create new repo on GitHub (as above)
2. Click **uploading an existing file**
3. Drag and drop all files (or the ZIP)
4. Commit changes

### Step 3: Create Release
Same as Option 1, Step 4

---

## Files to Upload for Release

Located in: `c:\Users\vern\Documents\Junktoys\releases\`

### x64 Version (for most PCs)
- **Folder:** `Junktoys-v2.0.0-x64`
- **ZIP:** `Junktoys-v2.0.0-x64.zip` (69.36 MB)
- **Contains:** Single `Junktoys.exe` (74.83 MB) + README.txt

### ARM64 Version (for ARM PCs)
- **Folder:** `Junktoys-v2.0.0-ARM64`
- **ZIP:** `Junktoys-v2.0.0-ARM64.zip` (64.80 MB)
- **Contains:** Single `Junktoys.exe` (70.68 MB) + README.txt

---

## What You've Built

### Total Features: 16
**10 NEW Features:**
1. 🎮 Game Mode Booster
2. 💾 RAM Turbo
3. ❄️ Process Freezer
4. 🌐 Network Tools Suite
5. 📊 System Monitor Dashboard
6. 🤖 Auto-Pilot Mode
7. 🧹 Registry Turbo Cleaner
8. 💼 Custom Profiles
9. 📁 Disk Space Analyzer
10. ⚡ System Tweaker Pro

**6 Original Features:**
- Quick Clean
- Background Apps Manager
- Bloatware Detector
- Startup Manager
- Services Optimizer
- Advanced Optimizations

### Code Stats
- **25 files** changed
- **4,187+ lines** of code added
- **20 new XAML pages** created
- Multi-platform support (x64 + ARM64)
- Single-file portable EXE
- No dependencies required

---

## Quick Test Before Upload

To verify everything works:

1. Go to `releases\Junktoys-v2.0.0-x64\`
2. Right-click `Junktoys.exe` → Run as administrator
3. Test a few features
4. If it works, you're good to upload!

---

## Troubleshooting

### "Access Denied" when pushing
- Make sure you're logged into the correct GitHub account
- Use a Personal Access Token instead of password:
  1. GitHub → Settings → Developer settings → Personal access tokens
  2. Generate new token (classic)
  3. Use token as password when pushing

### "Repository not found"
- Double-check the repo name and username in the remote URL
- Make sure the repo exists on GitHub

### Build from source later
To rebuild from source anytime:
```powershell
cd c:\Users\vern\Documents\Junktoys
powershell -ExecutionPolicy Bypass -File build-all-releases.ps1
```

---

## 🎉 You're All Set!

Your project is complete and ready to share with the world! 🚀
