using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Raycast
    {
        public Vector2 Contact { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        internal Raycast(
            Vector2 contact,
            Vector2 normal,
            float depth)
            : this()
        {
            Contact = contact;
            Normal = normal;
            Depth = depth;
        }
    }
}
