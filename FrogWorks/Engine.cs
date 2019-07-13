using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;

namespace FrogWorks
{
    public class Engine : Game
    {
        private Scene _currentScene, _nextScene;
        private Cache<Scene> _sceneCache;
        private string _title = "New Game";
        private Version _version = new Version(0, 0, 1, 0);
        private bool _displayVersionOnTitle;

        internal static Engine Instance { get; private set; }

        public static int Width { get; private set; }

        public static int Height { get; private set; }

        public static string AssemblyDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }

        public static string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
        }

        protected GraphicsDeviceManager Graphics { get; private set; }

        protected RendererBatch RendererBatch { get; private set; }

        public Display Display { get; private set; }

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

        public bool DisplayVersionOnTitle
        {
            get { return _displayVersionOnTitle; }
            set
            {
                if (value == _displayVersionOnTitle) return;
                _displayVersionOnTitle = value;
                OnTitleChanged();
            }
        }

        public int FramesPerSecond { get; private set; }

        public float DeltaTime { get; private set; }

        public Engine(int width, int height)
        {
            _sceneCache = new Cache<Scene>();

            Instance = this;
            Width = width;
            Height = height;
            Graphics = new GraphicsDeviceManager(this);
            Display = new Display(Graphics, Window, width, height);
            RendererBatch = new RendererBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            OnTitleChanged();

            Input.Initialize();
        }

        public void SetScene<T>() where T : Scene, new()
        {
            _nextScene = _sceneCache.Create<T>();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (DeltaTime > 0f)
                FramesPerSecond = (int)Math.Round(1f / DeltaTime);

            Input.Update(IsActive, DeltaTime);

            _currentScene?.BeginUpdate();
            _currentScene?.Update(DeltaTime);
            _currentScene?.EndUpdate();

            if (_nextScene != _currentScene)
            {
                _currentScene?.End();
                var lastScene = _currentScene;
                _currentScene = _nextScene;
                OnSceneChanged(_currentScene, lastScene);
                _currentScene?.Initialize();
                _currentScene?.Begin();
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

        private void OnSceneChanged(Scene currentScene, Scene lastScene)
        {
            _sceneCache.Store(lastScene);
        }

        private void OnTitleChanged()
        {
            var version = _displayVersionOnTitle ? $"(ver. {_version.ToString()})" : "";
            Window.Title = $"{_title} {version}".Trim();
        }
    }
}
