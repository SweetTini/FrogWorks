using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Manifold
    {
        public Vector2 ContactPoint { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Depth { get; internal set; }
    }
}
