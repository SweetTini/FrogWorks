using FrogWorks.Demo.Scenes;
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
                game.Window.AllowUserResizing = true;
                game.IsMouseVisible = true;
                game.SetScene<TestScene>();
                game.Run();
            }
        }
    }
}
