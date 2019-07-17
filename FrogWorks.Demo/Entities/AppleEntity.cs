using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class AppleEntity : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Apple.png");

        protected Image Image { get; set; }

        public bool IsWiggling { get; private set; }
        

        public AppleEntity()
            : base()
        {
            Collider = new RectangleCollider(40f, 48f, -20f, -20f);
            Image = new Image(Texture, true);
            Image.CenterOrigin();
            AddComponents(Image);
        }

        public bool Contains(Vector2 point)
        {
            return Collider.Contains(point);
        }

        public void Wiggle()
        {
            if (!IsWiggling)
            {
                Wiggler.CreateAndApply(this, .5f, 5f, WigglerMode.EaseOut, OnWiggle, ResetWiggle);
                IsWiggling = true;
            }
        }

        private void OnWiggle(float offset)
        {
            Image.X = offset * 4f;
        }

        private void ResetWiggle()
        {
            IsWiggling = false;
        }
    }
}
