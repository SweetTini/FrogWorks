using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public sealed class GameAdapter : Game
    {
        private Runner _runner;
        private Scene _scene, _nextScene;
        private bool _isDirty;

        public GraphicsDeviceManager Graphics { get; private set; }

        public int FramesPerSecond { get; private set; }

        public float DeltaTime { get; private set; }

        internal GameAdapter(Runner runner)
        {
            _runner = runner;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.Reach;
            Input.Initialize();

            Content.RootDirectory = "Content";
        }

        public void GoTo<T>()
            where T : Scene, new() => _nextScene = new T();

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (DeltaTime > 0f) FramesPerSecond = (int)Math.Round(1f / DeltaTime);

            Input.Update(IsActive, DeltaTime);
            _scene?.Update(DeltaTime);

            if (_scene != _nextScene)
            {
                _scene?.InternalEnd();
                _scene = _nextScene;
                _scene?.InternalBegin();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ApplyChanges();
            _runner.Display.Draw(_scene);
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scene?.Layers.Dispose();
                Input.Dispose();
                Audio.DisposeCache();
                Texture.DisposeCache();
            }

            base.Dispose(disposing);
        }

        internal void ApplyChanges()
        {
            if (_isDirty)
            {
                Graphics.ApplyChanges();
                _isDirty = false;
            }
        }

        internal void MarkAsDirty() => _isDirty = true;
    }
}
