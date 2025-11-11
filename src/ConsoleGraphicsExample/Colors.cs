using System;

namespace ConsoleGraphicsExample
{
    /// <summary>
    /// Helper class for working with ANSI 256-color palette.
    /// Provides color constants and conversion utilities.
    /// </summary>
    public static class Colors
    {
        // ===== Standard 16 Colors (0-15) =====

        public const byte Black = 0;
        public const byte Maroon = 1;
        public const byte Green = 2;
        public const byte Olive = 3;
        public const byte Navy = 4;
        public const byte Purple = 5;
        public const byte Teal = 6;
        public const byte Silver = 7;
        public const byte Gray = 8;
        public const byte Red = 9;
        public const byte Lime = 10;
        public const byte Yellow = 11;
        public const byte Blue = 12;
        public const byte Magenta = 13;
        public const byte Cyan = 14;
        public const byte White = 15;

        // ===== Common Named Colors from 216-color cube =====

        // Reds
        public const byte DarkRed = 52;
        public const byte Red1 = 124;
        public const byte Red2 = 160;
        public const byte BrightRed = 196;
        public const byte Crimson = 197;

        // Oranges
        public const byte DarkOrange = 166;
        public const byte Orange = 208;
        public const byte BrightOrange = 214;

        // Yellows
        public const byte DarkYellow = 136;
        public const byte Yellow1 = 184;
        public const byte Yellow2 = 220;
        public const byte BrightYellow = 226;
        public const byte Gold = 178;

        // Greens
        public const byte DarkGreen = 22;
        public const byte Green1 = 28;
        public const byte Green2 = 34;
        public const byte BrightGreen = 46;
        public const byte LimeGreen = 118;
        public const byte SpringGreen = 48;

        // Cyans
        public const byte DarkCyan = 23;
        public const byte Cyan1 = 37;
        public const byte Cyan2 = 43;
        public const byte BrightCyan = 51;
        public const byte Aqua = 45;

        // Blues
        public const byte DarkBlue = 17;
        public const byte Blue1 = 18;
        public const byte Blue2 = 19;
        public const byte Blue3 = 20;
        public const byte BrightBlue = 21;
        public const byte SkyBlue = 117;
        public const byte DeepSkyBlue = 39;

        // Purples & Magentas
        public const byte DarkPurple = 53;
        public const byte Purple1 = 57;
        public const byte Purple2 = 93;
        public const byte BrightPurple = 99;
        public const byte Violet = 177;
        public const byte DarkMagenta = 90;
        public const byte Magenta1 = 165;
        public const byte BrightMagenta = 201;
        public const byte Pink = 205;
        public const byte HotPink = 198;

        // Browns & Tans
        public const byte Brown = 94;
        public const byte DarkBrown = 52;
        public const byte Tan = 180;
        public const byte Beige = 223;

        // ===== Grayscale (232-255) =====

        public const byte Gray0 = 232;   // Darkest
        public const byte Gray1 = 233;
        public const byte Gray2 = 234;
        public const byte Gray3 = 235;
        public const byte Gray4 = 236;
        public const byte Gray5 = 237;
        public const byte Gray6 = 238;
        public const byte Gray7 = 239;
        public const byte Gray8 = 240;
        public const byte Gray9 = 241;
        public const byte Gray10 = 242;
        public const byte Gray11 = 243;
        public const byte Gray12 = 244;
        public const byte Gray13 = 245;
        public const byte Gray14 = 246;
        public const byte Gray15 = 247;
        public const byte Gray16 = 248;
        public const byte Gray17 = 249;
        public const byte Gray18 = 250;
        public const byte Gray19 = 251;
        public const byte Gray20 = 252;
        public const byte Gray21 = 253;
        public const byte Gray22 = 254;
        public const byte Gray23 = 255;  // Lightest

        /// <summary>
        /// Converts RGB values (0-255) to the nearest ANSI 256-color palette index.
        /// </summary>
        /// <param name="r">Red component (0-255)</param>
        /// <param name="g">Green component (0-255)</param>
        /// <param name="b">Blue component (0-255)</param>
        /// <returns>ANSI 256-color palette index (16-255)</returns>
        /// <remarks>
        /// This uses the 216-color cube (6×6×6) portion of the palette.
        /// Each RGB component is quantized to 6 levels: 0, 95, 135, 175, 215, 255.
        ///
        /// Formula: 16 + 36 × r + 6 × g + b
        /// where r, g, b are in range 0-5
        /// </remarks>
        public static byte FromRgb(byte r, byte g, byte b)
        {
            // Convert RGB (0-255) to 6-level quantized values (0-5)
            int r6 = RgbTo6Level(r);
            int g6 = RgbTo6Level(g);
            int b6 = RgbTo6Level(b);

            // Convert to 216-color cube index (16-231)
            return (byte)(16 + 36 * r6 + 6 * g6 + b6);
        }

        /// <summary>
        /// Converts RGB values (0.0-1.0) to the nearest ANSI 256-color palette index.
        /// </summary>
        /// <param name="r">Red component (0.0-1.0)</param>
        /// <param name="g">Green component (0.0-1.0)</param>
        /// <param name="b">Blue component (0.0-1.0)</param>
        /// <returns>ANSI 256-color palette index (16-255)</returns>
        public static byte FromRgb(float r, float g, float b)
        {
            return FromRgb(
                (byte)(Math.Clamp(r, 0f, 1f) * 255),
                (byte)(Math.Clamp(g, 0f, 1f) * 255),
                (byte)(Math.Clamp(b, 0f, 1f) * 255)
            );
        }

        /// <summary>
        /// Returns a grayscale color from the 24-level grayscale ramp (232-255).
        /// </summary>
        /// <param name="level">Grayscale level (0-23, where 0 is darkest and 23 is lightest)</param>
        /// <returns>ANSI 256-color palette index (232-255)</returns>
        public static byte Grayscale(int level)
        {
            return (byte)(232 + Math.Clamp(level, 0, 23));
        }

        /// <summary>
        /// Returns a grayscale color from a normalized value (0.0-1.0).
        /// </summary>
        /// <param name="value">Normalized grayscale value (0.0 = darkest, 1.0 = lightest)</param>
        /// <returns>ANSI 256-color palette index (232-255)</returns>
        public static byte Grayscale(float value)
        {
            int level = (int)(Math.Clamp(value, 0f, 1f) * 23);
            return Grayscale(level);
        }

        /// <summary>
        /// Converts an 8-bit RGB component (0-255) to a 6-level value (0-5).
        /// </summary>
        private static int RgbTo6Level(byte value)
        {
            // Quantization thresholds for 6 levels
            // Levels: 0(0), 95(1), 135(2), 175(3), 215(4), 255(5)
            if (value < 48) return 0;
            if (value < 115) return 1;
            if (value < 155) return 2;
            if (value < 195) return 3;
            if (value < 235) return 4;
            return 5;
        }

        /// <summary>
        /// Creates a color by interpolating between two colors.
        /// </summary>
        /// <param name="color1">Start color</param>
        /// <param name="color2">End color</param>
        /// <param name="t">Interpolation factor (0.0 = color1, 1.0 = color2)</param>
        /// <returns>Interpolated color</returns>
        /// <remarks>
        /// This is a simplified interpolation that works in color index space.
        /// For better results, use RGB interpolation via FromRgb().
        /// </remarks>
        public static byte Lerp(byte color1, byte color2, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return (byte)(color1 + (color2 - color1) * t);
        }

        /// <summary>
        /// Gets a color from the HSV color space.
        /// </summary>
        /// <param name="hue">Hue angle in degrees (0-360)</param>
        /// <param name="saturation">Saturation (0.0-1.0)</param>
        /// <param name="value">Value/Brightness (0.0-1.0)</param>
        /// <returns>ANSI 256-color palette index</returns>
        public static byte FromHsv(float hue, float saturation, float value)
        {
            // Normalize hue to 0-360 range
            hue = hue % 360;
            if (hue < 0) hue += 360;

            saturation = Math.Clamp(saturation, 0f, 1f);
            value = Math.Clamp(value, 0f, 1f);

            // Convert HSV to RGB
            float c = value * saturation;
            float x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            float m = value - c;

            float r, g, b;

            if (hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return FromRgb(r + m, g + m, b + m);
        }

        /// <summary>
        /// Generates a rainbow color based on position.
        /// </summary>
        /// <param name="position">Position in rainbow (0.0-1.0)</param>
        /// <returns>Rainbow color at the specified position</returns>
        public static byte Rainbow(float position)
        {
            float hue = Math.Clamp(position, 0f, 1f) * 360;
            return FromHsv(hue, 1f, 1f);
        }
    }
}
