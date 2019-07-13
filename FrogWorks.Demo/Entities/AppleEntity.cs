using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class AppleEntity : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Apple.png");

        protected Image Image { get; set; }

        protected Wiggler Wiggler { get; private set; }

        public bool IsWiggling => Wiggler != null && Wiggler.Counter > 0f;
        

        public AppleEntity()
            : base()
        {
            Collider = new RectangleCollider(40f, 48f, -20f, -20f);
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            AddComponents(Image);
        }

        public bool IsClicked(Vector2 point)
        {
            return Collider?.Contains(point) ?? false;
        }

        public void Wiggle()
        {
            if (!IsWiggling)
            {
                Wiggler = Wiggler.CreateAndApply(this, .5f, 5f, WigglerMode.EaseOut, OnWiggle);
            }
        }

        private void OnWiggle(float offset)
        {
            X += offset * 2f;
        }
    }
}
