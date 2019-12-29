using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Ray
    {
        public Vector2 Position { get; set; }

        public Vector2 Normal { get; set; }

        public float Distance { get; set; }

        public Vector2 Endpoint => Position + Normal * Distance;

        public Vector2 ApplyImpact(float distance) => Position + Normal * distance;

        public Ray(Vector2 from, Vector2 to)
            : this()
        {
            Position = from;
            Normal = Vector2.Normalize(to - from);
            Distance = Vector2.Distance(from, to);
        }

        public Ray(float fromX, float fromY, float toX, float toY)
            : this(new Vector2(fromX, fromY), new Vector2(toX, toY))
        {
        }

        public Ray(Vector2 position, Vector2 normal, float distance)
            : this()
        {
            Position = position;
            Normal = normal;
            Distance = distance;
        }

        public Ray(float x, float y, float normalX, float normalY, float distance)
            : this(new Vector2(x, y), new Vector2(normalX, normalY), distance)
        {
        }
    }
}
