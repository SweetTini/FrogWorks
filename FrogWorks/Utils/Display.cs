using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Display
    {
        protected GraphicsDeviceManager Graphics { get; private set; }

        protected RenderTarget2D BackBuffer { get; private set; }

        protected Matrix ScreenMatrix { get; private set; }

        protected Viewport Viewport { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int BackBufferWidth { get; private set; }

        public int BackBufferHeight { get; private set; }

        public int HorizontalPadding { get; private set; }

        public int VerticalPadding { get; private set; }

        public float HorizontalScale { get; private set; } = 1f;

        public float VerticalScale { get; private set; } = 1f;

        public Scaling Scaling { get; private set; } = Scaling.Fit;

        public Color ClearColor { get; set; } = Color.Black;

        public Display(GraphicsDeviceManager graphics, int width, int height)
        {
            Graphics = graphics;
            Width = width;
            Height = height;

            Graphics.DeviceCreated += OnDeviceChanged;
            Graphics.DeviceReset += OnDeviceChanged;
            SetFixedScale();

            BackBuffer = new RenderTarget2D(Graphics.GraphicsDevice, Width, Height);
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

        public void SetScaling(Scaling scaling)
        {
            Scaling = scaling;
            Graphics.ApplyChanges();
        }

        internal void ApplyScaling()
        {
            var parameters = Graphics.GraphicsDevice.PresentationParameters;
            var sourceRatio = 1f * parameters.BackBufferHeight / parameters.BackBufferWidth;
            var targetRatio = 1f * Height / Width;
            
            switch (Scaling)
            {
                case Scaling.None:
                    HorizontalScale = VerticalScale = 1f;
                    break;
                case Scaling.Fit:
                    VerticalScale = sourceRatio < targetRatio 
                        ? 1f * parameters.BackBufferHeight / Height 
                        : 1f * parameters.BackBufferWidth / Width;
                    HorizontalScale = VerticalScale;
                    break;
                case Scaling.PixelPerfect:
                    VerticalScale = sourceRatio < targetRatio
                        ? parameters.BackBufferHeight / Height
                        : parameters.BackBufferWidth / Width;
                    HorizontalScale = VerticalScale;
                    break;
                case Scaling.Stretch:
                    HorizontalScale = 1f * parameters.BackBufferWidth / Width;
                    VerticalScale = 1f * parameters.BackBufferHeight / Height;
                    break;
            }

            BackBufferWidth = (int)(Width * HorizontalScale);
            BackBufferHeight = (int)(Height * VerticalScale);
            HorizontalPadding = (parameters.BackBufferWidth - BackBufferWidth) / 2;
            VerticalPadding = (parameters.BackBufferHeight - BackBufferHeight) / 2;
            Viewport = new Viewport(HorizontalPadding, VerticalPadding, BackBufferWidth, BackBufferHeight, 0, 1);
            ScreenMatrix = Matrix.CreateScale(VerticalScale);
        }

        internal void DrawBackBuffer(RendererBatch batch, Scene scene)
        {
            Graphics.GraphicsDevice.SetRenderTarget(BackBuffer);
            Graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Width, Height);

            Graphics.GraphicsDevice.SetRenderTarget(null);
            Graphics.GraphicsDevice.Viewport = Viewport;

            Graphics.GraphicsDevice.Clear(ClearColor);
            batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ScreenMatrix);
            batch.Sprite.Draw(BackBuffer, Vector2.Zero, Color.White);
            batch.Sprite.End();
        }

        private void OnDeviceChanged(object sender, EventArgs args)
        {
            ApplyScaling();
        }
    }

    public enum Scaling
    {
        None,
        Fit,
        PixelPerfect,
        Stretch
    }
}
