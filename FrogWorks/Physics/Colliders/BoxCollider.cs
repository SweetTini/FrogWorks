using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class BoxCollider : ShapeCollider
    {
        Box _box;

        protected internal override Shape Shape => _box;

        public override Vector2 Size
        {
            get { return _box.Size; }
            set
            {
                value = value.Abs();

                if (_box.Size != value)
                {
                    _box.Size = value;
                    OnTransformedInternally();
                }
            }
        }

        public override Vector2 Min
        {
            get { return AbsolutePosition; }
            set { AbsolutePosition = value; }
        }

        public override Vector2 Max
        {
            get { return AbsolutePosition + Size; }
            set { AbsolutePosition = value - Size; }
        }

        public BoxCollider(float width, float height)
            : this(Vector2.Zero, new Vector2(width, height))
        {
        }

        public BoxCollider(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public BoxCollider(Vector2 size)
            : this(Vector2.Zero, size)
        {
        }

        public BoxCollider(Vector2 position, Vector2 size)
            : base(position)
        {
            _box = new Box(AbsolutePosition, size);
        }

        public override Collider Clone()
        {
            return new BoxCollider(Position, Size);
        }
    }
}
