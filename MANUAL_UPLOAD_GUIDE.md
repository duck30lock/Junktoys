# 🚨 Manual Upload Guide (Account Suspended Issue)

Your **30duck** account is still showing as suspended. Here are your options:

---

## Option 1: Wait & Retry (Recommended)

If you just fixed it via Mac Keychain, it might take **24-48 hours** to fully restore.

### Once Account is Active:
1. Double-click `PUSH_WHEN_READY.bat` in this folder
2. It will automatically push everything to **30duck/Junktoys**

**OR manually run:**
```powershell
git push -u origin main
```

---

## Option 2: Clear Windows Credentials & Retry Now

The suspension might be cached in Windows credentials:

### Steps:
1. **Open Credential Manager:**
   - Press `Win + R`
   - Type: `control /name Microsoft.CredentialManager`
   - Press Enter

2. **Remove GitHub credentials:**
   - Look for any entries containing `github.com`
   - Click each one → **Remove**

3. **Try pushing again:**
   ```powershell
   git push -u origin main
   ```
   
4. **Enter credentials** when prompted:
   - Username: `30duck`
   - Password: Use a **Personal Access Token** (not your password!)

### Get Personal Access Token:
1. Go to: https://github.com/settings/tokens
2. Click **Generate new token (classic)**
3. Give it a name: "Junktoys Upload"
4. Check: `repo` (all repo permissions)
5. Click **Generate token**
6. **COPY THE TOKEN** (you can't see it again!)
7. Use this token as your password when pushing

---

## Option 3: Create Fresh Repo Manually

If git push keeps failing, upload manually:

### Steps:
1. **Go to GitHub:** https://github.com/30duck
2. **Create new repo:** Click `+` → New repository
   - Name: `Junktoys`
   - Public
   - **DON'T** initialize with README
3. **Don't use git push**, instead:
   - Go to the empty repo
   - Click **uploading an existing file**
   - Drag & drop ALL files from `c:\Users\vern\Documents\Junktoys`
   - Commit directly to main

4. **Then create release:**
   - Go to Releases → New release
   - Tag: `v2.0.0`
   - Title: `🔥 Junktoys v2.0.0 - 10 NEW FEATURES!`
   - Upload ZIPs from `releases` folder

---

## Option 4: Use GitHub Desktop (Easiest GUI Method)

### Steps:
1. **Download:** https://desktop.github.com/
2. **Install & Login** with 30duck account
3. **Add existing repo:**
   - File → Add local repository
   - Choose: `c:\Users\vern\Documents\Junktoys`
4. **Push:**
   - Click **Publish repository**
   - Or: Repository → Push

---

## Option 5: Contact GitHub Support

If account issues persist:

1. Go to: https://support.github.com/contact
2. Subject: "Account Suspension - Need Help"
3. Explain you fixed the issue on Mac and need account restored

---

## ✅ Quick Status Check

Run this to check if account is working:

```powershell
git ls-remote https://github.com/30duck/Junktoys.git
```

**If you see:** "Your account is suspended" → Wait longer or try Option 2/3
**If you see:** List of refs → Account is working! Use `PUSH_WHEN_READY.bat`

---

## 📦 Your Files Are Ready

All built releases are in: `releases\`
- `Junktoys-v2.0.0-x64.zip` (69.36 MB)
- `Junktoys-v2.0.0-ARM64.zip` (64.80 MB)

**You can distribute these directly** even without GitHub!

---

## Need Help?

The git repo is configured and ready. Once your account is fully active, just run:
```
PUSH_WHEN_READY.bat
```

Or manually:
```powershell
git push -u origin main
```
