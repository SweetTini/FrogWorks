using Microsoft.Xna.Framework;
using System;
using XnaGame = Microsoft.Xna.Framework.Game;

namespace FrogWorks
{
    public class Game : XnaGame
    {
        private Scene _currentScene, _nextScene;
        private string _title = "New Game";
        private Version _version = new Version(0, 0, 1, 0);
        private bool _displayVersion;

        public static Game Instance { get; private set; }

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
                    OnHeaderChanged();
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
                OnHeaderChanged();
            }
        }

        public bool DisplayVersion
        {
            get { return _displayVersion; }
            set
            {
                if (value == _displayVersion) return;
                _displayVersion = value;
                OnHeaderChanged();
            }
        }

        public Game(int width, int height)
        {
            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Display = new Display(Graphics, width, height);
            RendererBatch = new RendererBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            OnHeaderChanged();
        }

        protected override void Update(GameTime gameTime)
        {
            _currentScene?.Update();

            if (_nextScene != _currentScene)
            {
                _currentScene?.End();
                var lastScene = _currentScene;
                _currentScene = _nextScene;
                OnSceneChanged(_currentScene, lastScene);
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
            base.EndRun();
        }

        private void OnSceneChanged(Scene currentScene, Scene lastScene)
        {
        }

        private void OnHeaderChanged()
        {
            var version = _displayVersion ? $"(ver. {_version.ToString()})" : "";
            Window.Title = $"{_title} {version}".Trim();
        }
    }
}
