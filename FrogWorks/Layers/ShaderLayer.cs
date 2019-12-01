using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class ShaderLayer : Layer
    {
        protected Shader Shader { get; private set; }

        public ShaderLayer(Shader shader)
            : base()
        {
            Shader = shader;
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
