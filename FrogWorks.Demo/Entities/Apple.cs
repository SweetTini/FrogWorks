using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class Apple : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Apple.png");

        protected Image Image { get; set; }

        public Apple()
            : base()
        {
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            Components.Add(Image);
            Collider = new RectangleCollider(48f, 40f, -24f, -16f);
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            //Collider.DebugDraw(batch, Color.Yellow, false);
        }
    }
}
