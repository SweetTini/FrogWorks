using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestMenu : Scene
    {
        public TestMenu()
            : base()
        {
            BackgroundColor = Color.Black;

            var offsetX = (Engine.Display.Width - (Engine.Display.Width - 64f)) / 2f;
            var offsetY = (Engine.Display.Height - 160f) / 2f;

            AddEntities(new MenuItem("Display Test", offsetX, offsetY, () => SetNextScene<DisplayTest>()));
            AddEntities(new MenuItem("Graphics Test", offsetX, offsetY + 32, () => SetNextScene<GraphicsTest>()));
            AddEntities(new MenuItem("BVH Tree Test", offsetX, offsetY + 64, () => SetNextScene<BvhTreeTest>()));
            AddEntities(new MenuItem("Shape Collision Test", offsetX, offsetY + 96, () => SetNextScene<ShapeCollisionTest>()));
            AddEntities(new MenuItem("Basic Platformer Test", offsetX, offsetY + 128, () => SetNextScene<BasicPlatformerTest>()));
        }

        public override void Update(float deltaTime)
        {
            var cursor = MainLayer.Camera.ViewToWorld(Input.Mouse.Position);

            foreach (var item in GetEntitiesOfType<MenuItem>())
            {
                if (item.IsHovered(cursor) && Input.Mouse.IsClicked(MouseButton.Left))
                {
                    item.OnSelected?.Invoke();
                    break;
                }
            }

            base.Update(deltaTime);
        }
    }
}
