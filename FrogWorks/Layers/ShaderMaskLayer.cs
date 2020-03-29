using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public sealed class ShaderMaskLayer : ShaderLayer
    {
        BlendState _nonColorWriteBlend;
        DepthStencilState _alwaysStencil;
        AlphaTestEffect _alphaTestEffect;

        public bool Reverse { get; set; }

        public ShaderMaskLayer(Shader shader, bool reverse = false)
            : base(shader)
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

            base.AfterDraw(batch);
        }
    }
}
