using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Ray2D
    {
        public Vector2 Start { get; set; }

        public Vector2 End { get; set; }

        public Vector2 Edge => End - Start;

        public Vector2 Normal => Vector2.Normalize(End - Start);

        public float Length => Vector2.Distance(Start, End);

        public Ray2D(Vector2 start, Vector2 end)
            : this()
        {
            Start = start;
            End = end;
        }

        public Ray2D(float x1, float y1, float x2, float y2)
            : this(new Vector2(x1, y1), new Vector2(x2, y2))
        {
        }
    }
}
