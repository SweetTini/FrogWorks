using System;

namespace FrogWorks
{
    public class OrientedBoxCollider : PolygonCollider
    {
        public OrientedBoxCollider(float width, float height, float offsetX = 0f, float offsetY = 0f)
            : base(new RectangleF(0f, 0f, Math.Abs(width), Math.Abs(height)).ToVertices(), offsetX, offsetY)
        {
        }

        public override Collider Clone()
        {
            return new OrientedBoxCollider(Width, Height, X, Y)
            {
                Origin = Origin,
                Scale = Scale,
                Angle = Angle
            };
        }
    }
}
