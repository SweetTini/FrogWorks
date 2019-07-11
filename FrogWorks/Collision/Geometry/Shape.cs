using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Shape
    {
        public virtual Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.X; }
            set { Position = new Vector2(Position.X, value); }
        }

        public abstract Rectangle Bounds { get; }

        public abstract void Draw(RendererBatch batch, Color color, bool fill = false);

        public abstract bool Contains(Vector2 point);

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }
    }
}
