using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class OrientedRectangleCollider : PolygonCollider
    {
        public OrientedRectangleCollider(float width, float height, float offsetX = 0f, float offsetY = 0f)
            : base(new RectangleF(Vector2.Zero, new Vector2(width, height).Abs()).ToVertices(), offsetX, offsetY)
        {
        }

        public override Collider Clone()
        {
            return new OrientedRectangleCollider(Width, Height, X, Y)
            {
                Origin = Origin,
                Scale = Scale,
                Angle = Angle
            };
        }
    }
}
