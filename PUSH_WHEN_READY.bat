@echo off
echo ========================================
echo  Junktoys Git Push Script
echo ========================================
echo.
echo This will push to: https://github.com/30duck/Junktoys
echo.
echo Make sure your GitHub account is active!
echo.
pause

cd /d "%~dp0"

echo.
echo Pushing to GitHub...
git push -u origin main

echo.
if %ERRORLEVEL% EQU 0 (
    echo ========================================
    echo  SUCCESS! Code pushed to GitHub!
    echo ========================================
    echo.
    echo Next: Create a release at https://github.com/30duck/Junktoys/releases/new
    echo.
) else (
    echo ========================================
    echo  PUSH FAILED!
    echo ========================================
    echo.
    echo Possible issues:
    echo - Account still suspended
    echo - Need to login again
    echo - Wrong credentials
    echo.
    echo Try: git push -u origin main
    echo.
)

pause
