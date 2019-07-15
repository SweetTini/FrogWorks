using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class RectangleCollider : ShapeCollider<RectangleF>
    {
        public override Vector2 Size
        {
            get { return Shape.Size; }
            set
            {
                value = value.Abs();
                if (value == Shape.Size) return;
                Shape.Size = value;
                OnTransformed();
            }
        }

        public RectangleCollider(float width, float height, float offsetX = 0, float offsetY = 0f)
            : base()
        {
            Shape = new RectangleF(Vector2.Zero, new Vector2(width, height).Abs());
            Position = new Vector2(offsetX, offsetY);
        }

        public override Collider Clone()
        {
            return new RectangleCollider(Width, Height, X, Y);
        }
    }
}
