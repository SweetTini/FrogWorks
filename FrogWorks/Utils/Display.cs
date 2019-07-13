using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Display
    {
        private Scaling _scaling = Scaling.Fit;
        private bool _isBackBufferDirty;

        protected GraphicsDeviceManager Graphics { get; private set; }

        protected RenderTarget2D BackBuffer { get; private set; }

        protected Viewport Viewport { get; private set; }

        protected internal Matrix ScaleMatrix { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int ExtendedWidth { get; private set; }

        public int ExtendedHeight { get; private set; }

        public int ScreenWidth { get; private set; }

        public int ScreenHeight { get; private set; }

        public int BackBufferWidth { get; private set; }

        public int BackBufferHeight { get; private set; }

        public int HorizontalPadding { get; private set; }

        public int VerticalPadding { get; private set; }

        public float HorizontalScale { get; private set; } = 1f;

        public float VerticalScale { get; private set; } = 1f;

        public Scaling Scaling
        {
            get { return _scaling; }
            set
            {
                if (value == _scaling) return;
                _scaling = value;
                ApplyScaling();
                ResetBackBuffer();
            }
        }

        public Color ClearColor { get; set; } = Color.Black;

        internal Action OnBackBufferChanged { get; set; }

        internal Display(GraphicsDeviceManager graphics, GameWindow window, int width, int height)
        {
            Graphics = graphics;
            Width = width;
            Height = height;

            Graphics.DeviceCreated += OnDeviceChanged;
            Graphics.DeviceReset += OnDeviceChanged;
            window.ClientSizeChanged += OnDeviceChanged;
            SetFixedScale();
            ResetBackBuffer();
        }

        public void SetFixedScale(int scale = 1)
        {
            if (scale < 1) scale = 1;
            Graphics.PreferredBackBufferWidth = Width * scale;
            Graphics.PreferredBackBufferHeight = Height * scale;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        public void SetFullScreen()
        {
            var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            Graphics.PreferredBackBufferWidth = displayMode.Width;
            Graphics.PreferredBackBufferHeight = displayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
        }

        internal void ApplyScaling()
        {
            var lastExtendedWidth = ExtendedWidth;
            var lastExtendedHeight = ExtendedHeight;
            ScreenWidth = Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            ExtendedWidth = ExtendedHeight = 0;

            var sourceRatio = 1f * ScreenHeight / ScreenWidth;
            var targetRatio = 1f * Height / Width;

            switch (Scaling)
            {
                case Scaling.None:
                    HorizontalScale = VerticalScale = 1f;
                    break;
                case Scaling.Fit:
                    HorizontalScale = VerticalScale = sourceRatio < targetRatio 
                        ? 1f * ScreenHeight / Height 
                        : 1f * ScreenWidth / Width;
                    break;
                case Scaling.PixelPerfect:
                    HorizontalScale = VerticalScale = sourceRatio < targetRatio
                        ? ScreenHeight / Height
                        : ScreenWidth / Width;
                    break;
                case Scaling.Stretch:
                    HorizontalScale = 1f * ScreenWidth / Width;
                    VerticalScale = 1f * ScreenHeight / Height;
                    break;
                case Scaling.Extend:
                    if (sourceRatio < targetRatio)
                    {
                        var scale = 1f * ScreenHeight / Height;
                        var worldScale = 1f * Height / ScreenHeight;
                        ExtendedWidth = (int)Math.Round((ScreenWidth - Width * scale) * worldScale);
                        HorizontalScale = VerticalScale = scale;
                    }
                    else
                    {
                        var scale = 1f * ScreenWidth / Width;
                        var worldScale = 1f * Width / ScreenWidth;
                        ExtendedHeight = (int)Math.Round((ScreenHeight - Height * scale) * worldScale);
                        HorizontalScale = VerticalScale = scale;
                    }
                    break;
                case Scaling.Crop:
                    HorizontalScale = VerticalScale = sourceRatio > targetRatio
                        ? 1f * ScreenHeight / Height
                        : 1f * ScreenWidth / Width;
                    break;
            }

            BackBufferWidth = (int)((Width + ExtendedWidth) * HorizontalScale);
            BackBufferHeight = (int)((Height + ExtendedHeight) * VerticalScale);
            HorizontalPadding = (ScreenWidth - BackBufferWidth) / 2;
            VerticalPadding = (ScreenHeight - BackBufferHeight) / 2;
            Viewport = new Viewport(HorizontalPadding, VerticalPadding, BackBufferWidth, BackBufferHeight, 0, 1);
            ScaleMatrix = Matrix.CreateScale(HorizontalScale, VerticalScale, 1f);

            _isBackBufferDirty = lastExtendedWidth != ExtendedWidth || lastExtendedHeight != ExtendedHeight;
        }

        internal void DrawBackBuffer(RendererBatch batch, Scene scene)
        {
            Graphics.GraphicsDevice.SetRenderTarget(BackBuffer);
            Graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Width + ExtendedWidth, Height + ExtendedHeight);
            Graphics.GraphicsDevice.Clear(scene?.BackgroundColor ?? ClearColor);
            scene?.Draw(batch);

            Graphics.GraphicsDevice.SetRenderTarget(null);
            Graphics.GraphicsDevice.Viewport = Viewport;
            Graphics.GraphicsDevice.Clear(ClearColor);
            batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ScaleMatrix);
            batch.Sprite.Draw(BackBuffer, Vector2.Zero, Color.White);
            batch.Sprite.End();
        }

        private void ResetBackBuffer()
        {
            if (_isBackBufferDirty || BackBuffer == null)
            {
                if (BackBuffer != null)
                    BackBuffer.Dispose();

                BackBuffer = new RenderTarget2D(Graphics.GraphicsDevice, Width + ExtendedWidth, Height + ExtendedHeight);

                _isBackBufferDirty = false;
                OnBackBufferChanged?.Invoke();
            }
        }

        private void OnDeviceChanged(object sender, EventArgs args)
        {
            ApplyScaling();
            ResetBackBuffer();
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
