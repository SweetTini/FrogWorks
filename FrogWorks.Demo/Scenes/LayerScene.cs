using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class LayerScene : Scene
    {
        WaveShader _waveShader;

        public LayerScene()
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
            Add(new Apple(176, 160));
            Add(new Apple(176, 96));
            Add(new Apple(176, 128));

            ClearColor = Color.HotPink;
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            var mousePosition = Input.Mouse.Position;

            foreach (var apple in this.OnLayer(null).OfType<Apple>())
            {
                if (apple.IsOverlapping(mousePosition) 
                    && Input.Mouse.IsClicked(MouseButton.Left))
                {
                    apple.MoveToTop();
                    break;
                }
            }
        }

        protected override void AfterUpdate(float deltaTime)
        {
            _waveShader.Timer += deltaTime;
        }
    }
}
