using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class PolygonCollider : ShapeCollider<Polygon>
    {
        public Vector2[] Vertices => Shape.Vertices;

        public Vector2 Origin
        {
            get { return Shape.Origin; }
            set
            {
                if (value == Shape.Origin) return;
                Shape.Origin = value;
                OnTransformed();
            }
        }

        public Vector2 Scale
        {
            get { return Shape.Scale; }
            set
            {
                if (value == Shape.Scale) return;
                Shape.Scale = value;
                OnTransformed();
            }
        }

        public float Angle
        {
            get { return Shape.Angle; }
            set
            {
                if (value == Shape.Angle) return;
                Shape.Angle = value;
                OnTransformed();
            }
        }

        public float AngleInDegrees
        {
            get { return Shape.AngleInDegrees; }
            set
            {
                if (value == Shape.AngleInDegrees) return;
                Shape.AngleInDegrees = value;
                OnTransformed();
            }
        }

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

        public PolygonCollider(Vector2[] vertices, float offsetX = 0f, float offsetY = 0f)
            : base()
        {
            Shape = new Polygon(vertices);
            Position = new Vector2(offsetX, offsetY);
        }

        public override Collider Clone()
        {
            return new PolygonCollider(Shape.Original, X, Y)
            {
                Origin = Origin,
                Scale = Scale,
                Angle = Angle
            };
        }
    }
}
