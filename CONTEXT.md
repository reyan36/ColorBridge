# ColorBridge — AI Context File

This file is the authoritative handoff for any future AI session or contributor working on ColorBridge. Read it first.

If the user gives instructions that conflict with this file, ask the user which should win.

---

## 1. Project Summary

- Project: `ColorBridge`
- Type: Logitech MX Creative Console / Loupedeck plugin
- Language: C#, .NET 8
- Main goal: turn the Creative Console into a color workflow device for picking, converting, checking contrast, generating palettes, loading presets, and extracting colors from images.

---

## 2. Current Repository Structure

```text
ColorBridge/
├── src/
│   ├── ColorBridgePlugin/
│   │   ├── bin/
│   │   ├── package/
│   │   │   ├── actionsymbols/          # SVG action symbols for Logi Options+ sidebar
│   │   │   ├── icontemplates/          # .ict icon templates (legacy, purged at build)
│   │   │   └── metadata/
│   │   └── src/
│   │       ├── Application.cs
│   │       ├── Plugin.cs
│   │       ├── ColorBridgePlugin.csproj
│   │       ├── Directory.Build.props
│   │       ├── Assets/                 # Embedded PNG assets
│   │       ├── Engine/
│   │       │   ├── ColorConverter.cs
│   │       │   ├── ColorEngine.cs
│   │       │   ├── ImagePaletteExtractor.cs
│   │       │   ├── PaletteGenerator.cs
│   │       │   └── WcagChecker.cs
│   │       ├── Features/
│   │       │   ├── Dials/
│   │       │   │   ├── HueDialAdjustment.cs
│   │       │   │   ├── SatLightAdjustment.cs
│   │       │   │   └── SchemeCycleCommand.cs
│   │       │   ├── Palette/
│   │       │   │   └── PaletteFolder.cs
│   │       │   ├── Presets/
│   │       │   │   └── PresetsFolder.cs
│   │       │   └── Tools/
│   │       │       ├── ContrastBackgroundToggleCommand.cs
│   │       │       ├── ContrastCheckCommand.cs
│   │       │       ├── FormatConvertCommand.cs
│   │       │       ├── FromImageCommand.cs
│   │       │       ├── GeneratePaletteCommand.cs
│   │       │       ├── RandomColorCommand.cs
│   │       │       ├── SavePaletteCommand.cs
│   │       │       ├── ScreenPickerCommand.cs
│   │       │       ├── ShadesFolder.cs
│   │       │       └── TintsFolder.cs
│   │       ├── Helpers/
│   │       ├── Platform/
│   │       │   ├── ClipboardService.cs
│   │       │   └── ScreenColorPicker.cs
│   │       └── Rendering/
│   │           ├── ButtonTextRenderer.cs
│   │           ├── IconRenderer.cs
│   │           ├── PluginTheme.cs
│   │           └── SwatchRenderer.cs
├── .gitignore
├── COLOR_GUIDELINES.md
├── CONTEXT.md
├── LICENSE
├── PLAN.md
├── README.md
└── prototype.html
```

---

## 3. Build and Runtime

### Plugin build
```powershell
cd "E:/Hackathons Unfinished Projects/ColorBridge/src/ColorBridgePlugin/src"
dotnet build -c Debug
```

What happens:
- builds `ColorBridgePlugin.dll`
- copies package files (including `actionsymbols/`)
- writes a `.link` file into `%LocalAppData%\Logi\LogiPluginService\Plugins\`
- sends `loupedeck:plugin/ColorBridge/reload`

### Cache refresh
If SVG action symbols or icon changes don't appear:
1. Run `dotnet clean` then `dotnet build -c Debug`
2. Restart the Logi Plugin Service from Logi Options+ settings

### PowerShell note
This repo is being worked in PowerShell. Do **not** use `&&`. Use `;` or separate commands.

---

## 4. Plugin Architecture

### 4.1 Lifecycle
`src/ColorBridgePlugin/src/Plugin.cs`
- Initializes `PluginLog`
- Initializes `PluginResources`
- Accesses `ColorEngine.Instance`

**Removed features:** `DesktopValueHud` and `DesktopActivityFeed` have been fully removed. No helper app (`ColorBridgeHud`) is used.

### 4.2 Color state
`src/ColorBridgePlugin/src/Engine/ColorEngine.cs`
- Singleton source of truth
- Stores:
  - `Hue`
  - `Saturation`
  - `Lightness`
  - `ActiveFormat` (`HEX`, `RGB`, `HSL`, `CMYK`)
  - `CurrentScheme` (includes `Single`, `Complementary`, `Analogous`, `Triadic`, `SplitComplementary`, `Monochromatic`, `Shades`, `Tints`)
  - `Palette` (9 entries)
  - `CurrentSubDialMode` (`Saturation` / `Lightness`)
  - `CurrentContrastBackground` (`White` / `Dark`)
- Raises events:
  - `ColorChanged`
  - `PaletteChanged`
  - `FormatChanged`
  - `SubDialModeChanged`
  - `ContrastBackgroundChanged`

### 4.3 Palette generation
`src/ColorBridgePlugin/src/Engine/PaletteGenerator.cs`
- `SchemeType` enum includes: `Single`, `Complementary`, `Analogous`, `Triadic`, `SplitComplementary`, `Monochromatic`, `Shades`, `Tints`
- `Shades` and `Tints` are first-class scheme types (not inline generation)
- `OnColorChanged()` in `ColorEngine` calls `RegeneratePalette()` which uses the current scheme, so shades/tints auto-update when the base color changes

### 4.4 Contrast mode
Contrast is no longer "best of white or black".
The plugin now has an explicit contrast background mode in `ColorEngine`:
- `White`
- `Dark`

`GetContrastBackgroundRgb()` returns:
- white = `(255,255,255)`
- dark = `(26,26,46)` which matches the prototype dark surface better than pure black

### 4.5 Rendering strategy
All dynamic UI is now rendered using `BitmapBuilder` code. There are no static `.ict` templates used at runtime.

#### A. BitmapBuilder dynamic rendering (primary pattern)
All tool buttons, dials, and folder contents use `BitmapBuilder` directly in `GetCommandImage`:
- `SatLightAdjustment` — dual gradient strips with sliding dot indicators
- `HueDialAdjustment` — dynamic progress bar
- `SchemeAdjustment` — color accent bar + scheme name
- `RandomColorCommand` — animated orbital effect
- `GeneratePaletteCommand` — dynamic text
- Folder buttons (Shades, Tints, Palette, Presets) — use Logi's default folder rendering (no custom `GetButtonImage`)

#### B. User-authored full-image buttons
Use:
- `IconRenderer.RenderImageOnly(fileName)`

Used for:
- `Format Convert`
- `From Image`
- `Save Palette`
- `Screen Picker`

#### C. SwatchRenderer
Used for palette slot, shade, and tint buttons inside folders. Shows solid color fill + HEX overlay text.

### 4.6 Action Symbols (SVG)
Action symbols for the Logi Options+ sidebar live in `package/actionsymbols/`.

**Requirements (from Logi docs):**
- Must be SVG format
- Must have transparent background
- Must use **single color black** strokes and fills (Logi handles color inversion)
- File naming: `Loupedeck.<Namespace>.<ClassName>.svg`

**Current symbols:**
- `PaletteFolder.svg` — folder with palette grid
- `PresetsFolder.svg` — folder with star
- `ShadesFolder.svg` — folder with dark moon
- `TintsFolder.svg` — folder with sun
- `ContrastBackgroundToggleCommand.svg`
- `ContrastCheckCommand.svg`
- `GeneratePaletteCommand.svg` — Venn diagram circles
- `RandomColorCommand.svg` — dice icon
- `ScreenPickerCommand.svg`

### 4.7 Button refresh pattern (CRITICAL)
When state changes (color, palette, format), every button that displays dependent data must be explicitly refreshed. The pattern used across all folders:

```csharp
private void RefreshAllSlots()
{
    this.ButtonActionNamesChanged();
    for (var i = 1; i <= 9; i++)
    {
        this.CommandImageChanged(i.ToString());
    }
}
```

**Key rules:**
- `ButtonActionNamesChanged()` alone is NOT sufficient — some buttons silently skip redraw
- Must call `CommandImageChanged(param)` for EACH parameter individually
- Parameter strings must be character-for-character identical to what `GetCommandImage` receives
- Folder buttons use parameters `"1"` through `"9"` (1-indexed, not 0-indexed)
- For `PluginDynamicAdjustment`, use `this.ActionImageChanged()` (no parameter)

---

## 5. Current Feature Set

### Page 1 — Tools
- `Contrast Check` — WCAG AA/AAA check against selected background
- `Contrast Background` — toggles white / dark comparison background
- `Format Convert` — cycles HEX/RGB/HSL/CMYK, uses user-authored images
- `From Image` — extracts dominant color from image file
- `Generate Palette` — cycles schemes, dynamic text rendering
- `Random Color` — sets random HSL, animated orbital effect
- `Save Palette` — copies all 9 palette colors to clipboard
- `Screen Picker` — samples pixel under cursor
- `Shades` — dynamic folder, generates shade scale (1-indexed)
- `Tints` — dynamic folder, generates tint scale (1-indexed)

### Page 2 — Palette
- `Palette Slots` — 9-slot color swatches (1-indexed), tap to select + copy

### Page 3 — Presets
- `Palette Presets` — preset palette loader with 8 themes + `+ New` random entry

### Page 4 — Dials
- `Hue Wheel` — adjust hue, press resets to 0, dynamic progress bar
- `Sat / Light` — dual gradient strips with dot indicators, press toggles mode
- `Palette Scheme` — color accent bar + scheme name, dial cycles schemes

---

## 6. Important Files

- `src/ColorBridgePlugin/src/Plugin.cs` — plugin load/unload
- `src/ColorBridgePlugin/src/Engine/ColorEngine.cs` — state + events
- `src/ColorBridgePlugin/src/Engine/ColorConverter.cs` — HSL/RGB/HEX/CMYK + luminance
- `src/ColorBridgePlugin/src/Engine/WcagChecker.cs` — WCAG contrast math
- `src/ColorBridgePlugin/src/Engine/PaletteGenerator.cs` — palette generation (8 scheme types)
- `src/ColorBridgePlugin/src/Engine/ImagePaletteExtractor.cs` — image analysis
- `src/ColorBridgePlugin/src/Rendering/SwatchRenderer.cs` — palette swatches
- `src/ColorBridgePlugin/src/Rendering/IconRenderer.cs` — embedded image loading
- `src/ColorBridgePlugin/src/Platform/ClipboardService.cs` — clipboard via STA threads
- `src/ColorBridgePlugin/src/Platform/ScreenColorPicker.cs` — cursor pixel sampling

---

## 7. Design Rules

### Colors
- `BgDeep` = `#0A0A0F`
- `BgSurface` = `#141420`
- `BgSurface2` = `#1C1C2E`
- `TextPrimary` = `#E8E8F0`
- `AccentPrimary` = `#6366F1`
- `AccentViolet` = `#C084FC`
- `Success` = `#22C55E`
- `Warning` = `#F59E0B`
- `Danger` = `#EF4444`

### Icon rules
1. All dynamic UI uses `BitmapBuilder` code in `GetCommandImage`.
2. If the user provides a full button image, use image-only rendering via `IconRenderer`.
3. Folder outer icons use Logi's default folder renderer (no custom `GetButtonImage`).
4. Action symbols must be black-stroke SVGs with transparent backgrounds.
5. Do not blank `displayName` just to hide preview labels in Logi Options+.

---

## 8. Gotchas / Caveats

1. Logi Options+ preview text under icons is host UI, not plugin-rendered device content.
2. `System.Drawing.Common` is Windows-only; `From Image` is gated accordingly.
3. Clipboard access must stay on STA threads.
4. Screen picker is Windows-only.
5. Use PowerShell-safe command chaining (`;`, not `&&`).
6. `SchemeAdjustment` lives in `SchemeCycleCommand.cs`.
7. Folder slot parameters are 1-indexed strings (`"1"` through `"9"`).
8. `ActionImageChanged("")` with empty string does NOT refresh buttons with non-empty parameters.
9. SVG action symbol changes require Logi Plugin Service restart.
10. `.ict` files are purged at build time — do not rely on them.

---

## 9. How to Extend Safely

When adding a feature:
1. Put commands under `Features/<Category>/`.
2. Use `PluginDynamicCommand` or `PluginDynamicAdjustment`.
3. Keep action names stable unless the user explicitly wants renaming.
4. For a new image icon, place it in `src/ColorBridgePlugin/src/Assets/`.
5. For a new action symbol, create an SVG in `package/actionsymbols/` using the full class name.
6. Build from `src/ColorBridgePlugin/src` after every change.
7. Use the `RefreshAllSlots()` pattern for any folder with dynamic content.
8. Subscribe to ALL relevant events (`ColorChanged`, `PaletteChanged`, `FormatChanged`).

---

## 10. Removed Features

The following features were removed during the UI overhaul:
- **Desktop HUD** (`DesktopValueHud.cs`) — deleted
- **Desktop Activity Feed** (`DesktopActivityFeed.cs`) — deleted
- **Desktop HUD Toggle** (`DesktopHudToggleCommand.cs`) — deleted
- **Desktop Feed Toggle** (`DesktopActivityFeedToggleCommand.cs`) — deleted
- **ColorBridgeHud helper app** — no longer used
- **Static .ict icon templates** — purged at build time
- **Shell asset PNGs** (`cb-shades-shell.png`, etc.) — no longer used for dynamic rendering

---

## 11. Rules For Future AI Sessions

- Read this file first.
- Do not remove or rename mapped actions casually.
- Keep Page grouping consistent:
  - `Page 1 — Tools`
  - `Page 2 — Palette`
  - `Page 3 — Presets`
  - `Page 4 — Dials`
- Prefer `BitmapBuilder` for all dynamic rendering.
- Always use `RefreshAllSlots()` pattern (explicit per-parameter refresh).
- Never commit without user asking.
- Never modify git config.
- Keep Windows / PowerShell constraints in mind.

---

## 12. Ownership

Single-author hackathon project. Maintain current structure and style unless the user explicitly asks for a larger redesign.
