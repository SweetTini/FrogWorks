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
            batch.Primitive.Begin(viewMatrix: Camera.TransformMatrix);
            batch.Primitive.FillRectangle(Camera.Left, Camera.Top, 32f, 32f, Color.Red);
            batch.Primitive.FillRectangle(Camera.Right - 32f, Camera.Top, 32f, 32f, Color.Yellow);
            batch.Primitive.FillRectangle(Camera.Right - 32f, Camera.Bottom - 32f, 32f, 32f, Color.Blue);
            batch.Primitive.FillRectangle(Camera.Left, Camera.Bottom - 32f, 32f, 32f, Color.Green);
            batch.Primitive.End();
        }
    }
}
