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

        private Point _size;
        private Scaling _scaling = Scaling.Fit;
        private bool _isDirty;

        public Color ClearColor { get; set; } = Color.Black;

        public Point Size => _size + Extended;

        public int Width => Size.X;

        public int Height => Size.Y;

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

        internal DisplayAdapter(GameAdapter game, int width, int height, int scale, bool fullscreen)
        {
            _game = game;
            _size = new Point(width, height).Abs();
            _game.Graphics.DeviceCreated += OnDeviceChanged;
            _game.Graphics.DeviceReset += OnDeviceChanged;
            _game.Window.ClientSizeChanged += OnDeviceChanged;

            if (fullscreen) ToFullscreen();
            else ToFixedScale(scale);
            
            _game.ApplyChanges();

            _batch = new RendererBatch(_game.GraphicsDevice);
            _buffer = new RenderTarget2D(_game.GraphicsDevice, _size.X, _size.Y);
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
            _game.Graphics.PreferredBackBufferWidth = _size.X * scale;
            _game.Graphics.PreferredBackBufferHeight = _size.Y * scale;
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
            var lastExtended = Extended;
            var parameters = _game.GraphicsDevice.PresentationParameters;
            Client = new Point(parameters.BackBufferWidth, parameters.BackBufferHeight);
            Extended = Point.Zero;

            var source = 1f * Client.Y / Client.X;
            var target = 1f * _size.Y / _size.X;

            switch (Scaling)
            {
                case Scaling.None:
                    Scale = Vector2.One;
                    break;
                case Scaling.Fit:
                    {
                        var ratio = source < target
                            ? 1f * Client.Y / _size.Y
                            : 1f * Client.X / _size.X;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case Scaling.PixelPerfect:
                    {
                        var ratio = source < target
                            ? Client.Y / _size.Y
                            : Client.X / _size.X;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case Scaling.Stretch:
                    Scale = new Vector2(
                        1f * Client.X / _size.X, 
                        1f * Client.Y / _size.Y);
                    break;
                case Scaling.Extend:
                    if (source < target)
                    {
                        var viewScale = 1f * Client.Y / _size.Y;
                        var worldScale = 1f * _size.Y / Client.Y;
                        var amount = (int)Math.Round((Client.X - _size.X * viewScale) * worldScale);
                        Extended = new Point(amount, Extended.Y);
                        Scale = Vector2.One * viewScale;
                    }
                    else
                    {
                        var viewScale = 1f * Client.X / _size.X;
                        var worldScale = 1f * _size.X / Client.X;
                        var amount = (int)Math.Round((Client.Y - _size.Y * viewScale) * worldScale);
                        Extended = new Point(Extended.X, amount);
                        Scale = Vector2.One * viewScale;
                    }
                    break;
                case Scaling.Crop:
                    {
                        var ratio = source > target
                            ? 1f * Client.Y / _size.Y
                            : 1f * Client.X / _size.X;
                        Scale = Vector2.One * ratio;
                    }
                    break;
            }

            View = (Size.ToVector2() * Scale).Round().ToPoint();
            Padding = ((Client - View).ToVector2() / 2f).Round().ToPoint();

            _viewport = new Viewport(Padding.X, Padding.Y, View.X, View.Y, 0f, 1f);
            _matrix = Matrix.CreateScale(new Vector3(Scale, 1f));
            _isDirty = _isDirty || lastExtended != Extended;
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
