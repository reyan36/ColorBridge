---
<div align="center">

# ColorBridge
### Precision color workstation for MX Creative Console
[![Version](https://img.shields.io/badge/Version-1.0-blue.svg)](https://github.com/reyan36/ColorBridge/releases/tag/Version-1.0)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Logi Actions](https://img.shields.io/badge/Logi_Actions_SDK-000000?style=flat&logo=logitech&logoColor=white)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[Features](#features) • [Installation](#installation) • [Technologies](#technologies) • [Developer](#developer)

</div>

### Project Overview
ColorBridge is a Logi Actions SDK plugin that transforms the Logitech MX Creative Console into a unified color management workstation. It securely centralizes the fragmented color workflow of designers, brand managers, and developers into a single physical interface. By linking live hardware dials to digital color manipulation, ColorBridge allows you to instantly sample screen pixels, generate precise palettes, push formatted code snippets to Visual Studio Code, and sync seamlessly with Figma canvasses.

---

### Features

| Feature | Description |
| :--- | :--- |
| **Physical Dial Integrations** | Rotate hardware dials to smoothly sweep Hue, Saturation, and Lightness values using custom, live-painted progress bars built into the keypad screens. |
| **Direct Figma Integration** | Push auto-generated, perfectly contrasting palettes directly into Figma Swatch SVGs directly to your clipboard. |
| **VS Code Ready** | Export precise color swatches formatted automatically for Tailwind Config JSON, Vanilla CSS Properties, and SCSS variables without looking away from your canvas. |
| **Screen Pixel Picker** | Extract the exact pixel color from any desktop application with a single keypad tap. |
| **Auto-Palette Generation** | Instantly spin up mathematically perfect Complementary, Analogous, Triadic, Shade, and Tint scale palettes directly at the hardware level. |
| **Live WCAG Checking** | Get immediate AA/AAA pass/fail visual compliance and haptic feedback on your UX accessibility limits directly on your physical device. |
| **Clipboard Format Convert** | Quickly cycle and securely copy the active color to your PC clipboard across HTML HEX, RGB, HSL, and CMYK formats. |

---

### Installation

Installing the ColorBridge plugin takes literally seconds with zero dependencies or manual compiling required.

1. **Get the Release**: Download the pre-compiled `.lplug4` file from the official latest release here: [Version 1.0 Release](https://github.com/reyan36/ColorBridge/releases/tag/Version-1.0)
2. **Install**: Ensure you have Logitech Options+ installed and running. Simply **double-click** the downloaded `.lplug4` file.
3. **Approve**: Logitech Options+ will securely confirm installation of the custom plugin signature. Click **Install**.
4. **Use It**: Open Logitech Options+, navigate to your MX Creative Keypad or Dialpad configuration UI, and find "ColorBridge" located under the Plugin Actions sidebar. Start dragging the folders and tools directly onto your device keys and dials!

---

### Technologies

The application is built leveraging high-performance systems integrated deeply with Logitech hardware:

- **Core Engine**: Logi Actions SDK, C#, .NET 8
- **UI Rendering**: In-memory `BitmapBuilder` for real-time dynamic graphics natively pushed to LCD buttons.
- **System Internals**: Windows Screen Sampling APIs and Global Clipboard abstractions.
- **Hardware Integrations**: Action symbols, haptic feedback hooks, and `PluginDynamicFolder` rendering for multi-page touch interfaces.

---

## Developer

- [@Reyan Arshad](https://www.linkedin.com/in/reyan36/)

## License

Distributed under the MIT License. See `LICENSE` for more information.

## © 2026 ColorBridge All rights reserved
