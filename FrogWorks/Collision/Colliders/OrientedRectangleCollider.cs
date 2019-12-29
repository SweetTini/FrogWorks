using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class OrientedRectangleCollider : PolygonCollider
    {
        public OrientedRectangleCollider(Vector2 size)
            : this(size, Vector2.Zero)
        {
        }

        public OrientedRectangleCollider(Vector2 size, Vector2 offset)
            : base(new RectangleF(Vector2.Zero, size.Abs()).ToVertices(), offset)
        {
        }

        public OrientedRectangleCollider(float width, float height)
            : this(new Vector2(width, height), Vector2.Zero)
        {
        }

        public OrientedRectangleCollider(float width, float height, float offsetX, float offsetY) 
            : this(new Vector2(width, height), new Vector2(offsetX, offsetY))
        {
        }
    }
}
