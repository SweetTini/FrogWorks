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
                OnTransformedInternally();
            }
        }

        public RectangleCollider(Vector2 size)
            : this(size, Vector2.Zero)
        {
        }

        public RectangleCollider(Vector2 size, Vector2 offset)
            : base(offset)
        {
            _size = size.Abs();
        }

        public RectangleCollider(float width, float height)
            : this(new Vector2(width, height), Vector2.Zero)
        {
        }

        public RectangleCollider(float width, float height, float offsetX, float offsetY)
            : this(new Vector2(width, height), new Vector2(offsetX, offsetY))
        {
        }

        public override Collider Clone()
        {
            return new RectangleCollider(Size, Position);
        }
    }
}
