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

        protected override void BeforeDraw(RendererBatch batch)
        {
            Effect = null;
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            Effect = Shader?.Effect;
        }
    }
}
