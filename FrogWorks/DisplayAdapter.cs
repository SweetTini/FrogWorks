using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public sealed class DisplayAdapter : IDisposable
    {
        GameAdapter _game;
        RendererBatch _batch;
        Viewport _viewport;
        Matrix _matrix;

        Point _size;
        ScalingType _scaling = ScalingType.Fit;
        bool _isDirty;

        internal GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

        public Color ClearColor { get; set; } = Color.Black;

        public Point Size => _size + ExtendedSize;

        public int Width => Size.X;

        public int Height => Size.Y;

        public Point ExtendedSize { get; private set; }

        public Point ViewSize { get; private set; }

        public Point Padding { get; private set; }

        public Point ClientSize { get; private set; }

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

            var renderTarget = scene?.Draw(this, _batch);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = _viewport;
            GraphicsDevice.Clear(ClearColor);

            if (renderTarget == null) return;

            _batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _matrix);
            _batch.Sprite.Draw(renderTarget, Vector2.Zero, Color.White);
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
        {
            position -= Padding.ToVector2();
            return Vector2.Transform(position, Matrix.Invert(_matrix)).Round();
        }

        public Vector2 FromView(Vector2 position)
        {
            var transform = Vector2.Transform(position, _matrix).Round();
            return transform + Padding.ToVector2();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                _batch.Dispose();
                IsDisposed = true;
            }
        }

        void OnDeviceChanged(object sender, EventArgs args)
        {
            ApplyScaling();
        }

        void ApplyScaling()
        {
            var parameters = _game.GraphicsDevice.PresentationParameters;
            var lastExtendedSize = ExtendedSize;
            var lastScale = Scale;

            ExtendedSize = Point.Zero;
            ClientSize = new Point(
                parameters.BackBufferWidth,
                parameters.BackBufferHeight);

            if (Scaling == ScalingType.None)
            {
                Scale = Vector2.One;
            }
            else if (Scaling == ScalingType.Stretch)
            {
                Scale = ClientSize.ToVector2() / _size.ToVector2();
            }
            else
            {
                var source = 1f * ClientSize.Y / ClientSize.X;
                var target = 1f * _size.Y / _size.X;
                var canCrop = Scaling == ScalingType.Crop && source > target;
                var scaleByHeight = source < target;
                var ratio = canCrop || scaleByHeight
                    ? 1f * ClientSize.Y / _size.Y
                    : 1f * ClientSize.X / _size.X;

                if (Scaling == ScalingType.PixelPerfect)
                    ratio = ratio.Floor();

                if (Scaling == ScalingType.Extend)
                {
                    var worldRatio = scaleByHeight
                        ? 1f * _size.Y / ClientSize.Y
                        : 1f * _size.X / ClientSize.X;
                    var amount = scaleByHeight
                        ? ((ClientSize.X - _size.X * ratio) * worldRatio)
                        : ((ClientSize.Y - _size.Y * ratio) * worldRatio);
                    var scaleUnit = scaleByHeight
                        ? Vector2.UnitX
                        : Vector2.UnitY;

                    ExtendedSize = (scaleUnit * amount.Round()).ToPoint();
                }

                Scale = Vector2.One * ratio;
            }

            ViewSize = (Size.ToVector2() * Scale).Round().ToPoint();
            Padding = ((ClientSize - ViewSize).ToVector2() * .5f).Round().ToPoint();

            _viewport = new Viewport(new Rectangle(Padding, ViewSize));
            _matrix = Matrix.CreateScale(new Vector3(Scale, 1f));

            if (!_isDirty)
            {
                _isDirty = lastExtendedSize != ExtendedSize
                    || lastScale != Scale;
            }
        }

        void Reset(Scene scene)
        {
            if (_isDirty)
            {
                scene?.ResetDisplay();
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
