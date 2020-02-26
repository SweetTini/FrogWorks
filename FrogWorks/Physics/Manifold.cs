using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Manifold
    {
        public Vector2 ContactA { get; internal set; }

        public Vector2 ContactB { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        internal Manifold(
            Vector2 contact,
            Vector2 normal,
            float depth)
            : this(contact, default, normal, depth)
        {
        }

        internal Manifold(
            Vector2 contactA,
            Vector2 contactB,
            Vector2 normal,
            float depth)
            : this()
        {
            ContactA = contactA;
            ContactB = contactB;
            Normal = normal;
            Depth = depth;
        }
    }
}
