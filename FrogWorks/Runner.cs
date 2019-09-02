using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;

namespace FrogWorks
{
    public abstract class Runner : IRunner
    {
        public static Runner Application { get; private set; }

        protected internal GameAdapter Game { get; private set; }

        public Version Version { get; set; } = new Version(0, 0, 1);

        public virtual string AssemblyDirectory
            => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Game.Content.RootDirectory); }
            set { Game.Content.RootDirectory = value; }        
        }

        public Color ClearColor
        {
            get { return Game.Display.ClearColor; }
            set { Game.Display.ClearColor = value; }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int FramesPerSecond => Game.FramesPerSecond;

        public bool IsDisposed { get; protected set; }

        protected Runner(int width, int height)
        {
            Application = this;
            Width = width;
            Height = height;
            Game = new GameAdapter(this, width, height);
        }

        public virtual void Run() => Game.Run();

        public virtual void RunOnce() => Game.RunOneFrame();

        public void GoTo<T>()
            where T : Scene, new() => Game.GoTo<T>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                Game.Dispose();
                IsDisposed = true;
            }
        }
    }
}
