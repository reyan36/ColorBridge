@echo off
echo =======================================
echo  ColorBridge Manual Installer (No Admin Need)
echo =======================================
echo.

set PLUGIN_TARGET=%LOCALAPPDATA%\ColorBridge\Plugin
set LOGI_PLUGINS=%LOCALAPPDATA%\Logi\LogiPluginService\Plugins
set LINK_FILE=%LOGI_PLUGINS%\ColorBridge.link

echo [1/3] Copying plugin files to %PLUGIN_TARGET%...
if not exist "%PLUGIN_TARGET%" mkdir "%PLUGIN_TARGET%"
xcopy /E /Y /I "%~dp0*.*" "%PLUGIN_TARGET%" >nul

echo.
echo [2/3] Setting up Logitech connection...
if not exist "%LOGI_PLUGINS%" mkdir "%LOGI_PLUGINS%"
echo %PLUGIN_TARGET%> "%LINK_FILE%"

echo.
echo [3/3] Done! ColorBridge is now installed.
echo.
echo Please fully RESTART Logi Options+ to see it.
echo.
pause
