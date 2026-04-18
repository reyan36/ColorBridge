# ColorBridge
**Turn your MX Creative Console into a Color Studio**

## Overview
ColorBridge is a Logi Actions SDK plugin that transforms the MX Creative Console into a complete color management workstation for designers and developers.

## Demo Video
[Watch on YouTube](https://youtu.be/RLZL3CApXHo?si=iAAx_2kYsdYSOnmH) 

## The Problem
Designers and developers are constantly switching between different tools just to work with colors:
- Browser extensions for picking colors from screen
- Separate websites to check accessibility standards
- Online converters for HEX/RGB/HSL formats
- Different apps to manage color palettes

It's messy, inefficient, and breaks your workflow.

## The Solution
ColorBridge unifies the entire color workflow into one hardware device:

- **9 LCD Buttons** — Display live color swatches with HEX codes. Press to copy.
- **Main Dial** — Physical color wheel controlling Hue (0–360°)
- **Sub-Dial** — Adjust Saturation and Lightness with real-time gradient strips
- **Actions Ring** — Quick actions: Screen Picker, Format Convert, Contrast Check, Palette Generator

**Works everywhere:** Figma, Photoshop, VS Code, browsers — no app-specific plugins needed.

## Features

### Color Tools
- **Screen Picker** — Sample any pixel on screen, auto-copy formatted color
- **Format Convert** — Cycle between HEX, RGB, HSL, CMYK with one press
- **Contrast Check** — Real-time WCAG AA/AAA compliance checker
- **Random Color** — Generate random colors with animated feedback
- **Generate Palette** — Auto-generate palettes (Complementary, Analogous, Triadic, Split, Mono)

### Palette Management
- **9-Slot Palette** — Visual color swatches, tap to select + copy
- **Shades & Tints** — Auto-generate shade/tint scales from current color
- **8 Preset Palettes** — Brand, Material, Pastel, Earth, Neon, Ocean, Sunset, Forest
- **Save Palette** — Export all 9 colors to clipboard
- **From Image** — Extract dominant color from any image file

### Dial Controls
- **Hue Wheel** — Rotate to sweep the full 360° color wheel
- **Sat / Light** — Dual gradient strips with sliding dot indicators
- **Palette Scheme** — Scroll to cycle through palette generation algorithms

## Technical Stack

Built with **Logi Actions SDK (C#, .NET 8)**:
- `BitmapBuilder` — All dynamic UI rendered in code (gradient strips, progress bars, swatches)
- `PluginDynamicFolder` — Palette slots, shades, tints, and presets as interactive button grids
- `PluginDynamicAdjustment` — Dial-driven HSL controls and scheme cycling
- SVG action symbols for Logi Options+ sidebar
- System-wide clipboard integration
- WCAG contrast calculation algorithms

## Project Structure

```
ColorBridge/
├── src/ColorBridgePlugin/
│   └── src/
│       ├── Engine/         # ColorEngine, ColorConverter, PaletteGenerator, WcagChecker
│       ├── Features/
│       │   ├── Dials/      # HueDialAdjustment, SatLightAdjustment, SchemeAdjustment
│       │   ├── Palette/    # PaletteFolder (9-slot swatches)
│       │   ├── Presets/    # PresetsFolder (8 themes + random)
│       │   └── Tools/      # All tool commands + Shades/Tints folders
│       ├── Platform/       # ClipboardService, ScreenColorPicker
│       └── Rendering/      # SwatchRenderer, IconRenderer, PluginTheme
├── CONTEXT.md              # AI context / developer handoff document
├── prototype.html          # Interactive web prototype
└── README.md
```

## Building

```powershell
cd src/ColorBridgePlugin/src
dotnet build -c Debug
```

The build automatically:
- Compiles the plugin DLL
- Copies package files (SVG symbols, metadata)
- Creates the plugin link for Logi Plugin Service
- Sends a reload command

## Target Audience
- UI/UX Designers managing brand colors and design systems
- Front-end Developers working with CSS and color variables
- Brand Managers ensuring accessibility compliance
- Digital Artists requiring precise color control
- Anyone who works with colors across multiple applications daily

## Documentation
- [CONTEXT.md](CONTEXT.md) — Full technical context and architecture docs
- [ColorBridge Documentation.docx](ColorBridge%20Documentation.docx) — Detailed concept overview

## Developer

- [@Reyan Arshad](https://www.linkedin.com/in/reyan36/)

## License

Distributed under the MIT License. See `LICENSE` for more information.

---

**Built for the Logitech DevStudio Challenge 2026**
