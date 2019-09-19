using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public sealed class AlphaMaskLayer : Layer
    {
        private BlendState _nonColorWriteBlend;
        private DepthStencilState _alwaysStencil, _keepIfZeroStencil;
        private AlphaTestEffect _alphaTestEffect;

        public Color BackgroundColor { get; set; } = Color.Black;

        public AlphaMaskLayer()
            : base()
        {
            _nonColorWriteBlend = new BlendState() { ColorWriteChannels = ColorWriteChannels.None };

            _alphaTestEffect = new AlphaTestEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                AlphaFunction = CompareFunction.Greater,
                ReferenceAlpha = 0
            };

            _alwaysStencil = new DepthStencilState()
            {
                DepthBufferEnable = false,
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1
            };

            _keepIfZeroStencil = new DepthStencilState()
            {
                DepthBufferEnable = false,
                StencilEnable = true,
                StencilFunction = CompareFunction.Equal,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 0
            };
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            BlendState = _nonColorWriteBlend;
            DepthStencilState = _alwaysStencil;
            Effect = _alphaTestEffect;
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            BlendState = null;
            DepthStencilState = _keepIfZeroStencil;
            Effect = null;

            batch.Configure(BlendState, DepthStencilState, Effect, Camera);
            batch.Begin();
            batch.DrawPrimitives(primitive => 
            {
                primitive.FillRectangle(
                    Camera.View.Location.ToVector2(),
                    Camera.View.Size.ToVector2(),
                    BackgroundColor);
            });
            batch.End();
        }
    }
}
