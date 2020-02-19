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

            Add(new ShaderLayer(_waveShader) { RenderBeforeMerge = true });
            Add(new Layer());

            SetCurrentLayer(1);
            Add(new Apple() { X = 80f, Y = 96f });
            SetCurrentLayer(0);
            Add(new Apple() { X = 128f, Y = 112f });
            SetCurrentLayer(-1);
            Add(new Apple() { X = 176f, Y = 128f });

            ClearColor = Color.HotPink;
        }

        protected override void AfterUpdate(float deltaTime)
        {
            _waveShader.Timer += deltaTime;
        }
    }
}
