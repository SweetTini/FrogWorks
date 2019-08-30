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
    }
}
