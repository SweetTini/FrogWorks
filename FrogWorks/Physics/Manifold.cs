using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Manifold
    {
        public Collider Collider { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        public Vector2 Translation => Normal * Depth;

        internal Manifold(Collider collider, Vector2 normal, float depth)
            : this()
        {
            Collider = collider;
            Normal = normal;
            Depth = depth;
        }
    }
}
