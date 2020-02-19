using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class DisplayTestScene : Scene
    {
        public DisplayTestScene()
            : base()
        {
        }

        protected override void Begin()
        {
            var resolution = Runner.Application.Size.ToVector2();
            var apple = new Apple() { Position = resolution * .5f };
            Add(apple);

            ClearColor = Color.DarkSlateGray;
        }

        protected override void End()
        {
            Runner.Application.SetDisplay(ScalingType.Fit);
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Alpha1))
                Runner.Application.SetDisplay(ScalingType.None);
            else if (Input.Keyboard.IsPressed(Keys.Alpha2))
                Runner.Application.SetDisplay(ScalingType.Fit);
            else if (Input.Keyboard.IsPressed(Keys.Alpha3))
                Runner.Application.SetDisplay(ScalingType.PixelPerfect);
            else if (Input.Keyboard.IsPressed(Keys.Alpha4))
                Runner.Application.SetDisplay(ScalingType.Stretch);
            else if (Input.Keyboard.IsPressed(Keys.Alpha5))
                Runner.Application.SetDisplay(ScalingType.Extend);
            else if (Input.Keyboard.IsPressed(Keys.Alpha6))
                Runner.Application.SetDisplay(ScalingType.Crop);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            var display = Runner.Application.Size.ToVector2();
            var offset = display - Vector2.One * 17f;
            var size = Vector2.One * 16f;

            batch.DrawPrimitives(b =>
            {
                b.DrawRectangle(Vector2.Zero, size, Color.Red);
                b.DrawRectangle(Vector2.UnitX * offset.X, size, Color.Yellow);
                b.DrawRectangle(Vector2.UnitY * offset.Y, size, Color.LimeGreen);
                b.DrawRectangle(offset, size, Color.Cyan);
            });
        }
    }
}
