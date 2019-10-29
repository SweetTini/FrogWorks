using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class ShaderLayer : Layer
    {
        protected Effect ShaderEffect { get; private set; }

        public Action<Effect> OnShaderUpdate { get; set; }

        public ShaderLayer(Effect shaderEffect, Action<Effect> onShaderUpdate = null)
            : base()
        {
            ShaderEffect = shaderEffect;
            OnShaderUpdate = onShaderUpdate;
        }

        protected override void AfterUpdate(float deltaTime)
        {
            OnShaderUpdate?.Invoke(ShaderEffect);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            if (ShaderEffect == null) return;

            var lastEffect = Effect;
            Effect = ShaderEffect;

            batch.Configure(BlendState, DepthStencilState, Effect);
            batch.Begin();
            batch.DrawSprites(sprite => sprite.Draw(Buffer, Vector2.Zero, Color.White));
            batch.End();

            Effect = lastEffect;
        }
    }
}
