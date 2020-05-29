using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct AABB
    {
        public Vector2 Min { get; internal set; }

        public Vector2 Max { get; internal set; }

        public Vector2 Center => (Min + Max) / 2f;

        public Vector2 Size => Max - Min;

        public float Area => 2f * Size.X * Size.Y;

        internal AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public AABB Merge(AABB aabb)
        {
            return new AABB(
                Vector2.Min(Min, aabb.Min),
                Vector2.Max(Max, aabb.Max));
        }

        public AABB Expand(float amount)
        {
            return new AABB(
                Min - Vector2.One * amount,
                Max + Vector2.One * amount);
        }

        public bool Contains(AABB aabb)
        {
            return aabb.Min.X >= Min.X
                && aabb.Max.X <= Max.X
                && aabb.Min.Y >= Min.Y
                && aabb.Max.Y <= Max.Y;
        }

        public bool Overlaps(AABB aabb)
        {
            return aabb.Max.X > Min.X
                && aabb.Min.X < Max.X
                && aabb.Max.Y > Min.Y
                && aabb.Min.Y < Max.Y;
        }
    }
}
