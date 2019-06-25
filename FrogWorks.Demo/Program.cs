using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game(320, 240))
            {
                game.Display.SetFixedScale(2);
                game.Run();
            }
        }
    }
}
