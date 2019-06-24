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

        public GraphicsDeviceManager Graphics { get; private set; }

        public Display Display { get; private set; }

        public RendererBatch RendererBatch { get; private set; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value == _title) return;
                    _title = value.Trim();
                    OnTitleOrVersionChanged();
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
                OnTitleOrVersionChanged();
            }
        }

        public bool DisplayVersion
        {
            get { return _displayVersion; }
            set
            {
                if (value == _displayVersion) return;
                _displayVersion = value;
                OnTitleOrVersionChanged();
            }
        }

        public Game(int width, int height)
        {
            Graphics = new GraphicsDeviceManager(this);
            Display = new Display(Graphics, width, height);
            RendererBatch = new RendererBatch(GraphicsDevice);
            OnTitleOrVersionChanged();
        }

        protected override void Update(GameTime gameTime)
        {
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

        private void OnTitleOrVersionChanged()
        {
            var version = _displayVersion ? $"(ver. {_version.ToString()})" : "";
            Window.Title = $"{_title} {version}".Trim();
        }
    }
}
