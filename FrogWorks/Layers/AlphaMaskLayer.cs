using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public sealed class AlphaMaskLayer : Layer
    {
        BlendState _nonColorWriteBlend;
        DepthStencilState _alwaysStencil;
        AlphaTestEffect _alphaTestEffect;

        public Color ClearColor { get; set; } = Color.Black;

        public bool Reverse { get; set; }

        public AlphaMaskLayer(bool reverse = false)
            : base()
        {
            Reverse = reverse;

            _nonColorWriteBlend = new BlendState()
            {
                ColorWriteChannels = ColorWriteChannels.None
            };

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
            DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = false,
                StencilEnable = true,
                StencilFunction = Reverse
                    ? CompareFunction.Equal
                    : CompareFunction.NotEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 0
            };
            Effect = null;

            batch.Configure(BlendState, DepthStencilState, Effect, Matrix);
            batch.Begin();
            batch.DrawPrimitives(primitive =>
            {
                primitive.FillRectangle(
                    View.Location.ToVector2(),
                    View.Size.ToVector2(),
                    ClearColor);
            });
            batch.End();
        }
    }
}
