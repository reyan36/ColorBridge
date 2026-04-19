; ============================================================
;  ColorBridge Inno Setup Script
;  Produces a single .exe installer for the ColorBridge plugin.
;
;  Build:
;    1. powershell -ExecutionPolicy Bypass -File Build-Installer.ps1
;    2. "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ColorBridge.iss
; ============================================================

#define MyAppName "ColorBridge"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Reyan Arshad"
#define MyAppURL "https://github.com/reyan36/ColorBridge"

[Setup]
AppId={{A8F3D2E1-7B4C-4E9A-B6D5-1C2F8A9E3B7D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName={localappdata}\ColorBridge\Plugin
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=output
OutputBaseFilename=ColorBridge-Setup
SetupIconFile=ColorBridge.ico
UninstallDisplayIcon={app}\bin\ColorBridgePlugin.dll
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardSizePercent=100
PrivilegesRequired=lowest
UninstallDisplayName={#MyAppName}
WizardImageFile=WizardImage.bmp
WizardSmallImageFile=WizardSmallImage.bmp
LicenseFile=
InfoBeforeFile=

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
WelcomeLabel1=Welcome to ColorBridge
WelcomeLabel2=This will install ColorBridge v{#MyAppVersion} on your computer.%n%nColorBridge turns your Logitech MX Creative Console into a complete color workflow studio — pick colors, generate palettes, check WCAG contrast, and copy formatted values instantly.%n%n%nMade for the Logitech DevStudio Challenge 2026.%nThis plugin is NOT officially made by Logitech.%nDeveloped by Reyan Arshad.
FinishedHeadingLabel=ColorBridge Installed Successfully!
FinishedLabelNoIcons=ColorBridge has been installed into Logi Options+.%n%nIMPORTANT: Please restart Logi Options+ (close and reopen it) for the plugin to load.%n%nAfter restarting, open your MX Creative Console settings in Logi Options+ and look for ColorBridge actions.%n%n%nMade by Reyan Arshad%nBuilt for Logitech DevStudio Challenge 2026
FinishedLabel=ColorBridge has been installed into Logi Options+.%n%nIMPORTANT: Please restart Logi Options+ (close and reopen it) for the plugin to load.%n%nAfter restarting, open your MX Creative Console settings in Logi Options+ and look for ColorBridge actions.%n%n%nMade by Reyan Arshad%nBuilt for Logitech DevStudio Challenge 2026

[Files]
; Plugin binaries
Source: "plugin\bin\*"; DestDir: "{app}\bin"; Flags: ignoreversion recursesubdirs createallsubdirs
; Action symbols
Source: "plugin\actionsymbols\*"; DestDir: "{app}\actionsymbols"; Flags: ignoreversion
; Metadata (includes Icon256x256.png)
Source: "plugin\metadata\*"; DestDir: "{app}\metadata"; Flags: ignoreversion
; Icon templates (if present)
Source: "plugin\icontemplates\*"; DestDir: "{app}\icontemplates"; Flags: ignoreversion skipifsourcedoesntexist

[Run]
; Create the .link file to register with Logi Plugin Service
Filename: "cmd.exe"; Parameters: "/c echo|set /p=""{app}"" > ""{localappdata}\Logi\LogiPluginService\Plugins\ColorBridge.link"""; Flags: runhidden; StatusMsg: "Registering plugin with Logi Options+..."

[UninstallRun]
; Remove the .link file on uninstall
Filename: "cmd.exe"; Parameters: "/c del /q ""{localappdata}\Logi\LogiPluginService\Plugins\ColorBridge.link"""; Flags: runhidden; RunOnceId: "RemoveLink"

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
Type: files; Name: "{localappdata}\Logi\LogiPluginService\Plugins\ColorBridge.link"

[Code]
function IsLogiOptionsInstalled(): Boolean;
begin
  Result := True;
end;

function IsLogiOptionsRunning(): Boolean;
var
  ResultCode: Integer;
begin
  Exec('cmd.exe', '/c tasklist /FI "IMAGENAME eq logioptionsplus.exe" 2>nul | findstr /I "logioptionsplus" >nul', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := (ResultCode = 0);
end;

function IsPluginServiceRunning(): Boolean;
var
  ResultCode: Integer;
begin
  Exec('cmd.exe', '/c tasklist /FI "IMAGENAME eq LogiPluginService.exe" 2>nul | findstr /I "LogiPluginService" >nul', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := (ResultCode = 0);
end;

function InitializeSetup(): Boolean;
var
  Res: Integer;
begin
  Result := True;

  // Check if Logi Options+ is installed at all
  if not IsLogiOptionsInstalled() then
  begin
    MsgBox('Logi Options+ does not appear to be installed.' + #13#10 + #13#10 +
           'ColorBridge requires Logi Options+ to work.' + #13#10 +
           'Please install it from:' + #13#10 +
           'https://www.logitech.com/software/logi-options-plus.html' + #13#10 + #13#10 +
           'Setup will now exit.', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  // Check if Logi Options+ is currently running — offer to close
  if IsLogiOptionsRunning() then
  begin
    Res := MsgBox('Logi Options+ is currently running.' + #13#10 + #13#10 +
                  'It is recommended to close Logi Options+ before installing ' +
                  'so the plugin loads cleanly on next launch.' + #13#10 + #13#10 +
                  'Would you like to close Logi Options+ now?' + #13#10 + #13#10 +
                  'Click YES to close it automatically.' + #13#10 +
                  'Click NO to continue anyway (you will need to restart it after install).', 
                  mbConfirmation, MB_YESNOCANCEL);

    if Res = IDCANCEL then
    begin
      Result := False;
      Exit;
    end;

    if Res = IDYES then
    begin
      // Kill Logi Options+ and Plugin Service
      Exec('cmd.exe', '/c taskkill /F /IM logioptionsplus.exe >nul 2>&1', '', SW_HIDE, ewWaitUntilTerminated, Res);
      Exec('cmd.exe', '/c taskkill /F /IM LogiPluginService.exe >nul 2>&1', '', SW_HIDE, ewWaitUntilTerminated, Res);
      Sleep(1500);
    end;
  end
  else if not IsPluginServiceRunning() then
  begin
    MsgBox('Logi Options+ is installed but not running.' + #13#10 + #13#10 +
           'The plugin will be installed successfully.' + #13#10 +
           'Please start Logi Options+ after installation for the plugin to load.',
           mbInformation, MB_OK);
  end;
end;

// After install — remind to restart
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Try to start Logi Options+ again if we closed it
    if not IsLogiOptionsRunning() then
    begin
      // Don't auto-start — let user do it for a clean load
    end;
  end;
end;
