using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Raycast
    {
        public Collider Collider { get; internal set; }

        public Vector2 Contact { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }

        public float Percent { get; internal set; }

        public float Distance { get; internal set; }
    }
}
