using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class PolygonCollider : ShapeCollider<Polygon>
    {
        public Vector2[] Vertices => Shape.Transform;

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

        public override float Width
        {
            get { return Shape.Width; }
            set
            {
                value = Math.Abs(value);
                if (value == Shape.Width) return;
                Shape.Width = value;
                OnTransformed();
            }
        }

        public override float Height
        {
            get { return Shape.Height; }
            set
            {
                value = Math.Abs(value);
                if (value == Shape.Height) return;
                Shape.Height = value;
                OnTransformed();
            }
        }

        public PolygonCollider(Vector2[] vertices, float offsetX, float offsetY)
            : base()
        {
            Shape = new Polygon(vertices);
            Position = new Vector2(offsetX, offsetY);
        }

        public override void Draw(RendererBatch batch, Color color)
        {
            batch.DrawPrimitives((primitive) => primitive.DrawPolygon(Vertices, color));
        }

        public override Collider Clone()
        {
            return new PolygonCollider(Shape.Vertices, X, Y)
            {
                Origin = Origin,
                Scale = Scale,
                Angle = Angle
            };
        }
    }
}
