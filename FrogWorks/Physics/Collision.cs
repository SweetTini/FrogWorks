using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Collision
    {
        #region SAT Method
        public static bool Overlaps(Shape shapeA, Shape shapeB)
        {
            if (IsCircle(shapeA) && IsCircle(shapeB))
                return CirclesOverlap(shapeA as Circle, shapeB as Circle);

            var axesA = shapeA.GetAxes(shapeB.GetFocis());
            var axesB = shapeB.GetAxes(shapeA.GetFocis());
            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length
                    ? axesA[i]
                    : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    float minA, maxA, minB, maxB;
                    shapeA.Project(axis, out minA, out maxA);
                    shapeB.Project(axis, out minB, out maxB);

                    var dist = Shape.GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;
                }
            }

            return true;
        }

        public static bool Overlaps(Shape shapeA, Shape shapeB, out Manifold hit)
        {
            if (IsCircle(shapeA) && IsCircle(shapeB))
                return CirclesOverlap(shapeA as Circle, shapeB as Circle, out hit);

            hit = default;

            var depth = float.PositiveInfinity;
            var normal = Vector2.Zero;
            var axesA = shapeA.GetAxes(shapeB.GetFocis());
            var axesB = shapeB.GetAxes(shapeA.GetFocis());
            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length
                    ? axesA[i]
                    : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    float minA, maxA, minB, maxB;
                    shapeA.Project(axis, out minA, out maxA);
                    shapeB.Project(axis, out minB, out maxB);

                    var dist = Shape.GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;

                    var contained =
                        Shape.IsIntervalContained(minA, maxA, minB, maxB) ||
                        Shape.IsIntervalContained(minB, maxB, minA, maxA);

                    if (contained)
                    {
                        var min = (minA - minB).Abs();
                        var max = (maxA - maxB).Abs();

                        dist += max > min ? min : max;
                        if (max > min) axis = -axis;
                    }

                    if (depth > dist)
                    {
                        depth = dist;
                        normal = axis;
                    }
                }
            }

            var center = Vector2.Dot(shapeA.Center - shapeB.Center, normal);
            if (center <= 0) normal = -normal;

            hit.Depth = depth;
            hit.Normal = normal;
            return true;
        }

        static bool CirclesOverlap(Circle circleA, Circle circleB)
        {
            var dist = circleA.Center - circleB.Center;
            var radius = circleA.Radius + circleB.Radius;
            return dist.LengthSquared() < radius * radius;
        }

        static bool CirclesOverlap(Circle circleA, Circle circleB, out Manifold hit)
        {
            hit = default;

            var dist = circleA.Center - circleB.Center;
            var distSq = dist.LengthSquared();
            var radius = circleA.Radius + circleB.Radius;

            if (distSq < radius * radius)
            {
                var length = distSq.Sqrt();
                var normal = length != 0f
                    ? dist * length.Inverse()
                    : Vector2.UnitY;

                hit.Normal = normal;
                hit.Depth = radius - length;
                return true;
            }

            return false;
        }

        static bool IsCircle(Shape shape)
        {
            return shape is Circle;
        }
        #endregion
    }
}
