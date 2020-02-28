using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public sealed class Box : Shape
    {
        Vector2 _size;

        public override Vector2 Center
        {
            get { return Position + Size * .5f; }
            set { Position = value - Size * .5f; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                value = value.Abs();

                if (_size != value)
                {
                    _size = value;
                    MarkAsDirty();
                }
            }
        }

        public float Width
        {
            get { return Size.X; }
            set { Size = new Vector2(value, Size.Y); }
        }

        public float Height
        {
            get { return Size.Y; }
            set { Size = new Vector2(Size.X, value); }
        }

        public Vector2 Min => Position;

        public Vector2 Max => Position + Size;

        public Box(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public Box(Vector2 position, Vector2 size)
            : base(position)
        {
            _size = size;
        }

        protected override Rectangle RecalculateBounds()
        {
            return new Rectangle(Position.ToPoint(), Size.ToPoint());
        }

        public override bool Contains(Vector2 point)
        {
            return Min.X <= point.X && point.X < Max.X
                && Min.Y <= point.Y && point.Y < Max.Y;
        }

        public override void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null)
        {
            batch.DrawPrimitives(p =>
            {
                if (fillColor.HasValue)
                    p.FillRectangle(Position, Size, fillColor.Value);

                p.DrawRectangle(Position, Size, strokeColor);
            });
        }

        public override Shape Clone()
        {
            return new Box(Position, Size);
        }

        Vector2[] GetVertices()
        {
            return new Vector2[]
            {
                Position,
                Position + Size * Vector2.UnitX,
                Position + Size,
                Position + Size * Vector2.UnitY
            };
        }

        internal override Vector2[] GetFocis()
        {
            return null;
        }

        internal override Vector2[] GetAxes(Vector2[] focis)
        {
            return GetAxes(GetVertices(), focis);
        }

        internal override void Project(Vector2 axis, out float min, out float max)
        {
            Project(GetVertices(), axis, out min, out max);
        }
    }
}
