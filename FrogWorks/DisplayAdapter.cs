using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public sealed class DisplayAdapter : IDisposable
    {
        private GameAdapter _game;
        private RendererBatch _batch;
        private Viewport _viewport;
        private Matrix _matrix;

        private Point _size;
        private ScalingType _scaling = ScalingType.Fit;
        private bool _isDirty;

        internal GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

        public Color ClearColor { get; set; } = Color.Black;

        public Point Size => _size + Extended;

        public int Width => Size.X;

        public int Height => Size.Y;

        public Point Extended { get; private set; }

        public Point View { get; private set; }

        public Point Padding { get; private set; }

        public Point Client { get; private set; }

        public Vector2 Scale { get; private set; }

        public ScalingType Scaling
        {
            get { return _scaling; }
            set
            {
                if (_scaling == value) return;
                _scaling = value;
                ApplyScaling();
            }
        }

        public bool IsDisposed { get; private set; }

        internal DisplayAdapter(GameAdapter game, Point size, int scale, bool fullscreen)
        {
            _game = game;
            _size = size;
            _game.Graphics.DeviceCreated += OnDeviceChanged;
            _game.Graphics.DeviceReset += OnDeviceChanged;
            _game.Window.ClientSizeChanged += OnDeviceChanged;

            if (fullscreen) ToFullscreen();
            else ToFixedScale(scale);

            _game.ApplyChanges();

            _batch = new RendererBatch(_game.GraphicsDevice);
        }

        public void Draw(Scene scene)
        {
            Reset(scene);

            var buffer = scene?.Draw(this, _batch);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = _viewport;
            GraphicsDevice.Clear(ClearColor);

            if (buffer == null) return;

            _batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _matrix);
            _batch.Sprite.Draw(buffer, Vector2.Zero, Color.White);
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
                _batch.Dispose();
                IsDisposed = true;
            }
        }

        private void OnDeviceChanged(object sender, EventArgs args)
        {
            ApplyScaling();
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
                case ScalingType.None:
                    Scale = Vector2.One;
                    break;
                case ScalingType.Fit:
                    {
                        var ratio = source < target
                            ? 1f * Client.Y / _size.Y
                            : 1f * Client.X / _size.X;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case ScalingType.PixelPerfect:
                    {
                        var ratio = source < target
                            ? Client.Y / _size.Y
                            : Client.X / _size.X;
                        Scale = Vector2.One * ratio;
                    }
                    break;
                case ScalingType.Stretch:
                    Scale = new Vector2(
                        1f * Client.X / _size.X,
                        1f * Client.Y / _size.Y);
                    break;
                case ScalingType.Extend:
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
                case ScalingType.Crop:
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

        private void Reset(Scene scene)
        {
            if (_isDirty)
            {
                scene?.OnDisplayReset();
                _isDirty = false;
            }
        }
    }

    public enum ScalingType
    {
        None,
        Fit,
        PixelPerfect,
        Stretch,
        Extend,
        Crop
    }
}
