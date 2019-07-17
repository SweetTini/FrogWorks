using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class MiniAppleEntity : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\MiniApple.png");

        protected Image Image { get; set; }

        public MiniAppleEntity()
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

            Collider.Draw(batch, Color.Yellow);
        }

        public bool Contains(Vector2 point)
        {
            return Collider.Contains(point);
        }
    }
}
