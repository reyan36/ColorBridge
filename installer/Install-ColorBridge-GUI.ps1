# ============================================================
#  Install-ColorBridge-GUI.ps1
#  GUI installer for the ColorBridge Logi Options+ plugin.
# ============================================================

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = "Stop"

# ── Paths ────────────────────────────────────────────────────
$scriptDir       = $PSScriptRoot
$pluginSourceDir = Join-Path $scriptDir "plugin"
$installDir      = Join-Path $env:LOCALAPPDATA "ColorBridge\Plugin"
$pluginsRoot     = Join-Path $env:LOCALAPPDATA "Logi\LogiPluginService\Plugins"
$linkFile        = Join-Path $pluginsRoot "ColorBridge.link"

# ── Theme Colors ─────────────────────────────────────────────
$bgDark      = [System.Drawing.Color]::FromArgb(18, 18, 28)
$bgPanel     = [System.Drawing.Color]::FromArgb(26, 26, 42)
$bgButton    = [System.Drawing.Color]::FromArgb(99, 102, 241)
$bgBtnHover  = [System.Drawing.Color]::FromArgb(120, 122, 255)
$bgCancel    = [System.Drawing.Color]::FromArgb(55, 55, 75)
$textWhite   = [System.Drawing.Color]::FromArgb(232, 232, 240)
$textMuted   = [System.Drawing.Color]::FromArgb(140, 140, 165)
$accentGreen = [System.Drawing.Color]::FromArgb(34, 197, 94)
$accentRed   = [System.Drawing.Color]::FromArgb(239, 68, 68)
$accentViolet= [System.Drawing.Color]::FromArgb(192, 132, 252)

# ── Fonts ────────────────────────────────────────────────────
$fontTitle   = New-Object System.Drawing.Font("Segoe UI", 22, [System.Drawing.FontStyle]::Bold)
$fontSub     = New-Object System.Drawing.Font("Segoe UI", 10)
$fontBody    = New-Object System.Drawing.Font("Segoe UI", 9.5)
$fontBtn     = New-Object System.Drawing.Font("Segoe UI Semibold", 10)
$fontStatus  = New-Object System.Drawing.Font("Segoe UI", 9)
$fontSmall   = New-Object System.Drawing.Font("Segoe UI", 8)

# ── Main Form ────────────────────────────────────────────────
$form = New-Object System.Windows.Forms.Form
$form.Text = "ColorBridge Installer"
$form.Size = New-Object System.Drawing.Size(520, 420)
$form.StartPosition = "CenterScreen"
$form.FormBorderStyle = "FixedSingle"
$form.MaximizeBox = $false
$form.BackColor = $bgDark
$form.ForeColor = $textWhite

# ── Header Panel ─────────────────────────────────────────────
$headerPanel = New-Object System.Windows.Forms.Panel
$headerPanel.Dock = "Top"
$headerPanel.Height = 100
$headerPanel.BackColor = $bgPanel

$lblIcon = New-Object System.Windows.Forms.Label
$lblIcon.Text = [char]0x1F3A8  # palette emoji fallback
$lblIcon.Font = New-Object System.Drawing.Font("Segoe UI Emoji", 30)
$lblIcon.Location = New-Object System.Drawing.Point(20, 18)
$lblIcon.AutoSize = $true
$lblIcon.ForeColor = $accentViolet
$headerPanel.Controls.Add($lblIcon)

$lblTitle = New-Object System.Windows.Forms.Label
$lblTitle.Text = "ColorBridge"
$lblTitle.Font = $fontTitle
$lblTitle.Location = New-Object System.Drawing.Point(78, 18)
$lblTitle.AutoSize = $true
$lblTitle.ForeColor = $textWhite
$headerPanel.Controls.Add($lblTitle)

$lblSubtitle = New-Object System.Windows.Forms.Label
$lblSubtitle.Text = "MX Creative Console Color Plugin"
$lblSubtitle.Font = $fontSub
$lblSubtitle.Location = New-Object System.Drawing.Point(82, 58)
$lblSubtitle.AutoSize = $true
$lblSubtitle.ForeColor = $textMuted
$headerPanel.Controls.Add($lblSubtitle)

$form.Controls.Add($headerPanel)

# ── Body Panel ───────────────────────────────────────────────
$bodyPanel = New-Object System.Windows.Forms.Panel
$bodyPanel.Location = New-Object System.Drawing.Point(0, 100)
$bodyPanel.Size = New-Object System.Drawing.Size(520, 220)
$bodyPanel.BackColor = $bgDark

# Description
$lblDesc = New-Object System.Windows.Forms.Label
$lblDesc.Text = "This will install ColorBridge into Logi Options+.`nThe plugin turns your Creative Console into a color workflow studio."
$lblDesc.Font = $fontBody
$lblDesc.Location = New-Object System.Drawing.Point(30, 15)
$lblDesc.Size = New-Object System.Drawing.Size(450, 42)
$lblDesc.ForeColor = $textMuted
$bodyPanel.Controls.Add($lblDesc)

# Install path label
$lblPath = New-Object System.Windows.Forms.Label
$lblPath.Text = "Install location:"
$lblPath.Font = $fontSmall
$lblPath.Location = New-Object System.Drawing.Point(30, 70)
$lblPath.AutoSize = $true
$lblPath.ForeColor = $textMuted
$bodyPanel.Controls.Add($lblPath)

$lblPathValue = New-Object System.Windows.Forms.Label
$lblPathValue.Text = $installDir
$lblPathValue.Font = $fontSmall
$lblPathValue.Location = New-Object System.Drawing.Point(30, 88)
$lblPathValue.Size = New-Object System.Drawing.Size(450, 18)
$lblPathValue.ForeColor = $accentViolet
$bodyPanel.Controls.Add($lblPathValue)

# Status items (checkmarks appear during install)
$statusLabels = @()
$statusItems = @(
    "Verify plugin package",
    "Check Logi Options+",
    "Check Plugin Service",
    "Copy plugin files",
    "Register plugin"
)

for ($i = 0; $i -lt $statusItems.Count; $i++) {
    $lbl = New-Object System.Windows.Forms.Label
    $lbl.Text = "     " + $statusItems[$i]
    $lbl.Font = $fontStatus
    $lbl.Location = New-Object System.Drawing.Point(40, 120 + ($i * 20))
    $lbl.Size = New-Object System.Drawing.Size(420, 18)
    $lbl.ForeColor = $textMuted
    $lbl.Visible = $false
    $bodyPanel.Controls.Add($lbl)
    $statusLabels += $lbl
}

$form.Controls.Add($bodyPanel)

# ── Footer Panel ─────────────────────────────────────────────
$footerPanel = New-Object System.Windows.Forms.Panel
$footerPanel.Dock = "Bottom"
$footerPanel.Height = 60
$footerPanel.BackColor = $bgPanel

# Install Button
$btnInstall = New-Object System.Windows.Forms.Button
$btnInstall.Text = "Install"
$btnInstall.Font = $fontBtn
$btnInstall.Size = New-Object System.Drawing.Size(120, 38)
$btnInstall.Location = New-Object System.Drawing.Point(260, 11)
$btnInstall.BackColor = $bgButton
$btnInstall.ForeColor = [System.Drawing.Color]::White
$btnInstall.FlatStyle = "Flat"
$btnInstall.FlatAppearance.BorderSize = 0
$btnInstall.Cursor = "Hand"
$footerPanel.Controls.Add($btnInstall)

# Cancel Button
$btnCancel = New-Object System.Windows.Forms.Button
$btnCancel.Text = "Cancel"
$btnCancel.Font = $fontBtn
$btnCancel.Size = New-Object System.Drawing.Size(100, 38)
$btnCancel.Location = New-Object System.Drawing.Point(390, 11)
$btnCancel.BackColor = $bgCancel
$btnCancel.ForeColor = $textMuted
$btnCancel.FlatStyle = "Flat"
$btnCancel.FlatAppearance.BorderSize = 0
$btnCancel.Cursor = "Hand"
$footerPanel.Controls.Add($btnCancel)

$form.Controls.Add($footerPanel)

# ── Helper functions ─────────────────────────────────────────
function Set-StepStatus {
    param([int]$Index, [string]$Status, [System.Drawing.Color]$Color)
    $prefix = switch ($Status) {
        "ok"      { [char]0x2714 }  # checkmark
        "fail"    { [char]0x2718 }  # X mark
        "warn"    { [char]0x26A0 }  # warning
        "working" { [char]0x25CF }  # filled circle
        default   { " " }
    }
    $statusLabels[$Index].Text = "  $prefix  " + $statusItems[$Index]
    $statusLabels[$Index].ForeColor = $Color
    $statusLabels[$Index].Visible = $true
    $form.Refresh()
    Start-Sleep -Milliseconds 350
}

function Show-AllSteps {
    foreach ($lbl in $statusLabels) {
        $lbl.Visible = $true
    }
}

# ── Cancel handler ───────────────────────────────────────────
$btnCancel.Add_Click({
    $form.Close()
})

# ── Install handler ──────────────────────────────────────────
$btnInstall.Add_Click({
    $btnInstall.Enabled = $false
    $btnInstall.BackColor = $bgCancel
    $btnCancel.Enabled = $false
    $lblDesc.Text = "Installing ColorBridge..."
    Show-AllSteps

    $success = $true

    # Step 1 — Verify package
    Set-StepStatus 0 "working" $accentViolet
    $dllPath = Join-Path $pluginSourceDir "bin\ColorBridgePlugin.dll"
    if (-not (Test-Path $dllPath)) {
        Set-StepStatus 0 "fail" $accentRed
        $lblDesc.Text = "Plugin files not found. Run Build-Installer.ps1 first."
        $lblDesc.ForeColor = $accentRed
        $btnCancel.Enabled = $true
        $btnCancel.Text = "Close"
        return
    }
    Set-StepStatus 0 "ok" $accentGreen

    # Step 2 — Check Logi Options+
    Set-StepStatus 1 "working" $accentViolet
    if (-not (Test-Path $pluginsRoot)) {
        Set-StepStatus 1 "fail" $accentRed
        $lblDesc.Text = "Logi Options+ not found. Please install it first."
        $lblDesc.ForeColor = $accentRed
        $btnCancel.Enabled = $true
        $btnCancel.Text = "Close"
        return
    }
    Set-StepStatus 1 "ok" $accentGreen

    # Step 3 — Check Plugin Service
    Set-StepStatus 2 "working" $accentViolet
    $serviceProc = Get-Process -Name "LogiPluginService" -ErrorAction SilentlyContinue
    if ($serviceProc) {
        $statusLabels[2].Text = "  $([char]0x2714)  Check Plugin Service (running)"
        $statusLabels[2].ForeColor = $accentGreen
    } else {
        $statusLabels[2].Text = "  $([char]0x26A0)  Plugin Service not running (start Logi Options+ later)"
        $statusLabels[2].ForeColor = [System.Drawing.Color]::FromArgb(245, 158, 11)
    }
    $form.Refresh()
    Start-Sleep -Milliseconds 350

    # Step 4 — Copy files
    Set-StepStatus 3 "working" $accentViolet
    try {
        if (Test-Path $installDir) {
            Remove-Item -Path $installDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $installDir -Force | Out-Null
        Copy-Item -Path "$pluginSourceDir\*" -Destination $installDir -Recurse -Force
        Set-StepStatus 3 "ok" $accentGreen
    } catch {
        Set-StepStatus 3 "fail" $accentRed
        $lblDesc.Text = "Failed to copy files: $_"
        $lblDesc.ForeColor = $accentRed
        $btnCancel.Enabled = $true
        $btnCancel.Text = "Close"
        return
    }

    # Step 5 — Register plugin
    Set-StepStatus 4 "working" $accentViolet
    try {
        Set-Content -Path $linkFile -Value $installDir -NoNewline -Encoding UTF8
        Set-StepStatus 4 "ok" $accentGreen
    } catch {
        Set-StepStatus 4 "fail" $accentRed
        $lblDesc.Text = "Failed to register plugin: $_"
        $lblDesc.ForeColor = $accentRed
        $btnCancel.Enabled = $true
        $btnCancel.Text = "Close"
        return
    }

    # Reload if service is running
    if ($serviceProc) {
        try {
            Start-Process "loupedeck:plugin/ColorBridge/reload" -ErrorAction SilentlyContinue
        } catch { }
    }

    # Done!
    $lblDesc.Text = "ColorBridge has been installed successfully!`nOpen Logi Options+ to find it in your Creative Console actions."
    $lblDesc.ForeColor = $accentGreen
    $btnCancel.Enabled = $true
    $btnCancel.Text = "Finish"
    $btnInstall.Text = "Done"
})

# ── Show form ────────────────────────────────────────────────
[void]$form.ShowDialog()
$form.Dispose()
