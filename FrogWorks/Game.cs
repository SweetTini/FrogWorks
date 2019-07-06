﻿using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using XnaGame = Microsoft.Xna.Framework.Game;

namespace FrogWorks
{
    public class Game : XnaGame
    {
        private Scene _currentScene, _nextScene;
        private Cache<Scene> _sceneCache;
        private string _title = "New Game";
        private Version _version = new Version(0, 0, 1, 0);
        private bool _displayVersionOnTitle;

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

        public string AssemblyDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }

        public string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Content.RootDirectory); }
        }

        public Game(int width, int height)
        {
            _sceneCache = new Cache<Scene>();

            Instance = this;
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
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (deltaTime > 0f)
                FramesPerSecond = (int)Math.Round(1f / deltaTime);

            Input.Update(IsActive, deltaTime);

            _currentScene?.BeginUpdate();
            _currentScene?.Update(deltaTime);
            _currentScene?.EndUpdate();

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
