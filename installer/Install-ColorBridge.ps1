# ============================================================
#  Install-ColorBridge.ps1
#  One-click installer for the ColorBridge Logi Options+ plugin.
#  Requires: Windows, Logi Options+ installed
# ============================================================

$ErrorActionPreference = "Stop"

# ── Branding ─────────────────────────────────────────────────
function Show-Banner {
    Clear-Host
    Write-Host ""
    Write-Host "  ╔══════════════════════════════════════════════════╗" -ForegroundColor Magenta
    Write-Host "  ║                                                  ║" -ForegroundColor Magenta
    Write-Host "  ║        🎨  ColorBridge Installer  🎨            ║" -ForegroundColor Magenta
    Write-Host "  ║                                                  ║" -ForegroundColor Magenta
    Write-Host "  ║   Turn your MX Creative Console into a          ║" -ForegroundColor DarkGray
    Write-Host "  ║   complete Color Workflow Studio                 ║" -ForegroundColor DarkGray
    Write-Host "  ║                                                  ║" -ForegroundColor Magenta
    Write-Host "  ╚══════════════════════════════════════════════════╝" -ForegroundColor Magenta
    Write-Host ""
}

function Write-Step {
    param([string]$Step, [string]$Message)
    Write-Host "  [$Step] $Message" -ForegroundColor Cyan
}

function Write-OK {
    param([string]$Message)
    Write-Host "        ✅ $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "        ⚠️  $Message" -ForegroundColor Yellow
}

function Write-Fail {
    param([string]$Message)
    Write-Host "        ❌ $Message" -ForegroundColor Red
}

# ── Paths ────────────────────────────────────────────────────
$scriptDir       = $PSScriptRoot
$pluginSourceDir = Join-Path $scriptDir "plugin"
$installDir      = Join-Path $env:LOCALAPPDATA "ColorBridge\Plugin"
$pluginsRoot     = Join-Path $env:LOCALAPPDATA "Logi\LogiPluginService\Plugins"
$linkFile        = Join-Path $pluginsRoot "ColorBridge.link"
$logiServiceName = "LogiPluginService"

# ── Main ─────────────────────────────────────────────────────
Show-Banner

# Step 1 — Verify plugin files exist
Write-Step "1/5" "Checking installer package..."
if (-not (Test-Path $pluginSourceDir)) {
    Write-Fail "Plugin files not found at: $pluginSourceDir"
    Write-Host "        Run Build-Installer.ps1 first to package the plugin." -ForegroundColor DarkGray
    Write-Host ""
    Read-Host "  Press Enter to exit"
    exit 1
}

$dllPath = Join-Path $pluginSourceDir "ColorBridgePlugin.dll"
if (-not (Test-Path $dllPath)) {
    Write-Fail "ColorBridgePlugin.dll not found in package."
    Write-Host "        Run Build-Installer.ps1 first." -ForegroundColor DarkGray
    Write-Host ""
    Read-Host "  Press Enter to exit"
    exit 1
}
Write-OK "Plugin package found."

# Step 2 — Check Logi Options+
Write-Step "2/5" "Checking Logi Options+..."

$logiInstalled = Test-Path $pluginsRoot
if (-not $logiInstalled) {
    Write-Fail "Logi Options+ plugin directory not found."
    Write-Host "        Expected: $pluginsRoot" -ForegroundColor DarkGray
    Write-Host "        Please install Logi Options+ first:" -ForegroundColor DarkGray
    Write-Host "        https://www.logitech.com/software/logi-options-plus.html" -ForegroundColor Blue
    Write-Host ""
    Read-Host "  Press Enter to exit"
    exit 1
}
Write-OK "Logi Options+ detected."

# Step 3 — Check if Logi Plugin Service is running
Write-Step "3/5" "Checking Logi Plugin Service..."

$serviceProcess = Get-Process -Name "LogiPluginService" -ErrorAction SilentlyContinue
if ($serviceProcess) {
    Write-OK "Logi Plugin Service is running (PID: $($serviceProcess.Id))"
} else {
    Write-Warn "Logi Plugin Service is NOT running."
    Write-Host ""
    Write-Host "        The plugin will be installed, but you need to" -ForegroundColor DarkGray
    Write-Host "        start Logi Options+ for it to load." -ForegroundColor DarkGray
    Write-Host ""
}

# Step 4 — Install plugin files
Write-Step "4/5" "Installing ColorBridge plugin..."

# Create install directory
if (Test-Path $installDir) {
    Write-Host "        Removing previous installation..." -ForegroundColor DarkGray
    Remove-Item -Path $installDir -Recurse -Force
}

New-Item -ItemType Directory -Path $installDir -Force | Out-Null

# Copy all plugin files
Copy-Item -Path "$pluginSourceDir\*" -Destination $installDir -Recurse -Force

$installedCount = (Get-ChildItem -Path $installDir -Recurse -File).Count
Write-OK "$installedCount files installed to:"
Write-Host "        $installDir" -ForegroundColor DarkGray

# Step 5 — Create plugin link
Write-Step "5/5" "Registering with Logi Plugin Service..."

# The .link file contains the path to the plugin directory
# (the folder that contains bin/, actionsymbols/, metadata/)
Set-Content -Path $linkFile -Value $installDir -NoNewline -Encoding UTF8

Write-OK "Plugin registered: $linkFile"

# ── Reload ───────────────────────────────────────────────────
Write-Host ""
if ($serviceProcess) {
    Write-Host "  Sending reload command..." -ForegroundColor DarkGray
    try {
        $reloadUrl = "loupedeck:plugin/ColorBridge/reload"
        Start-Process $reloadUrl -ErrorAction SilentlyContinue
        Write-OK "Reload signal sent."
    } catch {
        Write-Warn "Could not send reload. Restart Logi Options+ manually."
    }
}

# ── Done ─────────────────────────────────────────────────────
Write-Host ""
Write-Host "  ╔══════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "  ║                                                  ║" -ForegroundColor Green
Write-Host "  ║   ✅  ColorBridge installed successfully!        ║" -ForegroundColor Green
Write-Host "  ║                                                  ║" -ForegroundColor Green
Write-Host "  ║   Open Logi Options+ and look for ColorBridge    ║" -ForegroundColor DarkGray
Write-Host "  ║   in your MX Creative Console actions.           ║" -ForegroundColor DarkGray
Write-Host "  ║                                                  ║" -ForegroundColor Green
Write-Host "  ╚══════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Read-Host "  Press Enter to close"
