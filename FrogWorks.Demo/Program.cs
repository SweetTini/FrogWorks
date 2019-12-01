using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var runner = new DesktopRunner(160, 144, 3))
            {
                runner.GoTo<GraphicsTestSceneA>();
                runner.AllowUserResizing = true;
                runner.IsMouseVisible = true;
                runner.Run();
            }
        }
    }
}
