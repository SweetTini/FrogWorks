using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Raycast
    {
        public Collider Collider { get; internal set; }

        public Vector2 Contact { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        internal Raycast(
            Collider collider,
            Vector2 contact,
            Vector2 normal,
            float depth)
            : this()
        {
            Collider = collider;
            Contact = contact;
            Normal = normal;
            Depth = depth;
        }
    }
}
