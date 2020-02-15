using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct Raycast
    {
        public Ray2D Ray { get; internal set; }

        public Vector2 Normal { get; internal set; }

        public float Impact { get; internal set; }

        public Vector2 Startpoint => Ray.Position + Ray.Normal * (Ray.Distance * Impact);

        public Vector2 Endpoint => Startpoint + Normal * (Ray.Distance - Ray.Distance * Impact);

        internal Raycast(Ray2D ray)
            : this()
        {
            Ray = ray;
        }
    }
}
