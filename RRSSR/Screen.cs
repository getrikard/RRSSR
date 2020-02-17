using System;

namespace RRSSR
{
    class Screen
    {
        private static void DrawBorder()
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;

            Console.BackgroundColor = Settings.FgColor;

            for (int left = 0; left < w; left++)
            {
                Console.SetCursorPosition(left, 0);
                Console.Write(' ');
                Console.SetCursorPosition(left, h - 1);
                Console.Write(' ');
            }

            for (int top = 0; top < h; top++)
            {
                Console.SetCursorPosition(0, top);
                Console.Write("  ");
                Console.SetCursorPosition(w - 2, top);
                Console.Write("  ");
            }

            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = Settings.BgColor;
        }
    }
}
