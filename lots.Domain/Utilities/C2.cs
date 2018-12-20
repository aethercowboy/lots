using System;

namespace lots.Utilities
{
    public static class C2
    {
        public static void WriteMessage(string message) => Console.WriteLine(message);

        public static void Write(string message) => Console.Write(message);

        private static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(message);

            Console.ResetColor();
        }

        public static void WriteError(string message) => WriteColor(message, ConsoleColor.Red);
        public static void WriteWarning(string message) => WriteColor(message, ConsoleColor.Yellow);
        public static void WriteDebug(string message) => WriteColor(message, ConsoleColor.Green);

        public static string ReadLine() => Console.ReadLine();
    }
}
