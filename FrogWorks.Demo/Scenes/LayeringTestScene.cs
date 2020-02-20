using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class LayeringTestScene : Scene
    {
        WaveShader _waveShader;

        public LayeringTestScene()
            : base()
        {
        }

        protected override void Begin()
        {
            _waveShader = Shader.Load<WaveShader>(@"Shaders\Wave");

            Add(new ShaderLayer(_waveShader));
            Add(new Layer() { Color = Color.Purple });

            SetCurrentLayer(1);
            Add(new Apple(80, 96));
            SetCurrentLayer(0);
            Add(new Apple(128, 112));       
            SetCurrentLayer(-1);
            Add(new Apple(176, 128));

            ClearColor = Color.HotPink;
        }

        protected override void AfterUpdate(float deltaTime)
        {
            _waveShader.Timer += deltaTime;
        }
    }
}
