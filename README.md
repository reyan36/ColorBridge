# ColorBridge
**Turn your MX Creative Console into a Color Studio**

## 🎨 Overview
ColorBridge is a Logi Actions SDK plugin concept that transforms the MX Creative Console into a complete color management workstation for designers and developers.

## 🚀 Live Prototype
[View Interactive Prototype](prototype.html)

## ❓ The Problem
Designers and developers are constantly switching between different tools just to work with colors:
- Browser extensions for picking colors from screen
- Separate websites to check accessibility standards
- Online converters for HEX/RGB/HSL formats
- Different apps to manage color palettes

It's messy, inefficient, and breaks your workflow.

## ✨ The Solution
ColorBridge unifies the entire color workflow into one hardware device:

- **9 LCD Buttons** - Display live color swatches with HEX codes. Press to copy.
- **Main Dial** - Physical color wheel controlling Hue (0-360°)
- **Sub-Dial** - Adjust Saturation and Lightness with real-time feedback
- **Actions Ring** - 8 quick actions: Screen Picker, Format Paste, Contrast Check, Palette Generator
- **Haptic Feedback** - Feel when colors fail accessibility standards through your mouse

**Works everywhere:** Figma, Photoshop, VS Code, browsers—no app-specific plugins needed.

## 🎯 Key Features

### Hardware Integration
- Physical color wheel interaction via dial
- Real-time WCAG AA/AAA contrast checking
- Multi-format clipboard output (HEX, RGB, HSL, CMYK)
- Visual palette management on LCD buttons
- Auto-generate color schemes (Complementary, Analogous, Triadic)
- Universal tool that works across all applications

### Innovation
- **First hardware color tool** for Logitech ecosystem
- **Haptic accessibility feedback** - Feel compliance through your mouse
- **Dial-as-color-wheel** - Intuitive and unprecedented interaction
- **No app plugins needed** - Universal workflow

## 🛠️ Technical Approach

Built with **Logi Actions SDK (C#)**:
- `PluginDynamicCommand` - Button actions and color copying
- `PluginDynamicAdjustment` - Dial-based HSL controls
- Dynamic LCD image generation for real-time swatches
- Haptics API for tactile accessibility feedback
- WCAG contrast calculation algorithms
- Palette generation algorithms
- System-wide clipboard integration

## 👥 Target Audience
- UI/UX Designers managing brand colors and design systems
- Front-end Developers working with CSS and color variables
- Brand Managers ensuring accessibility compliance
- Digital Artists requiring precise color control
- Anyone who works with colors across multiple applications daily

## 📋 Development Roadmap

If selected for Semi-Finals:
1. ✅ Develop core color picker and dial integration
2. ✅ Implement dynamic LCD swatch rendering
3. ✅ Build haptic feedback system for accessibility
4. ✅ Create palette generation algorithms
5. ✅ Test across multiple operating systems and applications
6. ✅ Refine UX based on real-world testing

## 🎥 Demo Video
[Watch on YouTube](#) *(Coming soon)*

## 📄 Documentation
See [ColorBridge Documentation.docx](ColorBridge%20Documentation.docx) for detailed concept overview.

## 📸 Screenshots
*(Screenshots will be added as development progresses)*

## 🏆 Competition
**Logitech DevStudio Challenge 2026** - Application Phase Submission

## 📝 License
MIT License - See [LICENSE](LICENSE) file for details

## 👨‍💻 Developer
Reyan Arshad

---

**Built for the Logitech DevStudio Challenge 2026**
```

---