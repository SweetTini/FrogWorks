using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider : Collider
    {
        protected internal abstract Shape Shape { get; }

        protected ShapeCollider(Vector2 position)
            : base(position)
        {
        }

        public sealed override bool Contains(Vector2 point)
        {
            return Shape.Contains(point);
        }

        public sealed override bool Raycast(Vector2 start, Vector2 end, out Raycast hit)
        {
            return Shape.Raycast(start, end, out hit);
        }

        public sealed override bool Overlaps(Shape shape)
        {
            return Shape.Overlaps(shape);
        }

        public sealed override bool Overlaps(Shape shape, out Manifold hit)
        {
            return Shape.Overlaps(shape, out hit);
        }

        public sealed override void Draw(RendererBatch batch, Color color)
        {
            Shape.Draw(batch, color);
        }

        protected sealed override void OnTransformed()
        {
            Shape.Position = AbsolutePosition;
        }
    }
}
