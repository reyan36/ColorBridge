<div align="center">

# ColorBridge 
[![Download](https://img.shields.io/badge/Download-Latest%20Release-brightgreen?style=for-the-badge&logo=github)](https://github.com/reyan36/ColorBridge/releases/tag/Version-1.0)
[![Watch Demo](https://img.shields.io/badge/Watch-Demo-red?style=for-the-badge&logo=youtube)](https://youtu.be/U-4ly7aRNjk?si=MYWKh377oJ782p2X)
[![Devpost](https://img.shields.io/badge/View-Devpost-003E54?style=for-the-badge&logo=devpost)](https://devpost.com/software/colorbridge-phase-2)

### Turn your MX Creative Console into a Color Studio.
[![Version](https://img.shields.io/badge/Version-1.0-blue.svg)](https://github.com/reyan36/ColorBridge/releases/tag/Version-1.0)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Logi Actions](https://img.shields.io/badge/Logi_Actions_SDK-000000?style=flat&logo=logitech&logoColor=white)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[Features](#features) • [Installation](#installation) • [Technologies](#technologies) • [Developer](#developer)
---
</div>

### Project Overview
A Logi Actions SDK plugin that turns the MX Creative Console into a unified color workstation sample pixels, generate palettes, check contrast, and export to Figma or VS Code, all from the device.
 
---
 
### Features
 
**Actions Ring**
| Feature | Description |
| :--- | :--- |
| **Copy HEX** | Copy the active color as a HEX code to clipboard. |
| **Copy RGB** | Copy the active color as an RGB value to clipboard. |
| **Copy HSL** | Copy the active color as an HSL value to clipboard. |
| **Copy All Formats** | Copy the active color in all formats at once. |
| **Darken** | Darken the current color by a fixed step. |
| **Lighten** | Lighten the current color by a fixed step. |
| **Invert Color** | Invert the current color to its opposite. |
| **Paste Color** | Paste a color from clipboard into the engine. |
| **Save Preset** | Save the current color to a preset slot. |
 
**Tools**
| Feature | Description |
| :--- | :--- |
| **Screen Picker** | Sample any pixel on your desktop with one tap. |
| **Contrast Check** | Real-time WCAG AA/AAA pass/fail feedback. |
| **Contrast Background Toggle** | Switch contrast checker between light and dark backgrounds. |
| **Format Cycle** | Cycle the active format between HEX, RGB, HSL, and CMYK. |
| **From Image** | Extract the dominant color from any image file. |
| **Generate Palette** | Auto-generate Complementary, Analogous, Triadic, Split, or Mono palettes. |
| **Random Color** | Generate a random color with animated feedback. |
| **Save Palette** | Export all 9 palette colors to clipboard. |
| **Shades** | Open a folder of 9 shade steps from the current color. |
| **Tints** | Open a folder of 9 tint steps from the current color. |
 
**Palette**
| Feature | Description |
| :--- | :--- |
| **Palette Slots** | 9 live color swatches on LCD buttons. Tap to select and copy. |
 
**Presets**
| Feature | Description |
| :--- | :--- |
| **Palette Presets** | 8 built-in themed palettes: Brand, Material, Pastel, Earth, Neon, Ocean, Sunset, Forest. |
 
**Dials**
| Feature | Description |
| :--- | :--- |
| **Hue Wheel** | Rotate to sweep the full 360° color wheel. Press to reset. |
| **Sat / Light** | Dual sliders controlling Saturation and Lightness with gradient indicators. |
| **Palette Scheme** | Scroll to cycle through palette generation algorithms. |
 
**VS Code Integration**
| Feature | Description |
| :--- | :--- |
| **Export CSS** | Export the palette as vanilla CSS custom properties. |
| **Export SCSS** | Export the palette as SCSS variables. |
| **Export Tailwind** | Export the palette as Tailwind config JSON. |
 
**Figma Integration**
| Feature | Description |
| :--- | :--- |
| **Copy Figma Swatches** | Generate color swatches as SVG and copy to clipboard for Figma. |
 
---
 
 
### Installation
 
1. Download the `.lplug4` file from the [Version 1.0 Release](https://github.com/reyan36/ColorBridge/releases/tag/Version-1.0).
2. Double-click the file with Logitech Options+ running.
3. Confirm the install prompt.
4. Find **ColorBridge** in the Plugin Actions sidebar and assign tools to your keys and dials.
---
 
### Technologies
 
- **Core**: Logi Actions SDK, C#, .NET 8
- **UI**: `BitmapBuilder` for real-time LCD graphics
- **System**: Windows Screen Sampling APIs, Global Clipboard
- **Hardware**: Haptic feedback, action symbols, `PluginDynamicFolder`

---

## Developer

- [@Reyan Arshad](https://www.linkedin.com/in/reyan36/)

## License

Distributed under the MIT License. See `LICENSE` for more information.
<div align="center">
  
## Made For Logitech Dev Studio 2026

</div>
