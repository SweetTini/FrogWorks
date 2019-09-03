using FrogWorks.Demo.Scenes;
using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var runner = new DesktopRunner(320, 240, 2))
            {
                runner.GoTo<TestScene>();
                runner.AllowUserResizing = true;
                runner.Run();
            }
        }
    }
}
