using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Shape
    {
        Vector2 _position;
        Rectangle _bounds;
        bool _isDirty = true;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    MarkAsDirty();
                    OnTranslated();
                }
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public abstract Vector2 Center { get; set; }

        public float CenterX
        {
            get { return Center.X; }
            set { Center = new Vector2(value, Center.Y); }
        }

        public float CenterY
        {
            get { return Center.Y; }
            set { Center = new Vector2(Center.X, value); }
        }

        public Rectangle Bounds
        {
            get
            {
                if (_isDirty)
                    RecalculateBoundsInternally();

                return _bounds;
            }
        }

        protected Shape(Vector2 position)
        {
            _position = position;
        }

        internal void RecalculateBoundsInternally()
        {
            _bounds = RecalculateBounds();
        }

        protected abstract Rectangle RecalculateBounds();

        public abstract void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null);

        public abstract Shape Clone();

        protected void MarkAsDirty()
        {
            _isDirty = true;
        }

        protected virtual void OnTranslated()
        {
        }
    }
}
