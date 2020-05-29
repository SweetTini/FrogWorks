using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public static class Collision
    {
        #region Raycasting
        public static bool CastRay(
            Vector2 origin,
            Vector2 normal,
            float distance,
            Shape shape,
            out Raycast hit)
        {
            hit = default;

            if (shape is Circle)
                return CastRay(origin, normal, distance, shape as Circle, out hit);

            var vertices = shape.GetVertices();
            var polyNorms = vertices.Normalize();
            var polyNorm = Vector2.Zero;
            var minDist = 0f;
            var maxDist = distance;

            for (int i = 0; i < vertices.Length; i++)
            {
                var num = Vector2.Dot(polyNorms[i], vertices[i] - origin);
                var denom = Vector2.Dot(polyNorms[i], normal);
                if (denom == 0f && num < 0f) return false;

                if (denom < 0f && num < minDist * denom)
                {
                    minDist = num / denom;
                    polyNorm = polyNorms[i];
                }
                else if (denom > 0f && num < maxDist * denom)
                {
                    maxDist = num / denom;
                }

                if (maxDist < minDist) return false;
            }

            if (polyNorm != Vector2.Zero)
            {
                hit.Contact = origin + normal * minDist;
                hit.Normal = polyNorm;
                hit.Depth = distance - minDist;
                hit.Percent = minDist.Divide(distance);
                hit.Distance = minDist;

                return true;
            }

            return false;
        }

        static bool CastRay(
            Vector2 origin,
            Vector2 normal,
            float distance,
            Circle circ,
            out Raycast hit)
        {
            hit = default;

            var m = origin - circ.Center;
            var b = Vector2.Dot(m, normal);
            var c = Vector2.Dot(m, m) - circ.Radius * circ.Radius;
            if (b > 0f && c > 0f) return false;

            var discr = b * b - c;
            if (discr < 0f) return false;

            var depth = -b - discr.Sqrt();

            if (depth.Between(0f, distance))
            {
                var contact = origin + normal * depth;

                hit.Contact = contact;
                hit.Normal = Vector2.Normalize(contact - circ.Center);
                hit.Depth = distance - depth;
                hit.Percent = depth.Divide(distance);
                hit.Distance = depth;

                return true;
            }

            return false;
        }
        #endregion

        #region SAT Method

        #region Basic
        public static bool Overlaps(Shape shapeA, Shape shapeB)
        {
            if (shapeA is Box)
            {
                if (shapeB is Box)
                    return Overlaps(shapeA as Box, shapeB as Box);
                else if (shapeB is Circle)
                    return Overlaps(shapeA as Box, shapeB as Circle);
                else if (shapeB is Polygon)
                    return Overlaps(shapeA as Box, shapeB as Polygon);
            }
            else if (shapeA is Circle)
            {
                if (shapeB is Box)
                    return Overlaps(shapeA as Circle, shapeB as Box);
                else if (shapeB is Circle)
                    return Overlaps(shapeA as Circle, shapeB as Circle);
                else if (shapeB is Polygon)
                    return Overlaps(shapeA as Circle, shapeB as Polygon);
            }
            else if (shapeA is Polygon)
            {
                Overlaps(shapeA as Polygon, shapeB);
            }

            return false;
        }

        public static bool Overlaps(Box boxA, Box boxB)
        {
            return boxB.Max.X > boxA.Min.X && boxA.Max.X > boxB.Min.X
                && boxB.Max.Y > boxA.Min.Y && boxA.Max.Y > boxB.Min.Y;
        }

        public static bool Overlaps(Box box, Circle circ)
        {
            return Overlaps(circ, box);
        }

        public static bool Overlaps(Box box, Polygon poly)
        {
            return Overlaps(poly, box);
        }

        public static bool Overlaps(Circle circ, Box box)
        {
            var distance = circ.Center - circ.Center.Clamp(box.Min, box.Max);
            return distance.LengthSquared() < circ.Radius * circ.Radius;
        }

        public static bool Overlaps(Circle circA, Circle circB)
        {
            var distance = circA.Center - circB.Center;
            var radius = circA.Radius + circB.Radius;
            return distance.LengthSquared() < radius * radius;
        }

        public static bool Overlaps(Circle circ, Polygon poly)
        {
            return Overlaps(poly, circ);
        }

        public static bool Overlaps(Polygon poly, Shape shape)
        {
            var polyAxes = poly.GetVertices().Normalize();
            var shapeAxes = shape is Circle
                ? new Vector2[] { Vector2.Normalize(shape.Center - poly.GetClosestPoint(shape.Center)) }
                : shape.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < polyAxes.Length + shapeAxes.Length; i++)
            {
                axis = i < polyAxes.Length ? polyAxes[i] : shapeAxes[i - polyAxes.Length];

                if (axis != Vector2.Zero)
                {
                    poly.Project(axis, out float minA, out float maxA);
                    shape.Project(axis, out float minB, out float maxB);

                    var dist = GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;
                }
            }

            return true;
        }
        #endregion

        #region Advanced
        public static bool Overlaps(Shape shapeA, Shape shapeB, out Manifold hit)
        {
            hit = default;

            if (shapeA is Box)
            {
                if (shapeB is Box)
                    return Overlaps(shapeA as Box, shapeB as Box, out hit);
                else if (shapeB is Circle)
                    return Overlaps(shapeA as Box, shapeB as Circle, out hit);
                else if (shapeB is Polygon)
                    return Overlaps(shapeA as Box, shapeB as Polygon, out hit);
            }
            else if (shapeA is Circle)
            {
                if (shapeB is Box)
                    return Overlaps(shapeA as Circle, shapeB as Box, out hit);
                else if (shapeB is Circle)
                    return Overlaps(shapeA as Circle, shapeB as Circle, out hit);
                else if (shapeB is Polygon)
                    return Overlaps(shapeA as Circle, shapeB as Polygon, out hit);
            }
            else if (shapeA is Polygon)
            {
                return Overlaps(shapeA as Polygon, shapeB, out hit);
            }

            return false;
        }

        public static bool Overlaps(Box boxA, Box boxB, out Manifold hit)
        {
            hit = default;

            var halfA = boxA.Size / 2f;
            var halfB = boxB.Size / 2f;
            var distance = boxA.Center - boxB.Center;
            var offset = halfA + halfB - distance.Abs();
            if (offset.X < 0f || offset.Y < 0f) return false;

            var depth = offset.X < offset.Y ? offset.X : offset.Y;
            var normal = offset.X < offset.Y ? Vector2.UnitX : Vector2.UnitY;
            var factor = offset.X < offset.Y ? distance.X : distance.Y;

            normal *= factor < 0 ? -1 : 1;
            hit.Normal = normal;
            hit.Depth = depth;

            return true;
        }

        public static bool Overlaps(Box box, Circle circ, out Manifold hit)
        {
            var overlaps = Overlaps(circ, box, out hit);
            hit.Normal = -hit.Normal;
            return overlaps;
        }

        public static bool Overlaps(Box box, Polygon poly, out Manifold hit)
        {
            var overlaps = Overlaps(poly, box, out hit);
            hit.Normal = -hit.Normal;
            return overlaps;
        }

        public static bool Overlaps(Circle circ, Box box, out Manifold hit)
        {
            hit = default;

            var distance = circ.Center - circ.Center.Clamp(box.Min, box.Max);
            var distanceSq = Vector2.Dot(distance, distance);
            var radiusSq = circ.Radius * circ.Radius;

            if (distanceSq < radiusSq)
            {
                if (distanceSq != 0f)
                {
                    var length = distanceSq.Sqrt();
                    var normal = Vector2.Normalize(distance);

                    hit.Normal = normal;
                    hit.Depth = circ.Radius - length;
                }
                else
                {
                    var halfSize = box.Size / 2f;
                    var length = circ.Center - box.Center;
                    var offset = halfSize - length.Abs();

                    var depth = offset.X < offset.Y ? offset.X : offset.Y;
                    var normal = offset.X < offset.Y ? Vector2.UnitX : Vector2.UnitY;
                    var factor = offset.X < offset.Y ? length.X : length.Y;

                    normal *= factor < 0 ? -1 : 1;
                    hit.Normal = normal;
                    hit.Depth = circ.Radius + depth;
                }

                return true;
            }

            return false;
        }

        public static bool Overlaps(Circle circA, Circle circB, out Manifold hit)
        {
            hit = default;

            var distance = circA.Center - circB.Center;
            var distanceSq = distance.LengthSquared();
            var radius = circA.Radius + circB.Radius;

            if (distanceSq < radius * radius)
            {
                var length = distanceSq.Sqrt();
                var normal = length != 0f
                    ? distance * length.Inverse()
                    : Vector2.UnitY;

                hit.Normal = normal;
                hit.Depth = radius - length;
                return true;
            }

            return false;
        }

        public static bool Overlaps(Circle circ, Polygon poly, out Manifold hit)
        {
            var overlaps = Overlaps(poly, circ, out hit);
            hit.Normal = -hit.Normal;
            return overlaps;
        }

        public static bool Overlaps(Polygon poly, Shape shape, out Manifold hit)
        {
            hit = default;

            var normal = Vector2.Zero;
            var depth = float.PositiveInfinity;
            var polyAxes = poly.GetVertices().Normalize();
            var shapeAxes = shape is Circle
                ? new Vector2[] { Vector2.Normalize(shape.Center - poly.GetClosestPoint(shape.Center)) }
                : shape.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < polyAxes.Length + shapeAxes.Length; i++)
            {
                axis = i < polyAxes.Length ? polyAxes[i] : shapeAxes[i - polyAxes.Length];

                if (axis != Vector2.Zero)
                {
                    poly.Project(axis, out float minA, out float maxA);
                    shape.Project(axis, out float minB, out float maxB);

                    var dist = GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;

                    if (ContainsIntervals(minA, maxA, minB, maxB))
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

            var offset = Vector2.Dot(poly.Center - shape.Center, normal);
            if (offset < 0) normal = -normal;

            hit.Depth = depth;
            hit.Normal = normal;

            return true;
        }
        #endregion

        #region Misc.
        static void Project(this Shape shape, Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            if (shape is Circle)
            {
                var dotProd = Vector2.Dot(shape.Center, axis);
                var radius = (shape as Circle).Radius;

                min = dotProd - radius;
                max = dotProd + radius;
            }
            else
            {
                var vertices = shape.GetVertices();
                float dotProd;

                for (int i = 0; i < vertices.Length; i++)
                {
                    dotProd = Vector2.Dot(vertices[i], axis);

                    if (min > dotProd) min = dotProd;
                    if (max < dotProd) max = dotProd;
                }
            }
        }

        static Vector2 GetClosestPoint(this Polygon poly, Vector2 point)
        {
            var vertices = poly.GetVertices();
            var minDistance = float.PositiveInfinity;
            var closest = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var next = vertices[i];
                var distance = (point - next).LengthSquared();

                if (minDistance > distance)
                {
                    minDistance = distance;
                    closest = next;
                }
            }

            return closest;
        }

        static float GetIntervalDepth(float minA, float maxA, float minB, float maxB)
        {
            var outside = minA > maxB || minB > maxA;

            if (!outside)
            {
                var min = maxA.Min(maxB);
                var max = minA.Max(minB);
                return min - max;
            }

            return 0f;
        }

        static bool ContainsIntervals(float minA, float maxA, float minB, float maxB)
        {
            return (minA > minB && maxA < maxB)
                || (minB > minA && maxB < maxA);
        }
        #endregion

        #endregion
    }
}
