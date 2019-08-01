using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class MiniApple : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\MiniApple.png");

        protected Image Image { get; set; }

        public MiniApple()
            : base()
        {
            Collider = new CircleCollider(12f, -13f, -10f);
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            AddComponents(Image);
        }

        public override void Draw(RendererBatch batch)
        {
            base.Draw(batch);
        }

        public bool Contains(Vector2 point)
        {
            return Collider.Contains(point);
        }
    }
}
