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
            var camera = DefaultLayer.Camera;
            batch.Primitive.Begin(viewMatrix: camera.TransformMatrix);
            batch.Primitive.FillRectangle(camera.Left, camera.Top, 32f, 32f, Color.Red);
            batch.Primitive.FillRectangle(camera.Right - 32f, camera.Top, 32f, 32f, Color.Yellow);
            batch.Primitive.FillRectangle(camera.Right - 32f, camera.Bottom - 32f, 32f, 32f, Color.Blue);
            batch.Primitive.FillRectangle(camera.Left, camera.Bottom - 32f, 32f, 32f, Color.Green);
            batch.Primitive.End();
        }
    }
}
