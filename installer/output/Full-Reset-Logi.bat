@echo off
echo ============================================================
echo  FULL Logi Options+ and ColorBridge Nuclear Reset
echo  This will COMPLETELY remove everything and start fresh.
echo ============================================================
echo.
echo  WARNING: This will:
echo    - Kill all Logitech processes
echo    - Stop the Logi updater service
echo    - Remove ALL ColorBridge plugin files
echo    - Remove the DemoPlugin registration
echo    - Delete ALL Logi Options+ cache, settings, and data
echo    - Uninstall Logi Options+ from your system
echo.
echo  You will need to reinstall Logi Options+ from scratch after.
echo.
echo  Press CTRL+C to cancel, or...
pause
echo.

echo ============================================================
echo  STEP 1: Killing ALL Logitech processes...
echo ============================================================
taskkill /F /IM "logioptionsplus.exe" /T >nul 2>&1
taskkill /F /IM "logioptionsplus_agent.exe" /T >nul 2>&1
taskkill /F /IM "logioptionsplus_appbroker.exe" /T >nul 2>&1
taskkill /F /IM "logioptionsplus_updater.exe" /T >nul 2>&1
taskkill /F /IM "LogiPluginService.exe" /T >nul 2>&1
taskkill /F /IM "LogiPluginServiceExt.exe" /T >nul 2>&1
timeout /t 2 >nul
echo  Done.
echo.

echo ============================================================
echo  STEP 2: Stopping Logi Options+ Windows Service...
echo ============================================================
net stop "OptionsPlusUpdaterService" >nul 2>&1
sc config "OptionsPlusUpdaterService" start=disabled >nul 2>&1
echo  Done.
echo.

echo ============================================================
echo  STEP 3: Removing ColorBridge plugin completely...
echo ============================================================
del /q "%LOCALAPPDATA%\Logi\LogiPluginService\Plugins\ColorBridge.link" >nul 2>&1
del /q "%LOCALAPPDATA%\Logi\LogiPluginService\Plugins\DemoPlugin.link" >nul 2>&1
rmdir /s /q "%LOCALAPPDATA%\ColorBridge" >nul 2>&1
echo  Done.
echo.

echo ============================================================
echo  STEP 4: Nuking ALL Logi Options+ local data and cache...
echo ============================================================
rmdir /s /q "%LOCALAPPDATA%\Logi" >nul 2>&1
rmdir /s /q "%LOCALAPPDATA%\LogiOptionsPlus" >nul 2>&1
rmdir /s /q "%LOCALAPPDATA%\Logitech" >nul 2>&1
rmdir /s /q "%APPDATA%\Logi" >nul 2>&1
rmdir /s /q "%APPDATA%\Logitech" >nul 2>&1
rmdir /s /q "%APPDATA%\LogiOptionsPlus" >nul 2>&1
rmdir /s /q "%LOCALAPPDATA%\Temp\Logi" >nul 2>&1
echo  Done.
echo.

echo ============================================================
echo  STEP 5: Uninstalling Logi Options+ from Windows...
echo ============================================================
echo  Searching for Logi Options+ uninstaller...

:: Try the standard uninstall paths
if exist "%ProgramFiles%\LogiOptionsPlus\unins000.exe" (
    echo  Found uninstaller. Running...
    "%ProgramFiles%\LogiOptionsPlus\unins000.exe" /SILENT
    timeout /t 5 >nul
) else if exist "%ProgramFiles(x86)%\LogiOptionsPlus\unins000.exe" (
    echo  Found uninstaller (x86^). Running...
    "%ProgramFiles(x86)%\LogiOptionsPlus\unins000.exe" /SILENT
    timeout /t 5 >nul
) else (
    echo  No automatic uninstaller found.
    echo  Please manually uninstall from:
    echo    Settings ^> Apps ^> Installed Apps ^> Logi Options+ ^> Uninstall
    echo.
    echo  Waiting for you to do that now...
    pause
)
echo  Done.
echo.

echo ============================================================
echo  STEP 6: Final cleanup of leftover program files...
echo ============================================================
rmdir /s /q "%ProgramFiles%\LogiOptionsPlus" >nul 2>&1
rmdir /s /q "%ProgramFiles(x86)%\LogiOptionsPlus" >nul 2>&1
rmdir /s /q "%ProgramData%\Logishrd" >nul 2>&1
echo  Done.
echo.

echo ============================================================
echo  COMPLETE! Your system is fully cleaned.
echo ============================================================
echo.
echo  Next steps:
echo    1. RESTART your computer
echo    2. Download fresh Logi Options+ from:
echo       https://www.logitech.com/software/logi-options-plus.html
echo    3. Install it
echo    4. Open Logi Options+ and let it detect your console(s)
echo    5. Run the ColorBridge installer (ColorBridge-Setup.exe)
echo       or use Install-ColorBridge.bat
echo    6. Restart Logi Options+ one more time
echo.
echo  Press any key to exit...
pause >nul
