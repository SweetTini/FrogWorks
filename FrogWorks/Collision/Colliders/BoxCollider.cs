using System;
using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class BoxCollider : ShapeCollider<RectangleF>
    {
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

        public BoxCollider(float width, float height, float offsetX = 0f, float offsetY = 0f)
            : base()
        {
            Shape = new RectangleF(0f, 0f, Math.Abs(width), Math.Abs(height));
            Position = new Vector2(offsetX, offsetY);
        }

        public override void Draw(RendererBatch batch, Color color)
        {
            batch.DrawPrimitives((primitive) => primitive.DrawRectangle(Shape.Position, Shape.Size, color));
        }

        public override Collider Clone()
        {
            return new BoxCollider(Width, Height, X, Y);
        }
    }
}
