using System;

namespace ConsoleGraphicsExample
{
    /// <summary>
    /// Demonstrates GraphicsBuffer capabilities including shapes, text, colors, and performance.
    /// </summary>
    internal static class GraphicsBufferDemo
    {
        public static void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Demo 1: Basic shapes and drawing primitives
            BasicShapesDemo();
            Thread.Sleep(3000);

            // Demo 2: Color palette showcase
            ColorPaletteDemo();
            Thread.Sleep(3000);

            // Demo 3: Animated scene
            AnimatedSceneDemo();
            Thread.Sleep(2000);

            // Demo 4: Colors helper showcase
            ColorsHelperDemo();
            Thread.Sleep(3000);

            // Demo 5: Full screen showcase
            FullScreenDemo();
            Thread.Sleep(3000);

            // Demo 6: Performance benchmark
            PerformanceBenchmark();
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Demonstrates basic drawing primitives: boxes, lines, fills, and text.
        /// </summary>
        static void BasicShapesDemo()
        {
            Console.Clear();
            var buffer = new GraphicsBuffer(80, 25);
            buffer.Clear(Colors.DarkBlue, Colors.White);

            // Title
            buffer.DrawText(2, 1, "GraphicsBuffer - Basic Shapes Demo", Colors.DarkBlue, Colors.BrightYellow);

            // Box styles
            buffer.DrawText(5, 4, "Single Line:", Colors.DarkBlue, Colors.White);
            buffer.DrawBox(5, 5, 15, 5, Colors.DarkBlue, Colors.BrightGreen, 0);

            buffer.DrawText(25, 4, "Double Line:", Colors.DarkBlue, Colors.White);
            buffer.DrawBox(25, 5, 15, 5, Colors.DarkBlue, Colors.BrightCyan, 1);

            buffer.DrawText(45, 4, "ASCII:", Colors.DarkBlue, Colors.White);
            buffer.DrawBox(45, 5, 15, 5, Colors.DarkBlue, Colors.BrightYellow, 2);

            // Filled rectangles
            buffer.DrawText(5, 11, "Filled Rectangles:", Colors.DarkBlue, Colors.White);
            byte[] colors = { Colors.BrightRed, Colors.Orange, Colors.BrightYellow, Colors.BrightGreen,
                            Colors.BrightCyan, Colors.BrightBlue, Colors.Purple2, Colors.BrightMagenta };
            for (int i = 0; i < colors.Length; i++)
            {
                buffer.FillRect(5 + i * 4, 13, 3, 2, colors[i], Colors.Black, '█');
            }

            // Lines
            buffer.DrawText(5, 16, "Lines:", Colors.DarkBlue, Colors.White);
            buffer.DrawHorizontalLine(5, 18, 30, Colors.DarkBlue, Colors.BrightRed, '─');
            buffer.DrawHorizontalLine(5, 19, 30, Colors.DarkBlue, Colors.Orange, '═');
            buffer.DrawVerticalLine(40, 16, 5, Colors.DarkBlue, Colors.BrightCyan, '│');
            buffer.DrawVerticalLine(42, 16, 5, Colors.DarkBlue, Colors.BrightGreen, '║');

            // Gradient effect with text using RGB
            buffer.DrawText(45, 16, "Color Gradient:", Colors.DarkBlue, Colors.White);
            string gradientText = "GRADIENT";
            for (int i = 0; i < gradientText.Length; i++)
            {
                float t = i / (float)(gradientText.Length - 1);
                byte color = Colors.FromRgb(1f, t, t * 0.5f); // Red to Yellow gradient
                buffer.SetChar(45 + i, 18, Colors.DarkBlue, color, gradientText[i]);
            }

            // Draw decorative border
            buffer.DrawBox(1, 0, 78, 23, Colors.DarkBlue, Colors.Purple2, 1);

            buffer.Draw();
        }

        /// <summary>
        /// Showcases the full 256-color palette in different arrangements.
        /// </summary>
        static void ColorPaletteDemo()
        {
            Console.Clear();
            var buffer = new GraphicsBuffer(80, 25);
            buffer.Clear(Colors.Black, Colors.White);

            // Title
            buffer.DrawBox(1, 0, 78, 3, Colors.Black, Colors.BrightYellow, 1);
            buffer.DrawText(25, 1, "256-Color Palette Showcase", Colors.Black, Colors.BrightYellow);

            // Standard 16 colors
            buffer.DrawText(2, 4, "Standard 16 colors:", Colors.Black, Colors.White);
            for (byte i = 0; i < 16; i++)
            {
                buffer.FillRect(2 + i * 4, 5, 3, 2, i, i, ' ');
            }

            // 216 color cube (6x6x6 RGB) - compact view using Colors.FromRgb()
            buffer.DrawText(2, 8, "216-color cube (6×6×6 RGB) via Colors.FromRgb():", Colors.Black, Colors.White);

            // The 6 quantization levels in the ANSI 216-color cube
            byte[] rgbLevels = { 0, 95, 135, 175, 215, 255 };

            for (int i = 0; i < 36; i++)
            {
                int row = i / 6;
                int posInRow = i % 6;

                // Decode i into green and blue components (0-5 each)
                int g = i / 6;
                int b = i % 6;

                for (int j = 0; j < 6; j++)
                {
                    // j represents the red component (0-5)
                    int r = j;

                    // Use Colors.FromRgb() to convert RGB triplet to ANSI color
                    byte color = Colors.FromRgb(rgbLevels[r], rgbLevels[g], rgbLevels[b]);

                    int x = 2 + posInRow * 12 + j * 2;
                    int y = 9 + row;
                    buffer.SetChar(x, y, Colors.Black, color, '█');
                    buffer.SetChar(x + 1, y, Colors.Black, color, '█');
                }
            }

            // Grayscale ramp (232-255)
            buffer.DrawText(2, 16, "Grayscale ramp (232-255):", Colors.Black, Colors.White);
            for (int i = 0; i < 24; i++)
            {
                byte color = Colors.Grayscale(i);
                buffer.FillRect(2 + i * 3, 17, 2, 2, color, Colors.White, '█');
            }

            // Color samples with labels using Colors constants
            buffer.DrawText(2, 20, "Named colors:", Colors.Black, Colors.White);
            string[] colorNames = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink" };
            byte[] namedColors = { Colors.BrightRed, Colors.Orange, Colors.BrightYellow, Colors.BrightGreen,
                                 Colors.BrightCyan, Colors.BrightBlue, Colors.Purple2, Colors.BrightMagenta };

            for (int i = 0; i < namedColors.Length; i++)
            {
                buffer.FillRect(2 + i * 9, 21, 7, 2, namedColors[i], Colors.Black, ' ');
                buffer.DrawText(2 + i * 9, 21, colorNames[i], namedColors[i], Colors.Black);
            }

            buffer.Draw();
        }

        /// <summary>
        /// Creates an animated scene with moving elements and color effects.
        /// </summary>
        static void AnimatedSceneDemo()
        {
            Console.Clear();
            var buffer = new GraphicsBuffer(80, 25);

            // Create starfield background
            var random = new Random(42);
            var stars = new (int x, int y, byte brightness)[100];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = (random.Next(0, 80), random.Next(0, 25), (byte)random.Next(240, 256));
            }

            // Animation loop
            for (int frame = 0; frame < 180; frame++)
            {
                // Clear with dark background
                buffer.Clear(16, 15);

                // Title
                buffer.DrawBox(20, 0, 40, 3, 16, 51, 1);
                buffer.DrawText(25, 1, "Animated Scene Demo", 16, 51);

                // Draw starfield
                foreach (var (x, y, brightness) in stars)
                {
                    int starX = (x + frame / 3) % 80;
                    buffer.SetChar(starX, y, 16, brightness, '.');
                }

                // Bouncing ball
                int ballX = 10 + (int)(Math.Abs(Math.Sin(frame * 0.05)) * 60);
                int ballY = 10 + (int)(Math.Abs(Math.Cos(frame * 0.08)) * 10);
                byte ballColor = (byte)(196 + (frame / 3) % 60);

                buffer.FillRect(ballX - 1, ballY - 1, 3, 3, ballColor, 0, '●');

                // Rotating color bars
                for (int i = 0; i < 8; i++)
                {
                    int barY = 5 + i * 2;
                    byte color = (byte)((frame * 2 + i * 20) % 216 + 16);
                    int barLength = 10 + (int)(Math.Sin(frame * 0.1 + i) * 8);
                    buffer.FillRect(2, barY, barLength, 1, color, 0, '▓');
                }

                // Pulsing text
                byte pulseColor = (byte)(196 + (int)(Math.Sin(frame * 0.15) * 30 + 30));
                string text = $"Frame: {frame}/180";
                buffer.DrawText(30, 22, text, 16, pulseColor);

                // Progress bar
                int progress = (frame * 50) / 180;
                buffer.DrawText(2, 23, "Progress: [", 16, 15);
                for (int i = 0; i < 50; i++)
                {
                    if (i < progress)
                    {
                        byte barColor = (byte)(46 + i);
                        buffer.SetChar(14 + i, 23, 16, barColor, '█');
                    }
                    else
                    {
                        buffer.SetChar(14 + i, 23, 16, 240, '░');
                    }
                }
                buffer.DrawText(65, 23, "]", 16, 15);

                buffer.Draw();
                Thread.Sleep(33); // ~30 FPS
            }
        }

        /// <summary>
        /// Demonstrates Colors helper class features including RGB, HSV, gradients, and rainbow effects.
        /// </summary>
        static void ColorsHelperDemo()
        {
            Console.Clear();
            var buffer = new GraphicsBuffer(80, 25);
            buffer.Clear(Colors.Black, Colors.White);

            // Title
            buffer.DrawBox(1, 0, 78, 3, Colors.Black, Colors.BrightCyan, 1);
            buffer.DrawText(25, 1, "Colors Helper Demo", Colors.Black, Colors.BrightCyan);

            // RGB color conversion
            buffer.DrawText(2, 4, "RGB to ANSI Colors (FromRgb):", Colors.Black, Colors.White);
            for (int i = 0; i < 10; i++)
            {
                float t = i / 9f;
                byte color = Colors.FromRgb(t, 0.5f, 1f - t); // Blue to Red gradient
                buffer.FillRect(2 + i * 7, 5, 6, 2, color, Colors.Black, '█');
            }

            // HSV rainbow
            buffer.DrawText(2, 8, "HSV Rainbow (FromHsv):", Colors.Black, Colors.White);
            for (int i = 0; i < 60; i++)
            {
                float hue = (i / 60f) * 360f;
                byte color = Colors.FromHsv(hue, 1f, 1f);
                buffer.SetChar(2 + i, 9, Colors.Black, color, '█');
            }

            // Rainbow helper
            buffer.DrawText(2, 11, "Rainbow Helper:", Colors.Black, Colors.White);
            for (int i = 0; i < 60; i++)
            {
                byte color = Colors.Rainbow(i / 60f);
                buffer.SetChar(2 + i, 12, Colors.Black, color, '█');
            }

            // Grayscale gradient
            buffer.DrawText(2, 14, "Grayscale Gradient:", Colors.Black, Colors.White);
            for (int i = 0; i < 24; i++)
            {
                byte color = Colors.Grayscale(i);
                buffer.FillRect(2 + i * 3, 15, 2, 2, color, Colors.White, '█');
            }

            // Named color constants
            buffer.DrawText(2, 18, "Named Color Constants:", Colors.Black, Colors.White);
            buffer.DrawText(2, 19, "Red", Colors.Black, Colors.BrightRed);
            buffer.DrawText(10, 19, "Orange", Colors.Black, Colors.Orange);
            buffer.DrawText(20, 19, "Yellow", Colors.Black, Colors.BrightYellow);
            buffer.DrawText(30, 19, "Green", Colors.Black, Colors.BrightGreen);
            buffer.DrawText(40, 19, "Cyan", Colors.Black, Colors.BrightCyan);
            buffer.DrawText(48, 19, "Blue", Colors.Black, Colors.BrightBlue);
            buffer.DrawText(56, 19, "Purple", Colors.Black, Colors.Purple2);
            buffer.DrawText(66, 19, "Pink", Colors.Black, Colors.Pink);

            // Color interpolation
            buffer.DrawText(2, 21, "Color Lerp (Red -> Blue):", Colors.Black, Colors.White);
            for (int i = 0; i < 40; i++)
            {
                float t = i / 39f;
                byte color = Colors.Lerp(Colors.BrightRed, Colors.BrightBlue, t);
                buffer.SetChar(2 + i, 22, Colors.Black, color, '█');
            }

            buffer.Draw();
        }

        /// <summary>
        /// Full screen demo showcasing GraphicsBuffer with dynamic console dimensions.
        /// Features: plasma effect, scrolling text, animated borders, and color cycling.
        /// RESIZABLE: Adapts to console window size changes in real-time.
        /// </summary>
        static void FullScreenDemo()
        {
            Console.Clear();

            // Get initial console dimensions
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            var buffer = new GraphicsBuffer(width, height);

            // Animation parameters
            var random = new Random(42);
            double time = 0;
            const int duration = 300; // frames

            for (int frame = 0; frame < duration; frame++)
            {
                time += 0.05;

                // Check for console resize
                try
                {
                    int currentWidth = Console.WindowWidth;
                    int currentHeight = Console.WindowHeight;

                    if (currentWidth != width || currentHeight != height)
                    {
                        // Console was resized - recreate buffer
                        width = currentWidth;
                        height = currentHeight;
                        buffer = new GraphicsBuffer(width, height);
                        // Screen clearing now handled by Draw() method
                    }
                }
                catch
                {
                    // Ignore resize detection errors (can happen on some terminals)
                }

                // Center coordinates (recalculated each frame for resize support)
                int centerX = width / 2;
                int centerY = height / 2;

                // Create plasma effect background
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Plasma calculation using sine waves
                        double value = Math.Sin(x / 8.0 + time);
                        value += Math.Sin(y / 8.0 + time);
                        value += Math.Sin((x + y) / 16.0 + time);
                        value += Math.Sin(Math.Sqrt(x * x + y * y) / 8.0 + time);
                        value = (value + 4) / 8; // Normalize to 0-1

                        // Convert to rainbow color
                        byte color = Colors.Rainbow((float)value);
                        char patternChar = " ░▒▓█"[(int)(value * 4.999)];

                        buffer.SetChar(x, y, color, Colors.Black, patternChar);
                    }
                }

                // Draw animated border
                byte borderColor = Colors.FromHsv((frame * 2) % 360, 1f, 1f);
                buffer.DrawBox(0, 0, width, height, Colors.Black, borderColor, 1);

                // Draw inner decorative frame
                if (width > 10 && height > 6)
                {
                    byte innerBorderColor = Colors.FromHsv((frame * 2 + 180) % 360, 1f, 1f);
                    buffer.DrawBox(2, 2, width - 4, height - 4, Colors.Black, innerBorderColor, 0);
                }

                // Title background panel
                if (width > 40 && height > 10)
                {
                    int panelWidth = 50;
                    int panelHeight = 8;
                    int panelX = Math.Max(0, centerX - panelWidth / 2);
                    int panelY = Math.Max(0, centerY - panelHeight / 2);

                    // Draw semi-transparent panel (using dark color)
                    buffer.FillRect(panelX, panelY, panelWidth, panelHeight, Colors.Gray2, Colors.White, ' ');
                    buffer.DrawBox(panelX, panelY, panelWidth, panelHeight, Colors.Gray2, Colors.BrightCyan, 1);

                    // Animated title
                    string title = "RESIZABLE FULL SCREEN DEMO";
                    int titleX = Math.Max(0, centerX - title.Length / 2);
                    int titleY = centerY - 2;

                    for (int i = 0; i < title.Length; i++)
                    {
                        float colorPos = (float)((time * 0.5 + i * 0.1) % 1.0);
                        byte charColor = Colors.Rainbow(colorPos);
                        buffer.SetChar(titleX + i, titleY, Colors.Gray2, charColor, title[i]);
                    }

                    // Info text
                    string info = $"Resolution: {width}×{height} (try resizing!)";
                    int infoX = Math.Max(0, centerX - info.Length / 2);
                    buffer.DrawText(infoX, centerY, info, Colors.Gray2, Colors.BrightYellow);

                    string frameText = $"Frame: {frame}/{duration}";
                    int frameX = Math.Max(0, centerX - frameText.Length / 2);
                    buffer.DrawText(frameX, centerY + 2, frameText, Colors.Gray2, Colors.BrightGreen);
                }

                // Scrolling text at bottom
                if (height > 3)
                {
                    string scrollText = "★ RESIZABLE in real-time ★ GraphicsBuffer adapts to any console size ★ High-performance rendering ★ 256-color palette ★ ";
                    int scrollOffset = (frame * 2) % scrollText.Length;
                    int bottomY = height - 2;

                    for (int i = 0; i < width && i < scrollText.Length; i++)
                    {
                        int textIndex = (scrollOffset + i) % scrollText.Length;
                        float colorPos = (float)(i / (float)width);
                        byte textColor = Colors.Rainbow(colorPos);
                        buffer.SetChar(i, bottomY, Colors.Black, textColor, scrollText[textIndex]);
                    }
                }

                // Animated corner indicators
                byte cornerColor = Colors.FromHsv((frame * 3) % 360, 1f, 1f);

                // Top-left
                if (width > 5 && height > 3)
                {
                    buffer.DrawText(3, 1, $"┌─", Colors.Black, cornerColor);
                }

                // Top-right
                if (width > 5 && height > 3)
                {
                    buffer.DrawText(width - 5, 1, $"─┐", Colors.Black, cornerColor);
                }

                // Bottom-left
                if (width > 5 && height > 3)
                {
                    buffer.DrawText(3, height - 2, $"└─", Colors.Black, cornerColor);
                }

                // Bottom-right
                if (width > 5 && height > 3)
                {
                    buffer.DrawText(width - 5, height - 2, $"─┘", Colors.Black, cornerColor);
                }

                // Floating particles
                for (int i = 0; i < 20; i++)
                {
                    double angle = time * 0.5 + i * Math.PI * 2 / 20;
                    int particleX = (int)(centerX + Math.Cos(angle) * (width / 4));
                    int particleY = (int)(centerY + Math.Sin(angle) * (height / 4));

                    if (particleX >= 0 && particleX < width && particleY >= 0 && particleY < height)
                    {
                        byte particleColor = Colors.FromHsv((float)((angle * 180 / Math.PI) % 360), 1f, 1f);
                        buffer.SetChar(particleX, particleY, Colors.Black, particleColor, '●');
                    }
                }

                buffer.Draw();
                Thread.Sleep(33); // ~30 FPS
            }

            // Final screen
            buffer.Clear(Colors.Black, Colors.White);
            buffer.DrawBox(0, 0, width, height, Colors.Black, Colors.BrightGreen, 1);

            // Recalculate center for final screen
            int finalCenterX = width / 2;
            int finalCenterY = height / 2;

            string[] messages = {
                "Resizable Full Screen Demo Complete!",
                "",
                $"Final Console Size: {width} × {height}",
                $"Total Cells: {width * height:N0}",
                "",
                "GraphicsBuffer adapts to any console dimensions",
                "Resize detection: Real-time buffer recreation"
            };

            int startY = Math.Max(0, finalCenterY - messages.Length / 2);
            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Length > 0)
                {
                    int msgX = Math.Max(0, finalCenterX - messages[i].Length / 2);
                    byte color = i == 0 ? Colors.BrightYellow : Colors.White;
                    buffer.DrawText(msgX, startY + i, messages[i], Colors.Black, color);
                }
            }

            buffer.Draw();
        }

        /// <summary>
        /// Performance benchmark - tests rendering speed with many moving particles.
        /// </summary>
        static void PerformanceBenchmark()
        {
            Console.Clear();
            var buffer = new GraphicsBuffer(80, 25);

            // Setup screen
            buffer.Clear(17, 15);
            buffer.DrawBox(0, 0, 80, 8, 17, 51, 1);
            buffer.DrawText(2, 1, "GraphicsBuffer Performance Benchmark", 17, 226);
            buffer.DrawText(2, 3, "Rendering thousands of colored particles...", 17, 46);
            buffer.Draw();
            Thread.Sleep(1000);

            // Initialize particles
            var random = new Random();
            const int particleCount = 100000; // 100k particles
            var particles = new (int x, int y, int dx, int dy, byte color)[particleCount];

            for (int i = 0; i < particleCount; i++)
            {
                particles[i] = (
                    random.Next(0, 80),
                    random.Next(9, 24),
                    random.Next(-1, 2),
                    random.Next(-1, 2),
                    (byte)random.Next(16, 232)
                );
            }

            // Animation loop
            var frameCount = 0;
            var startTime = DateTime.Now;
            const int maxFrames = 500;

            for (int frame = 0; frame < maxFrames; frame++)
            {
                // Clear particle area (preserve header)
                buffer.FillRect(0, 9, 80, 16, 17, 15, ' ');

                // Update and draw particles
                for (int i = 0; i < particleCount; i++)
                {
                    var (x, y, dx, dy, color) = particles[i];

                    // Move particle
                    x += dx;
                    y += dy;

                    // Bounce off walls
                    if (x <= 0 || x >= 79)
                    {
                        dx = -dx;
                        x = Math.Clamp(x, 0, 79);
                    }
                    if (y <= 9 || y >= 23)
                    {
                        dy = -dy;
                        y = Math.Clamp(y, 9, 23);
                    }

                    particles[i] = (x, y, dx, dy, color);

                    // Draw particle
                    buffer.SetChar(x, y, 17, color, '•');
                }

                // Update stats
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                var fps = elapsed > 0 ? frameCount / elapsed : 0;
                var cellsPerSecond = fps * particleCount;

                buffer.DrawText(2, 5, $"FPS: {fps:F0}    ", 17, 46);
                buffer.DrawText(2, 6, $"Frame: {frame}/{maxFrames}    ", 17, 51);
                buffer.DrawText(40, 5, $"Particles: {particleCount:N0}    ", 17, 226);
                buffer.DrawText(40, 6, $"Cells/sec: {cellsPerSecond:N0}    ", 17, 208);

                buffer.Draw();
                frameCount++;
            }

            // Final results
            var totalTime = (DateTime.Now - startTime).TotalSeconds;
            var finalFps = frameCount / totalTime;

            buffer.Clear(0, 15);
            buffer.DrawBox(10, 5, 60, 15, 0, 46, 1);

            buffer.DrawText(15, 7, "Performance Benchmark Complete!", 0, 226);
            buffer.DrawText(15, 9, $"Total Frames:    {frameCount}", 0, 15);
            buffer.DrawText(15, 10, $"Total Time:      {totalTime:F2} seconds", 0, 15);
            buffer.DrawText(15, 11, $"Average FPS:     {finalFps:F0}", 0, 46);
            buffer.DrawText(15, 12, $"Total Particles: {particleCount:N0}", 0, 51);
            buffer.DrawText(15, 13, $"Cells Updated:   {(long)frameCount * particleCount:N0}", 0, 51);

            var cellsPerSec = ((long)frameCount * particleCount) / totalTime;
            buffer.DrawText(15, 15, $"Cells per Second: {cellsPerSec:N0}", 0, 208);

            buffer.DrawText(15, 17, "GraphicsBuffer uses pre-formatted ANSI codes", 0, 240);
            buffer.DrawText(15, 18, "for zero-allocation rendering.", 0, 240);

            buffer.Draw();
        }
    }
}
