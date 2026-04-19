# ============================================================
#  Build-Installer.ps1
#  Builds the ColorBridge plugin and packages it for distribution.
#  Run this from the installer/ directory.
# ============================================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "  =======================================" -ForegroundColor Magenta
Write-Host "    ColorBridge Installer Packager" -ForegroundColor Magenta
Write-Host "  =======================================" -ForegroundColor Magenta
Write-Host ""

$repoRoot  = Split-Path -Parent $PSScriptRoot
$srcDir    = Join-Path $repoRoot "src\ColorBridgePlugin\src"
$outputDir = Join-Path $repoRoot "src\ColorBridgePlugin\bin\Debug"
$pluginDir = Join-Path $PSScriptRoot "plugin"

# Step 1 — Build the plugin
Write-Host "  [1/3] Building ColorBridge plugin..." -ForegroundColor Cyan
$buildResult = & dotnet build $srcDir -c Debug --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host ($buildResult | Out-String) -ForegroundColor Red
    Write-Host "  BUILD FAILED. Fix errors before packaging." -ForegroundColor Red
    exit 1
}
Write-Host "        Build succeeded." -ForegroundColor Green

# Step 2 — Clean old package
if (Test-Path $pluginDir) {
    Write-Host "  [2/3] Cleaning old package..." -ForegroundColor Cyan
    Remove-Item -Path $pluginDir -Recurse -Force
}
New-Item -ItemType Directory -Path $pluginDir -Force | Out-Null

# Step 3 — Copy built files preserving Loupedeck plugin layout
Write-Host "  [3/3] Packaging plugin files..." -ForegroundColor Cyan

# The Loupedeck plugin directory layout under Debug/ is:
#   bin/            -> DLL + deps
#   actionsymbols/  -> SVG action symbols
#   metadata/       -> LoupedeckPackage.yaml, Icon256x256.png, DefaultIconTemplate.ict
#   icontemplates/  -> .ict files (optional)
# We need to preserve this exact structure.

$folders = @("bin", "actionsymbols", "metadata", "icontemplates")
foreach ($folder in $folders) {
    $src = Join-Path $outputDir $folder
    if (Test-Path $src) {
        $dst = Join-Path $pluginDir $folder
        Copy-Item -Path $src -Destination $dst -Recurse -Force
    }
}

# Count files
$fileCount = (Get-ChildItem -Path $pluginDir -Recurse -File).Count

Write-Host ""
Write-Host "  Done! $fileCount files packaged into installer\plugin\" -ForegroundColor Green
Write-Host "  Distribute the entire 'installer' folder." -ForegroundColor DarkGray
Write-Host ""
