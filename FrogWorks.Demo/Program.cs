using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game(640, 480))
            {
                game.Run();
            }
        }
    }
}
