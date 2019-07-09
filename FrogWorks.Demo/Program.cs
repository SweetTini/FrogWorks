using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Engine(320, 240))
            {
                game.SetScene<CoreTestScene>();
                game.Window.AllowUserResizing = true;
                game.IsMouseVisible = true;
                game.Run();
            }
        }
    }
}
