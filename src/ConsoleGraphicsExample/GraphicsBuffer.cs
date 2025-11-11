using System;

namespace ConsoleGraphicsExample
{
    /// <summary>
    /// High-performance graphics buffer for console applications.
    /// Stores pre-formatted ANSI escape sequences for maximum rendering speed.
    /// Each cell contains: background color (256-color) + foreground color (256-color) + character.
    /// </summary>
    /// <remarks>
    /// Performance characteristics:
    /// - Zero allocations per frame after initialization
    /// - Single Console.Write() call to render entire buffer
    /// - Direct memory updates for cell modifications
    /// - Ideal for applications with thousands of updates per frame
    /// </remarks>
    public class GraphicsBuffer
    {
        private char[][] _lineBuffers;
        private int _width;
        private int _height;
        private int _bufferWidth;   // Actual allocated buffer width (capacity)
        private int _bufferHeight;  // Actual allocated buffer height (capacity)
        private int _lastConsoleWidth = -1;  // Track console size for resize detection
        private int _lastConsoleHeight = -1;

        // ANSI escape sequence format: "\x1b[48;5;###m\x1b[38;5;###m#"
        // Background: \x1b[48;5;###m (12 chars)
        // Foreground: \x1b[38;5;###m (12 chars)
        // Character: # (1 char)
        // Total: 23 chars per cell
        private const int CellSize = 23;
        private const string CellTemplate = "\x1b[48;5;000m\x1b[38;5;000m ";

        /// <summary>
        /// Gets the logical width of the buffer in characters (what will be drawn).
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the logical height of the buffer in characters (what will be drawn).
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Gets the actual allocated buffer width (capacity).
        /// </summary>
        public int BufferWidth => _bufferWidth;

        /// <summary>
        /// Gets the actual allocated buffer height (capacity).
        /// </summary>
        public int BufferHeight => _bufferHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsBuffer"/> class.
        /// </summary>
        /// <param name="width">Initial width in characters. If 0, buffer starts empty.</param>
        /// <param name="height">Initial height in characters. If 0, buffer starts empty.</param>
        public GraphicsBuffer(int width = 0, int height = 0)
        {
            _width = 0;
            _height = 0;
            _bufferWidth = 0;
            _bufferHeight = 0;
            _lineBuffers = Array.Empty<char[]>();

            if (width > 0 && height > 0)
            {
                EnsureSize(width, height);
            }
        }

        /// <summary>
        /// Ensures the buffer is at least the specified size, expanding if necessary.
        /// New cells are initialized with black background (0) and white foreground (15).
        /// </summary>
        /// <param name="width">Desired logical width in characters.</param>
        /// <param name="height">Desired logical height in characters.</param>
        /// <remarks>
        /// This method preserves existing content when expanding the buffer.
        /// The buffer never shrinks - it only grows. The logical size (Width/Height) can be
        /// smaller than the buffer capacity, but the actual allocated buffers remain large.
        /// </remarks>
        public void EnsureSize(int width, int height)
        {
            // Update logical size (can shrink)
            _width = width;
            _height = height;

            // Only grow buffer capacity if needed (never shrink)
            bool needsWidthGrowth = width > _bufferWidth;
            bool needsHeightGrowth = height > _bufferHeight;

            if (!needsWidthGrowth && !needsHeightGrowth)
                return; // Buffer is already large enough

            int newBufferWidth = Math.Max(width, _bufferWidth);
            int newBufferHeight = Math.Max(height, _bufferHeight);
            int lineSize = newBufferWidth * CellSize;

            var newLineBuffers = new char[newBufferHeight][];

            // Initialize or copy each line buffer
            for (int y = 0; y < newBufferHeight; y++)
            {
                // If we need width growth or this is a new line, allocate new line buffer
                if (needsWidthGrowth || y >= _bufferHeight)
                {
                    newLineBuffers[y] = new char[lineSize];

                    // Initialize all cells in this line
                    for (int x = 0; x < newBufferWidth; x++)
                    {
                        int writePos = x * CellSize;

                        // Copy old data if it exists
                        if (x < _bufferWidth && y < _bufferHeight)
                        {
                            Array.Copy(_lineBuffers[y], x * CellSize, newLineBuffers[y], writePos, CellSize);
                        }
                        else
                        {
                            // Initialize with template: bgColor=0 (black), fgColor=15 (white)
                            CellTemplate.CopyTo(0, newLineBuffers[y], writePos, CellTemplate.Length);
                        }
                    }
                }
                else
                {
                    // Reuse existing line buffer (no width growth, line exists)
                    newLineBuffers[y] = _lineBuffers[y];
                }
            }

            _lineBuffers = newLineBuffers;
            _bufferWidth = newBufferWidth;
            _bufferHeight = newBufferHeight;
        }

        /// <summary>
        /// Gets the starting position within a line buffer for a cell at column x.
        /// </summary>
        private int GetCellPosition(int x)
        {
            return x * CellSize;
        }

        /// <summary>
        /// Sets a character at the specified position with background and foreground colors.
        /// </summary>
        /// <param name="x">X coordinate (0-based, left to right).</param>
        /// <param name="y">Y coordinate (0-based, top to bottom).</param>
        /// <param name="bgColor">Background color (0-255 ANSI 256-color palette).</param>
        /// <param name="fgColor">Foreground color (0-255 ANSI 256-color palette).</param>
        /// <param name="character">Character to display.</param>
        /// <remarks>
        /// Color palette reference:
        /// - 0-15: Standard colors (black, red, green, yellow, blue, magenta, cyan, white, and bright variants)
        /// - 16-231: 6×6×6 RGB color cube
        /// - 232-255: Grayscale from dark to light
        /// </remarks>
        public void SetChar(int x, int y, byte bgColor, byte fgColor, char character)
        {
            // Check against buffer capacity (not logical size)
            if (x < 0 || x >= _bufferWidth || y < 0 || y >= _bufferHeight)
                return;

            int pos = GetCellPosition(x);
            char[] lineBuffer = _lineBuffers[y];

            // Format: "\x1b[48;5;###m\x1b[38;5;###m#"
            // Position breakdown:
            // 0-6:   \x1b[48;5;
            // 7-9:   ### (background color digits)
            // 10:    m
            // 11-17: \x1b[38;5;
            // 18-20: ### (foreground color digits)
            // 21:    m
            // 22:    # (character)

            WriteColorValue(lineBuffer, pos + 7, bgColor);
            WriteColorValue(lineBuffer, pos + 18, fgColor);
            lineBuffer[pos + 22] = character;
        }

        /// <summary>
        /// Writes a 3-digit color value (000-255) at the specified position in the buffer.
        /// </summary>
        private static void WriteColorValue(char[] buffer, int position, byte value)
        {
            buffer[position] = (char)('0' + (value / 100));
            buffer[position + 1] = (char)('0' + ((value / 10) % 10));
            buffer[position + 2] = (char)('0' + (value % 10));
        }

        /// <summary>
        /// Gets the character at the specified position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>The character at (x, y), or space if out of bounds.</returns>
        public char GetChar(int x, int y)
        {
            // Check against buffer capacity (not logical size)
            if (x < 0 || x >= _bufferWidth || y < 0 || y >= _bufferHeight)
                return ' ';

            int pos = GetCellPosition(x);
            return _lineBuffers[y][pos + 22];
        }

        /// <summary>
        /// Renders the buffer to the console.
        /// Only renders the logical viewport (Width x Height), not the entire buffer capacity.
        /// </summary>
        /// <remarks>
        /// The cursor is positioned at (0, 0) before rendering.
        /// ANSI colors are reset after rendering.
        /// Only the logical size is drawn, even if the buffer has larger capacity.
        /// </remarks>
        public void Draw()
        {
            int currentConsoleWidth = Console.WindowWidth;
            int currentConsoleHeight = Console.WindowHeight;

            this.EnsureSize(currentConsoleWidth, currentConsoleHeight);

            // Detect if console was resized (especially resized down)
            bool consoleResized = (_lastConsoleWidth != -1 && _lastConsoleHeight != -1) &&
                                 (currentConsoleWidth != _lastConsoleWidth || currentConsoleHeight != _lastConsoleHeight);
            bool resizedDown = consoleResized &&
                              (currentConsoleWidth < _lastConsoleWidth || currentConsoleHeight < _lastConsoleHeight);

            //Console.SetCursorPosition(0, 0);

            Console.Out.Write("\x1b[0m"); // Reset colors
            Console.Out.Write("\x1b[?25l"); // Hide cursor
            Console.Out.Write("\x1b[?7l"); // Disable line wrapping

            // Only clear screen when console is resized (especially when resized down to prevent artifacts)
            if (consoleResized)
            {
                Console.Out.Write("\x1b[2J"); // Clear entire screen (reduces resize artifacts)
                Console.Out.Write("\x1b[H"); // Move cursor to home position
            }

            // Write only the logical viewport (not the entire buffer capacity)
            for (int y = 0; y < _height; y++)
            {
                int width = Math.Min(_width, currentConsoleWidth);
                int drawWidth = width * CellSize; // Number of chars to draw per line

                Console.Out.Write($"\u001b[{y + 1};0H");

                // Write only the portion of the line buffer that's in the logical viewport
                Console.Out.Write(_lineBuffers[y], 0, drawWidth);

                // Clear to end of line to remove artifacts when resizing down
                if (resizedDown)
                {
                    Console.Out.Write("\x1b[K");
                }
            }

            // Reset colors
            Console.Out.Write("\x1b[0m");

            // Update tracking variables
            _lastConsoleWidth = currentConsoleWidth;
            _lastConsoleHeight = currentConsoleHeight;
        }

        /// <summary>
        /// Clears the buffer by filling it with spaces.
        /// </summary>
        /// <param name="bgColor">Background color to use (default: 0 = black).</param>
        /// <param name="fgColor">Foreground color to use (default: 15 = white).</param>
        public void Clear(byte bgColor = 0, byte fgColor = 15)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    SetChar(x, y, bgColor, fgColor, ' ');
                }
            }
        }

        /// <summary>
        /// Fills a rectangular region with a character and colors.
        /// </summary>
        /// <param name="x">Starting X coordinate.</param>
        /// <param name="y">Starting Y coordinate.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="fgColor">Foreground color.</param>
        /// <param name="character">Character to fill with.</param>
        public void FillRect(int x, int y, int width, int height, byte bgColor, byte fgColor, char character)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    SetChar(x + col, y + row, bgColor, fgColor, character);
                }
            }
        }

        /// <summary>
        /// Draws a horizontal line.
        /// </summary>
        /// <param name="x">Starting X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="length">Length of the line.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="fgColor">Foreground color.</param>
        /// <param name="character">Character to use for the line (default: '─').</param>
        public void DrawHorizontalLine(int x, int y, int length, byte bgColor, byte fgColor, char character = '─')
        {
            for (int i = 0; i < length; i++)
            {
                SetChar(x + i, y, bgColor, fgColor, character);
            }
        }

        /// <summary>
        /// Draws a vertical line.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Starting Y coordinate.</param>
        /// <param name="length">Length of the line.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="fgColor">Foreground color.</param>
        /// <param name="character">Character to use for the line (default: '│').</param>
        public void DrawVerticalLine(int x, int y, int length, byte bgColor, byte fgColor, char character = '│')
        {
            for (int i = 0; i < length; i++)
            {
                SetChar(x, y + i, bgColor, fgColor, character);
            }
        }

        /// <summary>
        /// Draws a text string at the specified position.
        /// </summary>
        /// <param name="x">Starting X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="text">Text to draw.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="fgColor">Foreground color.</param>
        public void DrawText(int x, int y, string text, byte bgColor, byte fgColor)
        {
            for (int i = 0; i < text.Length; i++)
            {
                SetChar(x + i, y, bgColor, fgColor, text[i]);
            }
        }

        /// <summary>
        /// Draws a box with borders.
        /// </summary>
        /// <param name="x">Starting X coordinate.</param>
        /// <param name="y">Starting Y coordinate.</param>
        /// <param name="width">Width of the box.</param>
        /// <param name="height">Height of the box.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="fgColor">Foreground color.</param>
        /// <param name="style">Border style: 0=single line, 1=double line, 2=ASCII (default: 0).</param>
        public void DrawBox(int x, int y, int width, int height, byte bgColor, byte fgColor, int style = 0)
        {
            if (width < 2 || height < 2) return;

            char topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical;

            switch (style)
            {
                case 1: // Double line
                    topLeft = '╔'; topRight = '╗'; bottomLeft = '╚'; bottomRight = '╝';
                    horizontal = '═'; vertical = '║';
                    break;
                case 2: // ASCII
                    topLeft = '+'; topRight = '+'; bottomLeft = '+'; bottomRight = '+';
                    horizontal = '-'; vertical = '|';
                    break;
                default: // Single line
                    topLeft = '┌'; topRight = '┐'; bottomLeft = '└'; bottomRight = '┘';
                    horizontal = '─'; vertical = '│';
                    break;
            }

            // Corners
            SetChar(x, y, bgColor, fgColor, topLeft);
            SetChar(x + width - 1, y, bgColor, fgColor, topRight);
            SetChar(x, y + height - 1, bgColor, fgColor, bottomLeft);
            SetChar(x + width - 1, y + height - 1, bgColor, fgColor, bottomRight);

            // Horizontal lines
            for (int i = 1; i < width - 1; i++)
            {
                SetChar(x + i, y, bgColor, fgColor, horizontal);
                SetChar(x + i, y + height - 1, bgColor, fgColor, horizontal);
            }

            // Vertical lines
            for (int i = 1; i < height - 1; i++)
            {
                SetChar(x, y + i, bgColor, fgColor, vertical);
                SetChar(x + width - 1, y + i, bgColor, fgColor, vertical);
            }
        }
    }
}
