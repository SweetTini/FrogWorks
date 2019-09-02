using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public sealed class DisplayAdapter : IDisposable
    {
        private GameAdapter _game;
        private RendererBatch _batch;
        private RenderTarget2D _buffer;
        private Viewport _viewport;
        private Matrix _matrix;

        private int _width, _height;
        private Scaling _scaling = Scaling.Fit;
        private bool _isDirty;

        public Color ClearColor { get; set; } = Color.Black;

        public int Width => _width + Extended.X;

        public int Height => _height + Extended.Y;

        public Point Extended { get; private set; }

        public Point View { get; private set; }

        public Point Padding { get; private set; }

        public Point Client { get; private set; }

        public Vector2 Scale { get; private set; }

        public Scaling Scaling
        {
            get { return _scaling; }
            set
            {
                if (_scaling == value) return;
                _scaling = value;
                ApplyScaling();
                Reset();
            }
        }

        public bool IsDisposed { get; private set; }

        public Action OnBufferChanged { get; set; }

        internal DisplayAdapter(GameAdapter game, int width, int height)
        {
            _game = game;
            _width = width;
            _height = height;
            _game.Graphics.DeviceCreated += OnDeviceChanged;
            _game.Graphics.DeviceReset += OnDeviceChanged;
            _game.Window.ClientSizeChanged += OnDeviceChanged;

            ToFixedScale();
            _game.ApplyChanges();

            _batch = new RendererBatch(_game.GraphicsDevice);
            _buffer = new RenderTarget2D(_game.GraphicsDevice, _width, _height);
        }

        public void Draw(Scene scene)
        {
            _game.GraphicsDevice.SetRenderTarget(_buffer);
            _game.GraphicsDevice.Viewport = new Viewport(0, 0, Width, Height);
            _game.GraphicsDevice.Clear(scene?.BackgroundColor ?? ClearColor);

            scene?.Draw(_batch);

            _game.GraphicsDevice.SetRenderTarget(null);
            _game.GraphicsDevice.Viewport = _viewport;
            _game.GraphicsDevice.Clear(ClearColor);

            _batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _matrix);
            _batch.Sprite.Draw(_buffer, Vector2.Zero, Color.White);
            _batch.Sprite.End();
        }

        public void ToFixedScale(int scale = 1)
        {
            scale = Math.Max(scale, 1);
            _game.Graphics.PreferredBackBufferWidth = _width * scale;
            _game.Graphics.PreferredBackBufferHeight = _height * scale;
            _game.Graphics.IsFullScreen = false;
            _game.MarkAsDirty();
        }

        public void ToFullscreen()
        {
            var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _game.Graphics.PreferredBackBufferWidth = displayMode.Width;
            _game.Graphics.PreferredBackBufferHeight = displayMode.Height;
            _game.Graphics.IsFullScreen = true;
            _game.MarkAsDirty();
        }

        public Vector2 ToView(Vector2 position)
            => Vector2.Transform(position - Padding.ToVector2(), Matrix.Invert(_matrix)).Round();

        public Vector2 FromView(Vector2 position)
            => Vector2.Transform(position, _matrix) + Padding.ToVector2().Round();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                _buffer.Dispose();
                _batch.Dispose();
                IsDisposed = true;
            }
        }

        private void OnDeviceChanged(object sender, EventArgs args)
        {
            ApplyScaling();
            Reset();
        }

        private void ApplyScaling()
        {
            var lastExtend = Extended;
            var parameters = _game.GraphicsDevice.PresentationParameters;
            Client = new Point(parameters.BackBufferWidth, parameters.BackBufferHeight);
            Extended = Point.Zero;

            var source = 1f * Client.Y / Client.X;
            var target = 1f * _height / _width;

            switch (Scaling)
            {
                case Scaling.None:
                    Scale = Vector2.One;
                    break;
                case Scaling.Fit:
                    {
                        var ratio = source < target
                            ? 1f * Client.Y / _height
                            : 1f * Client.X / _width;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case Scaling.PixelPerfect:
                    {
                        var ratio = source < target
                            ? Client.Y / _height
                            : Client.X / _width;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case Scaling.Stretch:
                    Scale = new Vector2(
                        1f * Client.X / _width, 
                        1f * Client.Y / _height);
                    break;
                case Scaling.Extend:
                    if (source < target)
                    {
                        var viewScale = 1f * Client.Y / _height;
                        var worldScale = 1f * _height / Client.Y;
                        var amount = (int)Math.Round((Client.X - _width * viewScale) * worldScale);
                        Extended = new Point(amount, Extended.Y);
                        Scale = Vector2.One * viewScale;
                    }
                    else
                    {
                        var viewScale = 1f * Client.X / _width;
                        var worldScale = 1f * _width / Client.X;
                        var amount = (int)Math.Round((Client.Y - _height * viewScale) * worldScale);
                        Extended = new Point(Extended.X, amount);
                        Scale = Vector2.One * viewScale;
                    }
                    break;
                case Scaling.Crop:
                    {
                        var ratio = source > target
                            ? 1f * Client.Y / _height
                            : 1f * Client.X / _width;
                        Scale = Vector2.One * ratio;
                    }
                    break;
            }

            View = new Point(
                (int)Math.Round(Width * Scale.X),
                (int)Math.Round(Height * Scale.Y));
            Padding = new Point(
                (Client.X - View.X) / 2,
                (Client.Y - View.Y) / 2);
            _viewport = new Viewport(Padding.X, Padding.Y, View.X, View.Y, 0f, 1f);
            _matrix = Matrix.CreateScale(new Vector3(Scale, 1f));
            if (!_isDirty) _isDirty = lastExtend != Extended;
        }

        private void Reset()
        {
            if (_isDirty || _buffer == null)
            {
                _buffer?.Dispose();
                _buffer = new RenderTarget2D(_game.GraphicsDevice, Width, Height);
                _isDirty = false;

                OnBufferChanged?.Invoke();
            }
        }
    }

    public enum Scaling
    {
        None,
        Fit,
        PixelPerfect,
        Stretch,
        Extend,
        Crop
    }
}
