﻿using FrogWorks.Demo.Scenes;
using System;

namespace FrogWorks.Demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var runner = new DesktopRunner(256, 224, 2))
            {
                runner.GoTo<MapTest>();
                runner.AllowUserResizing = true;
                runner.IsMouseVisible = true;
                runner.Run();
            }
        }
    }
}
