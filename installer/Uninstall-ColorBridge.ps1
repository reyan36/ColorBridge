# ============================================================
#  Uninstall-ColorBridge.ps1
#  Removes the ColorBridge plugin from Logi Options+.
# ============================================================

$ErrorActionPreference = "Stop"

Clear-Host
Write-Host ""
Write-Host "  ╔══════════════════════════════════════════════════╗" -ForegroundColor Yellow
Write-Host "  ║   ColorBridge Uninstaller                        ║" -ForegroundColor Yellow
Write-Host "  ╚══════════════════════════════════════════════════╝" -ForegroundColor Yellow
Write-Host ""

$installDir = Join-Path $env:LOCALAPPDATA "ColorBridge"
$linkFile   = Join-Path $env:LOCALAPPDATA "Logi\LogiPluginService\Plugins\ColorBridge.link"

$removed = $false

if (Test-Path $linkFile) {
    Remove-Item -Path $linkFile -Force
    Write-Host "  ✅ Plugin link removed." -ForegroundColor Green
    $removed = $true
}

if (Test-Path $installDir) {
    Remove-Item -Path $installDir -Recurse -Force
    Write-Host "  ✅ Plugin files removed." -ForegroundColor Green
    $removed = $true
}

if (-not $removed) {
    Write-Host "  ℹ️  ColorBridge was not installed." -ForegroundColor DarkGray
} else {
    Write-Host ""
    Write-Host "  ColorBridge has been uninstalled." -ForegroundColor Green
    Write-Host "  Restart Logi Options+ to complete removal." -ForegroundColor DarkGray
}

Write-Host ""
Read-Host "  Press Enter to close"
