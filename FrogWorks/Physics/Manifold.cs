using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Manifold
    {
        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        public Vector2 Translation => Normal * Depth;

        internal Manifold(Vector2 normal, float depth)
            : this()
        {
            Normal = normal;
            Depth = depth;
        }
    }
}
