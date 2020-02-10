using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class GraphicsTestScene : DefaultScene
    {
        private DebugFont Font { get; set; }

        private SplashPattern Pattern { get; set; }

        private BasicLayer TextLayer { get; set; }

        private ShaderLayer WaveLayer { get; set; }

        private WaveShader WaveShader { get; set; }

        private SoundTrack DemoTrack { get; set; }

        public GraphicsTestScene() 
            : base()
        {
        }

        protected override void Begin()
        {
            WaveShader = Shader.Load<WaveShader>("Shaders\\WaveShader.fx");
            WaveLayer = new ShaderLayer(WaveShader);
            TextLayer = new BasicLayer();

            Add(WaveLayer, TextLayer);

            Pattern = new SplashPattern(Color.Red);
            Font = new DebugFont(Runner.Application.Width - 16, Runner.Application.Height - 16)
            {
                Text = "Hello World",
                Position = Vector2.One * 8f,
                Color = Color.LightCoral,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            DefaultLayer.Add(Pattern);
            TextLayer.Add(Font);

            DemoTrack = SoundTrack.Load("Music\\Demonstrate.ogg");
            DemoTrack.Play();
        }

        protected override void AfterUpdate(float deltaTime)
        {
            WaveShader.Timer += deltaTime;
        }
    }
}
