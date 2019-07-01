using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class DisplayTestScene : Scene
    {
        public DisplayTestScene()
            : base()
        {
            BackgroundColor = Color.Gray;
        }

        public override void Draw(RendererBatch batch)
        {
            MainLayer.ConfigureBatch(batch);
            batch.Begin();

            batch.DrawPrimitives((primitive) =>
            {
                primitive.FillRectangle(MainLayer.Camera.Left, MainLayer.Camera.Top, 32f, 32f, Color.Red);
                primitive.FillRectangle(MainLayer.Camera.Right - 32f, MainLayer.Camera.Top, 32f, 32f, Color.Yellow);
                primitive.FillRectangle(MainLayer.Camera.Right - 32f, MainLayer.Camera.Bottom - 32f, 32f, 32f, Color.Blue);
                primitive.FillRectangle(MainLayer.Camera.Left, MainLayer.Camera.Bottom - 32f, 32f, 32f, Color.Green);
            });

            batch.End();
        }
    }
}
