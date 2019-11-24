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
                runner.AllowUserResizing = true;
                runner.IsMouseVisible = true;
                runner.Run();
            }
        }
    }
}
