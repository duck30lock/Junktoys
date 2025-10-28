# Junktoys - Publish to GitHub Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Publishing Junktoys to GitHub" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if git is installed
$gitVersion = git --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Git not found. Please install Git first." -ForegroundColor Red
    Write-Host "Download from: https://git-scm.com/download/win" -ForegroundColor Yellow
    exit 1
}

Write-Host "Git found: $gitVersion" -ForegroundColor Green
Write-Host ""

# Initialize git if needed
if (-not (Test-Path ".git")) {
    Write-Host "Initializing Git repository..." -ForegroundColor Yellow
    git init
    Write-Host ""
}

# Add all files
Write-Host "Adding files to Git..." -ForegroundColor Yellow
git add .

# Commit
Write-Host "Creating commit..." -ForegroundColor Yellow
git commit -m "Junktoys v1.0 - Ultimate Windows Optimizer with aggressive bloatware removal"

# Add remote
Write-Host "Connecting to GitHub repository..." -ForegroundColor Yellow
git remote remove origin 2>$null
git remote add origin https://github.com/Goenvim/Junktoys.git

# Push
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
Write-Host ""
Write-Host "You may be asked to sign in to GitHub..." -ForegroundColor Cyan
Write-Host ""

git branch -M main
git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  ✅ Code Published to GitHub!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Repository: https://github.com/Goenvim/Junktoys" -ForegroundColor White
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Build release: .\build-release.ps1" -ForegroundColor White
    Write-Host "2. Go to: https://github.com/Goenvim/Junktoys/releases" -ForegroundColor White
    Write-Host "3. Click 'Create a new release'" -ForegroundColor White
    Write-Host "4. Upload: release\Junktoys.exe" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "Push failed. This might be because:" -ForegroundColor Red
    Write-Host "- You need to sign in to GitHub" -ForegroundColor Yellow
    Write-Host "- The repository doesn't exist yet" -ForegroundColor Yellow
    Write-Host "- You don't have permission" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Try running: git push -u origin main" -ForegroundColor Cyan
}
