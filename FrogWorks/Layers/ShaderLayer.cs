using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public class ShaderLayer : Layer
    {
        protected Effect ShaderEffect { get; private set; }

        public ShaderLayer(Effect shaderEffect)
            : base()
        {
            ShaderEffect = shaderEffect;
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
