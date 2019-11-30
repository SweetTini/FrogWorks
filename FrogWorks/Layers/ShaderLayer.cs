using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class ShaderLayer : Layer
    {
        protected Shader Shader { get; private set; }

        public Action<Shader> OnShaderUpdate { get; set; }

        public ShaderLayer(Shader shader, Action<Shader> onShaderUpdate = null)
            : base()
        {
            Shader = shader;
            OnShaderUpdate = onShaderUpdate;
        }

        protected override void AfterUpdate(float deltaTime)
        {
            OnShaderUpdate?.Invoke(Shader);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            if (Shader == null) return;

            var lastEffect = Effect;
            Effect = Shader.Effect;

            batch.Configure(BlendState, DepthStencilState, Effect);
            batch.Begin();
            batch.DrawSprites(sprite => sprite.Draw(Buffer, Vector2.Zero, Color.White));
            batch.End();

            Effect = lastEffect;
        }
    }
}
