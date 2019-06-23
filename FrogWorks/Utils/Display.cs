using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Display
    {
        protected GraphicsDeviceManager Graphics { get; private set; }

        protected RenderTarget2D ScreenBuffer { get; private set; }

        protected Matrix ScreenMatrix { get; private set; }

        protected Viewport Viewport { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int ScreenWidth { get; private set; }

        public int ScreenHeight { get; private set; }

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

            ScreenBuffer = new RenderTarget2D(Graphics.GraphicsDevice, Width, Height);
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

            ScreenWidth = (int)(Width * HorizontalScale);
            ScreenHeight = (int)(Height * VerticalScale);
            HorizontalPadding = (parameters.BackBufferWidth - ScreenWidth) / 2;
            VerticalPadding = (parameters.BackBufferHeight - ScreenHeight) / 2;
            Viewport = new Viewport(HorizontalPadding, VerticalPadding, ScreenWidth, ScreenHeight, 0, 1);
            ScreenMatrix = Matrix.CreateScale(VerticalScale);
        }

        internal void DrawSceneOnBuffer(Scene scene)
        {
            Graphics.GraphicsDevice.SetRenderTarget(ScreenBuffer);
            Graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Width, Height);

            Graphics.GraphicsDevice.SetRenderTarget(null);
            Graphics.GraphicsDevice.Viewport = Viewport;
        }

        internal void DrawScreen(RendererBatch batch)
        {
            Graphics.GraphicsDevice.Clear(ClearColor);
            batch.Sprite.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ScreenMatrix);
            batch.Sprite.Draw(ScreenBuffer, Vector2.Zero, Color.White);
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
