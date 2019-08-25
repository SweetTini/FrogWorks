using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;

namespace FrogWorks
{
    public class Engine : Game
    {
        private Scene _currentScene, _nextScene;
        private string _title = "New Game";
        private Version _version = new Version(0, 0, 1, 0);
        private bool _displayVersionOnHeader;

        internal static Engine Instance { get; private set; }

        public static Display Display { get; private set; }

        public static string AssemblyDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }

        public static string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
        }

        public static int FramesPerSecond { get; private set; }

        protected GraphicsDeviceManager Graphics { get; private set; }

        protected RendererBatch RendererBatch { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value == _title) return;
                    _title = value.Trim();
                    OnTitleChanged();
                }
            }
        }

        public Version Version
        {
            get { return _version; }
            set
            {
                if (value == _version) return;
                _version = value;
                OnTitleChanged();
            }
        }

        public bool DisplayVersionOnHeader
        {
            get { return _displayVersionOnHeader; }
            set
            {
                if (value == _displayVersionOnHeader) return;
                _displayVersionOnHeader = value;
                OnTitleChanged();
            }
        }

        public float DeltaTime { get; private set; }

        public bool UseVSync
        {
            get { return Graphics.SynchronizeWithVerticalRetrace; }
            set
            {
                if (value == Graphics.SynchronizeWithVerticalRetrace) return;
                Graphics.SynchronizeWithVerticalRetrace = value;
                Graphics.ApplyChanges();
            }
        }

        public bool UseHiDef
        {
            get { return Graphics.GraphicsProfile == GraphicsProfile.HiDef; }
            set
            {
                var graphicsProfile = value ? GraphicsProfile.HiDef : GraphicsProfile.Reach;
                if (graphicsProfile == Graphics.GraphicsProfile) return;
                Graphics.GraphicsProfile = graphicsProfile;
                Graphics.ApplyChanges();
            }
        }

        public Engine(int width, int height)
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.Reach;
            Display = new Display(Graphics, Window, width, height);
            RendererBatch = new RendererBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            OnTitleChanged();

            Input.Initialize();
        }

        public void SetScene<T>() where T : Scene, new()
        {
            _nextScene = new T();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (DeltaTime > 0f)
                FramesPerSecond = (int)Math.Round(1f / DeltaTime);

            Input.Update(IsActive, DeltaTime);
            _currentScene?.Update(DeltaTime);

            if (_nextScene != _currentScene)
            {
                _currentScene?.InternalEnd();
                _currentScene = _nextScene;
                _currentScene?.InternalBegin();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Display.DrawBackBuffer(RendererBatch, _currentScene);
            base.Draw(gameTime);
        }

        protected override void EndRun()
        {
            RendererBatch.Dispose();
            Input.Close();
            base.EndRun();
        }

        private void OnTitleChanged()
        {
            var version = _displayVersionOnHeader ? $"(ver. {_version.ToString()})" : "";
            Window.Title = $"{_title} {version}".Trim();
        }
    }
}
