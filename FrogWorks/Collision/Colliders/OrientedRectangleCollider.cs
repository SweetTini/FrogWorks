using System;

namespace FrogWorks
{
    public class OrientedRectangleCollider : PolygonCollider
    {
        public OrientedRectangleCollider(float width, float height, float offsetX = 0f, float offsetY = 0f)
            : base(new RectangleF(0f, 0f, Math.Abs(width), Math.Abs(height)).ToVertices(), offsetX, offsetY)
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
