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
            if (shape is Circle)
                return RaycastCircle(start, end, shape as Circle, out hit);

            hit = default;

            var ray = new Ray2D(start, end);
            var vertices = shape.GetVertices();
            var normals = vertices.Normalize();
            var minDepth = 0f;
            var maxDepth = ray.Length;
            var normal = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var num = Vector2.Dot(normals[i], vertices[i] - ray.Start);
                var denom = Vector2.Dot(normals[i], ray.Normal);
                if (denom == 0f && num < 0f) return false;

                if (denom < 0f && num < minDepth * denom)
                {
                    minDepth = num / denom;
                    normal = normals[i];
                }
                else if (denom > 0f && num < maxDepth * denom)
                {
                    maxDepth = num / denom;
                }

                if (maxDepth < minDepth) return false;
            }

            if (normal != Vector2.Zero)
            {
                hit.Contact = ray.Start + ray.Normal * minDepth;
                hit.Normal = normal;
                hit.Depth = minDepth;
                return true;
            }

            return false;
        }

        public static bool Raycast(
            Vector2 start,
            Vector2 normal,
            float distance,
            Shape shape,
            out Raycast hit)
        {
            return Raycast(start, start + normal * distance, shape, out hit);
        }

        static bool RaycastCircle(
            Vector2 start,
            Vector2 end,
            Circle circle,
            out Raycast hit)
        {
            hit = default;

            var ray = new Ray2D(start, end);
            var m = ray.Start - circle.Center;
            var b = Vector2.Dot(m, ray.Normal);
            var c = Vector2.Dot(m, m) - circle.Radius * circle.Radius;
            if (b > 0f && c > 0f) return false;

            var discr = b * b - c;
            if (discr < 0f) return false;

            var depth = -b - discr.Sqrt();

            if (depth.Between(0f, ray.Length))
            {
                var contact = ray.Start + ray.Normal * depth;
                hit.Contact = contact;
                hit.Depth = depth;
                hit.Normal = Vector2.Normalize(contact - circle.Center);
                return true;
            }

            return false;
        }
        #endregion

        #region SAT Method
        public static bool Overlaps(Shape shapeA, Shape shapeB)
        {
            if (shapeA is Circle && shapeB is Circle)
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
            if (shapeA is Circle && shapeB is Circle)
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

                    if (Shape.IntervalsContained(minA, maxA, minB, maxB))
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

            var center = shapeA.Center - shapeB.Center;
            var offset = Vector2.Dot(center, normal);
            if (offset < 0) normal = -normal;

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
    }
}
