﻿using ChaiFoxes.FMODAudio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public sealed class GameAdapter : Game
    {
        Runner _runner;
        Scene _scene, _nextScene;
        bool _isDirty;

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
            //AudioMgr.Init(string.Empty);

            Content.RootDirectory = "Content";
        }

        public void GoTo<T>()
            where T : Scene, new()
        {
            _nextScene = new T();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (DeltaTime > 0f) 
                FramesPerSecond = (int)Math.Round(1f / DeltaTime);

            Input.Update(IsActive, DeltaTime);
            //AudioMgr.Update();

            _scene?.Update(DeltaTime);

            if (_scene != _nextScene)
            {
                _scene?.EndInternally();
                _scene = _nextScene;
                _scene?.BeginInternally();
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
                _scene?.ResetRenderTarget(true);
                Input.Dispose();
                AssetManager.ClearCache();
                //AudioMgr.Unload();
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

        internal void MarkAsDirty()
        {
            _isDirty = true;
        }
    }
}
