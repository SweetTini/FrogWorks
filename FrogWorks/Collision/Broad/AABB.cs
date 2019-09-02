using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct AABB
    {
        public Vector2 Upper { get; set; }

        public Vector2 Lower { get; set; }

        public Vector2 Center => (Upper + Lower) / 2f;

        public Vector2 Size => Lower - Upper;

        public float SurfaceArea => 2f * Size.X * Size.Y;

        public AABB(Vector2 upper, Vector2 lower)
            : this()
        {
            Upper = upper;
            Lower = lower;
        }

        public AABB(float x1, float y1, float x2, float y2)
            : this(new Vector2(x1, y1), new Vector2(x2, y2))
        {
        }

        public AABB Merge(AABB other)
        {
            return Merge(other, 0f);
        }

        public AABB Merge(AABB other, float padding)
        {
            return Merge(other, Vector2.One * padding);
        }

        public AABB Merge(AABB other, Vector2 padding)
        {
            return new AABB(
                Vector2.Min(Upper, other.Upper) - padding, 
                Vector2.Max(Lower, other.Lower) + padding);
        }

        public bool Contains(AABB other)
        {
            return other.Upper.X >= Upper.X && Lower.X >= other.Lower.X
                && other.Upper.Y >= Upper.Y && Lower.Y >= other.Lower.Y;
        }

        public bool Overlaps(AABB other)
        {
            return other.Lower.X > Upper.X && Lower.X > other.Upper.X
                && other.Lower.Y > Upper.Y && Lower.Y > other.Upper.Y;
        }
    }

    public interface IAABBContainer
    {
        AABB AabbBounds { get; }
    }
}
