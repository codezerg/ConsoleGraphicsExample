using System.Text;

namespace ConsoleGraphicsExample;

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;

        GraphicsBufferDemo.Run();

        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("\n\nPress any key to exit...");
        Console.ReadKey();
    }
}