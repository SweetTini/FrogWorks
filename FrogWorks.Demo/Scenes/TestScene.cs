using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class TestScene : DefaultScene
    {
        private DebugFont Font { get; set; }

        public TestScene() 
            : base()
        {
        }

        protected override void Begin()
        {
            BackgroundColor = Color.Gray;

            Font = new DebugFont(Runner.Application.Width - 16, Runner.Application.Height - 16);
            Font.Position = Vector2.One * 8f;
            Font.Color = Color.Black;
            Font.Text = "Hello World";

            DefaultLayer.Add(Font);
        }
    }
}
