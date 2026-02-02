# ConsoleGraphicsExample

A high-performance, zero-allocation console graphics rendering example for .NET 9.0. This project demonstrates a `GraphicsBuffer` class that uses pre-formatted ANSI escape sequences for blazingly fast terminal rendering with full 256-color palette support.

**This is a complete, working example project** that you can learn from, modify, and use as a foundation for your own console graphics applications.

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- **Zero-Allocation Rendering**: Pre-formatted ANSI sequences eliminate string allocations during frame rendering
- **256-Color Palette**: Full ANSI 256-color support (16 standard colors + 216 RGB cube + 24 grayscale levels)
- **High Performance**: Single `Console.Write()` call per frame, capable of rendering 100,000+ particles at 30+ FPS
- **Dynamic Resizing**: Automatically adapts to console window size changes with artifact-free rendering
- **Rich Drawing Primitives**: Boxes, lines, rectangles, text, and custom shapes
- **Color Utilities**: RGB/HSV conversion, gradients, rainbow effects, and named color constants
- **Unicode Support**: Box-drawing characters for professional-looking UI elements

## Quick Start

```csharp
using ConsoleGraphicsExample;

// Create a buffer matching console dimensions
var buffer = new GraphicsBuffer(Console.WindowWidth, Console.WindowHeight);

// Clear with dark blue background
buffer.Clear(Colors.DarkBlue, Colors.White);

// Draw a box
buffer.DrawBox(10, 5, 40, 10, Colors.DarkBlue, Colors.BrightCyan, 1);

// Draw some text
buffer.DrawText(15, 7, "Hello, AnsiCanvas!", Colors.DarkBlue, Colors.BrightYellow);

// Render to console
buffer.Draw();
```

## Installation

Clone this example repository and run:

```bash
git clone https://github.com/yourusername/ConsoleGraphicsExample.git
cd ConsoleGraphicsExample
dotnet build
dotnet run
```

The application will run through all demos automatically. Press any key after each demo to continue.

## API Overview

### GraphicsBuffer

The main rendering engine that manages the console buffer.

```csharp
// Create a buffer
var buffer = new GraphicsBuffer(width, height);

// Basic drawing operations
buffer.SetChar(x, y, bgColor, fgColor, character);
buffer.DrawText(x, y, "text", bgColor, fgColor);
buffer.DrawBox(x, y, width, height, bgColor, fgColor, style);
buffer.DrawHorizontalLine(x, y, length, bgColor, fgColor);
buffer.DrawVerticalLine(x, y, length, bgColor, fgColor);
buffer.FillRect(x, y, width, height, bgColor, fgColor, character);

// Clear and render
buffer.Clear(bgColor, fgColor);
buffer.Draw();
```

### Colors Helper

Comprehensive color utilities for the ANSI 256-color palette.

```csharp
// Named color constants
byte color = Colors.BrightRed;
byte color = Colors.DarkBlue;
byte color = Colors.Orange;

// RGB conversion (0-255 or 0.0-1.0)
byte color = Colors.FromRgb(255, 128, 0);
byte color = Colors.FromRgb(1.0f, 0.5f, 0.0f);

// HSV color space
byte color = Colors.FromHsv(hue: 180, saturation: 1.0f, value: 1.0f);

// Rainbow gradient
byte color = Colors.Rainbow(position: 0.5f);

// Grayscale (0-23 or 0.0-1.0)
byte color = Colors.Grayscale(12);
byte color = Colors.Grayscale(0.5f);

// Color interpolation
byte color = Colors.Lerp(Colors.BrightRed, Colors.BrightBlue, 0.5f);
```

## Demos

The project includes several comprehensive demos showcasing different capabilities:

### Basic Shapes Demo
Demonstrates box styles (single/double/ASCII), filled rectangles, lines, and gradient text.

### Color Palette Demo
Showcases the full 256-color palette including standard 16 colors, RGB cube, and grayscale ramp.

### Animated Scene Demo
Features bouncing particles, starfield backgrounds, color bars, and pulsing text at 30 FPS.

### Colors Helper Demo
Displays RGB/HSV conversions, rainbow effects, gradients, and color interpolation.

### Full Screen Demo
Real-time console resize detection with plasma effects, scrolling text, and animated borders. **Try resizing your console window!**

### Performance Benchmark
Renders 100,000 moving particles to measure FPS and cells-per-second throughput.

Run all demos:

```bash
dotnet run
```

## Performance

AnsiCanvas is designed for high-performance console applications:

- **Zero allocations per frame** after initialization
- **100,000 particles @ 30+ FPS** (tested on standard hardware)
- **Single Console.Write() call** to render entire buffer
- **Pre-formatted ANSI sequences** stored in memory for instant updates

Each cell is stored as 23 pre-formatted characters:
```
\x1b[48;5;###m\x1b[38;5;###m#
          │             │   │
          │             │   └─ Character (1 char)
          │             └─ Foreground color (3 chars)
          └─ Background color (3 chars)
```

## Architecture

### GraphicsBuffer
- Pre-formatted ANSI escape sequences stored as `char[][]` arrays
- Dynamic buffer capacity that grows but never shrinks
- Separate logical viewport (Width/Height) and buffer capacity (BufferWidth/BufferHeight)
- Resize detection with artifact-free screen clearing

### Colors
- Named constants for all 256 ANSI colors
- RGB (0-255 or 0.0-1.0) to ANSI conversion via 6×6×6 color cube quantization
- HSV color space support with hue, saturation, and value
- Gradient generation and color interpolation utilities

## Requirements

- .NET 9.0 or later
- Terminal with ANSI 256-color support (most modern terminals)
- UTF-8 encoding support for box-drawing characters

## Terminal Compatibility

Tested and working on:
- Windows Terminal
- Windows Console Host (with ANSI support enabled)
- Most Linux terminals (xterm, gnome-terminal, konsole, etc.)
- macOS Terminal

## What You'll Learn

This example project demonstrates:

- **High-performance rendering techniques** using pre-formatted ANSI sequences
- **Zero-allocation design patterns** for real-time graphics
- **Console window resize handling** with artifact-free clearing
- **ANSI 256-color palette** usage and color space conversions
- **Animation loops** running at 30+ FPS
- **Buffer management** with dynamic capacity growth
- **UTF-8 box-drawing characters** for professional UIs

## Use Cases

Perfect foundation for building:

- **Console Games**: Fast rendering for roguelikes, ASCII games, retro games
- **Terminal User Interfaces (TUIs)**: Dashboard applications, monitoring tools
- **Data Visualization**: Charts, graphs, real-time data displays
- **Educational Tools**: Demonstrating algorithms, simulations
- **System Utilities**: Enhanced CLI tools with rich visual feedback

## Examples

### Simple Animation Loop

```csharp
var buffer = new GraphicsBuffer(80, 25);

for (int frame = 0; frame < 300; frame++)
{
    buffer.Clear(Colors.Black, Colors.White);

    // Animate a bouncing ball
    int x = 10 + (int)(Math.Sin(frame * 0.1) * 30);
    int y = 10 + (int)(Math.Cos(frame * 0.1) * 8);
    byte color = Colors.Rainbow(frame / 300f);

    buffer.SetChar(x, y, color, Colors.White, '●');
    buffer.Draw();

    Thread.Sleep(33); // ~30 FPS
}
```

### Rainbow Text

```csharp
var buffer = new GraphicsBuffer(80, 25);
string text = "RAINBOW TEXT!";

for (int i = 0; i < text.Length; i++)
{
    float position = i / (float)text.Length;
    byte color = Colors.Rainbow(position);
    buffer.SetChar(10 + i, 12, Colors.Black, color, text[i]);
}

buffer.Draw();
```

### Plasma Effect

```csharp
var buffer = new GraphicsBuffer(80, 25);
double time = 0;

for (int y = 0; y < 25; y++)
{
    for (int x = 0; x < 80; x++)
    {
        double value = Math.Sin(x / 8.0 + time);
        value += Math.Sin(y / 8.0 + time);
        value += Math.Sin((x + y) / 16.0 + time);
        value = (value + 3) / 6; // Normalize to 0-1

        byte color = Colors.Rainbow((float)value);
        buffer.SetChar(x, y, color, Colors.Black, '█');
    }
}

buffer.Draw();
time += 0.1;
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- ANSI escape sequences: [Wikipedia - ANSI escape code](https://en.wikipedia.org/wiki/ANSI_escape_code)
- 256-color palette: [256 Colors - Cheat Sheet](https://www.ditig.com/256-colors-cheat-sheet)

---

**Star this repository if you find it useful!**
