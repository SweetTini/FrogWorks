using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Collision
    {
        #region Raycasting
        public static bool Raycast(
            Vector2 start,
            Vector2 end,
            Shape shape,
            out Raycast hit)
        {
            if (IsCircle(shape))
                return RaycastOnCircle(start, end, shape as Circle, out hit);

            hit = default;

            var ray = new Ray2D(start, end);
            var vertices = shape.GetVertices();
            var normals = vertices.Normalize();
            var min = 0f;
            var max = ray.Length;
            var normal = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var num = Vector2.Dot(normals[i], vertices[i] - ray.Start);
                var den = Vector2.Dot(normals[i], ray.Normal);
                if (den == 0f && num < 0f) return false;

                if (den < 0f && num < min * den)
                {
                    min = num / den;
                    normal = normals[i];
                }
                else if (den > 0f && num < max * den)
                {
                    max = num / den;
                }

                if (max < min) return false;
            }

            if (normal != Vector2.Zero)
            {
                hit.Contact = ray.Start + ray.Normal * min;
                hit.Normal = normal;
                hit.Depth = min;
                return true;
            }

            return false;
        }

        static bool RaycastOnCircle(
            Vector2 start,
            Vector2 end,
            Circle circle,
            out Raycast hit)
        {
            hit = default;

            var ray = new Ray2D(start, end);
            var m = circle.Center - ray.Start;
            var c = Vector2.Dot(m, m) - circle.Radius * circle.Radius;
            var b = Vector2.Dot(m, ray.Normal);
            var distSq = b * b - c;
            if (distSq < 0f) return false;

            var dist = -b - distSq.Sqrt();

            if (dist.Between(0f, ray.Length))
            {
                var contact = ray.Start + ray.Normal * dist;
                hit.Contact = contact;
                hit.Depth = dist;
                hit.Normal = Vector2.Normalize(contact - circle.Center);
                return true;
            }

            return false;
        }
        #endregion

        #region Separate Axis Test
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
        #endregion

        #region Helper Methods
        static bool IsCircle(Shape shape)
        {
            return shape is Circle;
        }
        #endregion
    }
}
