using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class RectangleCollider : ShapeCollider
    {
        private Vector2 _size;

        protected internal override Shape Shape => new RectangleF(AbsolutePosition, _size);

        public override Vector2 Size
        {
            get { return _size; }
            set
            {
                value = value.Abs();
                if (value == _size) return;
                _size = value;
                OnInternalTransformed();
            }
        }

        public RectangleCollider(float width, float height, float x = 0f, float y = 0f)
            : base(new Vector2(x, y))
        {
            _size = new Vector2(width, height).Abs();
        }

        public override Collider Clone() => new RectangleCollider(Width, Height, X, Y);
    }
}
