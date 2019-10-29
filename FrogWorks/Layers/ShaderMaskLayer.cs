using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class ShaderMaskLayer : ShaderLayer
    {
        private BlendState _nonColorWriteBlend;
        private DepthStencilState _alwaysStencil, _keepIfZeroStencil;
        private AlphaTestEffect _alphaTestEffect;

        public ShaderMaskLayer(Effect shaderEffect, 
                               Action<Effect> onShaderUpdate = null, 
                               bool reverse = false)
            : base(shaderEffect, onShaderUpdate)
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
                StencilFunction = reverse
                    ? CompareFunction.Equal
                    : CompareFunction.NotEqual,
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

            base.AfterDraw(batch);
        }
    }
}
