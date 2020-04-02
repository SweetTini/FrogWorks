using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;

namespace FrogWorks
{
    public abstract class Runner : IDisposable
    {
        public static Runner Application { get; private set; }

        protected internal GameAdapter Game { get; private set; }

        protected internal DisplayAdapter Display { get; private set; }

        public Version Version { get; set; } = new Version(0, 0, 1);

        public virtual string AssemblyDirectory
        {
            get 
            {
                var assembly = Assembly.GetEntryAssembly();
                return Path.GetDirectoryName(assembly.Location); 
            }
        }

        public string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Game.Content.RootDirectory); }
            set { Game.Content.RootDirectory = value; }        
        }

        public Color ClearColor
        {
            get { return Display.ClearColor; }
            set { Display.ClearColor = value; }
        }

        public Point Size { get; private set; }

        public int Width => Size.X;

        public int Height => Size.Y;

        public Point ActualSize => Display.Size;

        public Point ClientSize => Display.ClientSize;

        public Point ViewSize => Display.ViewSize;

        public int FramesPerSecond => Game.FramesPerSecond;

        public float DeltaTime => Game.DeltaTime;

        public bool IsDisposed { get; protected set; }

        protected Runner(int width, int height, int scale, bool fullscreen)
        {
            Application = this;
            Size = new Point(width, height).Abs();
            Game = new GameAdapter(this);
            Display = new DisplayAdapter(Game, Size, scale, fullscreen);
        }

        public virtual void Run()
        {
            Game.Run();
        }

        public virtual void RunOnce()
        {
            Game.RunOneFrame();
        }

        public void GoTo<T>()
            where T : Scene, new()
        {
            Game.GoTo<T>();
        }

        public Vector2 ToView(Vector2 position)
        {
            return Display.ToView(position);
        }

        public Vector2 FromView(Vector2 position)
        {
            return Display.FromView(position);
        }

        public void SetDisplay(ScalingType scaling)
        {
            Display.Scaling = scaling;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                Display.Dispose();
                Game.Dispose();
                IsDisposed = true;
            }
        }
    }
}
