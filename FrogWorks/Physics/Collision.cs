using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Collision
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

            if (shape is Box) return CastRay(origin, normal, distance, shape as Box, out hit);
            if (shape is Circle) return CastRay(origin, normal, distance, shape as Circle, out hit);
            if (shape is Polygon) return CastRay(origin, normal, distance, shape as Polygon, out hit);

            return false;
        }

        public static bool CastRay(
            Vector2 origin,
            Vector2 normal,
            float distance,
            Box box,
            out Raycast hit)
        {
            hit = default;

            var inv = normal.Inverse();
            var dA = (box.Min - origin) * inv;
            var dB = (box.Max - origin) * inv;
            var vA = Vector2.Min(dA, dB);
            var vB = Vector2.Max(dA, dB);
            var lo = vA.X.Max(vA.Y);
            var hi = vB.X.Min(vB.Y);

            if (hi >= 0f && hi >= lo && lo <= distance)
            {
                var contact = origin + normal * lo;
                var center = contact - (box.Max * .5f);
                var unit = center.Abs().X > center.Abs().Y ? Vector2.UnitX : Vector2.UnitY;

                hit.Contact = contact;
                hit.Normal = center.Sign() * unit;
                hit.Depth = distance - lo;
                hit.Percent = lo.Divide(distance);
                hit.Distance = lo;

                return true;
            }

            return false;
        }

        public static bool CastRay(
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

        public static bool CastRay(
            Vector2 origin,
            Vector2 normal,
            float distance,
            Polygon poly,
            out Raycast hit)
        {
            hit = default;

            var vertices = poly.GetVertices();
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
        #endregion

        #region SAT Method

        #region Basic
        public static bool Overlaps(Shape shapeA, Shape shapeB)
        {
            if (shapeA is Box)
            {
                if (shapeB is Box) return Overlaps(shapeA as Box, shapeB as Box);
                if (shapeB is Circle) return Overlaps(shapeA as Box, shapeB as Circle);
                if (shapeB is Polygon) return Overlaps(shapeA as Box, shapeB as Polygon);
            }

            if (shapeA is Circle)
            {
                if (shapeB is Box) return Overlaps(shapeA as Circle, shapeB as Box);
                if (shapeB is Circle) return Overlaps(shapeA as Circle, shapeB as Circle);
                if (shapeB is Polygon) return Overlaps(shapeA as Circle, shapeB as Polygon);
            }

            if (shapeA is Polygon)
            {
                if (shapeB is Box) return Overlaps(shapeA as Polygon, shapeB as Box);
                if (shapeB is Circle) return Overlaps(shapeA as Polygon, shapeB as Circle);
                if (shapeB is Polygon) return Overlaps(shapeA as Polygon, shapeB as Polygon);
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

        public static bool Overlaps(Polygon poly, Box box)
        {
            var axesA = poly.GetVertices().Normalize();
            var axesB = box.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length ? axesA[i] : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    Project(poly, axis, out float minA, out float maxA);
                    Project(box, axis, out float minB, out float maxB);

                    var dist = GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;
                }
            }

            return true;
        }

        public static bool Overlaps(Polygon poly, Circle circ)
        {
            var axes = poly.GetVertices().Normalize();
            var center = circ.Center;
            var circleAxis = Vector2.Normalize(center - GetClosestPointOnPolygon(poly, center));

            Vector2 axis;

            for (int i = 0; i <= axes.Length; i++)
            {
                axis = i < axes.Length ? axes[i] : circleAxis;

                if (axis != Vector2.Zero)
                {
                    Project(poly, axis, out float minA, out float maxA);
                    Project(circ, axis, out float minB, out float maxB);

                    var dist = GetIntervalDepth(minA, maxA, minB, maxB);
                    if (dist == 0) return false;
                }
            }

            return true;
        }

        public static bool Overlaps(Polygon polyA, Polygon polyB)
        {
            var axesA = polyA.GetVertices().Normalize();
            var axesB = polyB.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length ? axesA[i] : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    Project(polyA, axis, out float minA, out float maxA);
                    Project(polyB, axis, out float minB, out float maxB);

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
                if (shapeB is Box) return Overlaps(shapeA as Box, shapeB as Box, out hit);
                if (shapeB is Circle) return Overlaps(shapeA as Box, shapeB as Circle, out hit);
                if (shapeB is Polygon) return Overlaps(shapeA as Box, shapeB as Polygon, out hit);
            }

            if (shapeA is Circle)
            {
                if (shapeB is Box) return Overlaps(shapeA as Circle, shapeB as Box, out hit);
                if (shapeB is Circle) return Overlaps(shapeA as Circle, shapeB as Circle, out hit);
                if (shapeB is Polygon) return Overlaps(shapeA as Circle, shapeB as Polygon, out hit);
            }

            if (shapeA is Polygon)
            {
                if (shapeB is Box) return Overlaps(shapeA as Polygon, shapeB as Box, out hit);
                if (shapeB is Circle) return Overlaps(shapeA as Polygon, shapeB as Circle, out hit);
                if (shapeB is Polygon) return Overlaps(shapeA as Polygon, shapeB as Polygon, out hit);
            }

            return false;
        }

        public static bool Overlaps(Box boxA, Box boxB, out Manifold hit)
        {
            hit = default;

            var halfA = boxA.Size * .5f;
            var halfB = boxB.Size * .5f;
            var distance = boxB.Center - boxA.Center;
            var offset = halfA + halfB - distance.Abs();
            if (offset.X < 0f || offset.Y < 0f) return false;

            var depth = offset.X < offset.Y ? offset.X : offset.Y;
            var normal = offset.X < offset.Y ? Vector2.UnitX : Vector2.UnitY;
            var factor = offset.X < offset.Y ? distance.X : distance.Y;

            normal *= factor < 0f ? -1f : 1f;
            hit.Normal = -normal;
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

            var distance = circ.Center.Clamp(box.Min, box.Max) - circ.Center;
            var distanceSq = Vector2.Dot(distance, distance);
            var radiusSq = circ.Radius * circ.Radius;

            if (distanceSq < radiusSq)
            {
                if (distanceSq != 0f)
                {
                    var length = distanceSq.Sqrt();
                    var normal = Vector2.Normalize(distance);

                    hit.Normal = -normal;
                    hit.Depth = circ.Radius - length;
                }
                else
                {
                    var halfSize = box.Size * .5f;
                    var length = circ.Center - box.Center;
                    var offset = halfSize - length.Abs();

                    var depth = offset.X < offset.Y ? offset.X : offset.Y;
                    var normal = offset.X < offset.Y ? Vector2.UnitX : Vector2.UnitY;
                    var factor = offset.X < offset.Y ? length.X : length.Y;

                    normal *= factor < 0 ? 1f : -1f;
                    hit.Normal = -normal;
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

        public static bool Overlaps(Polygon poly, Box box, out Manifold hit)
        {
            hit = default;

            var normal = Vector2.Zero;
            var depth = float.PositiveInfinity;
            var axesA = poly.GetVertices().Normalize();
            var axesB = box.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length ? axesA[i] : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    Project(poly, axis, out float minA, out float maxA);
                    Project(box, axis, out float minB, out float maxB);

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

            var offset = Vector2.Dot(poly.Center - box.Center, normal);
            if (offset < 0) normal = -normal;

            hit.Depth = depth;
            hit.Normal = normal;

            return true;
        }

        public static bool Overlaps(Polygon poly, Circle circ, out Manifold hit)
        {
            hit = default;

            var normal = Vector2.Zero;
            var depth = float.PositiveInfinity;
            var axes = poly.GetVertices().Normalize();
            var center = circ.Center;
            var circleAxis = Vector2.Normalize(center - GetClosestPointOnPolygon(poly, center));

            Vector2 axis;

            for (int i = 0; i <= axes.Length; i++)
            {
                axis = i < axes.Length ? axes[i] : circleAxis;

                if (axis != Vector2.Zero)
                {
                    Project(poly, axis, out float minA, out float maxA);
                    Project(circ, axis, out float minB, out float maxB);

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

            var offset = Vector2.Dot(poly.Center - center, normal);
            if (offset < 0) normal = -normal;

            hit.Depth = depth;
            hit.Normal = normal;

            return true;
        }

        public static bool Overlaps(Polygon polyA, Polygon polyB, out Manifold hit)
        {
            hit = default;

            var normal = Vector2.Zero;
            var depth = float.PositiveInfinity;
            var axesA = polyA.GetVertices().Normalize();
            var axesB = polyB.GetVertices().Normalize();

            Vector2 axis;

            for (int i = 0; i < axesA.Length + axesB.Length; i++)
            {
                axis = i < axesA.Length ? axesA[i] : axesB[i - axesA.Length];

                if (axis != Vector2.Zero)
                {
                    Project(polyA, axis, out float minA, out float maxA);
                    Project(polyB, axis, out float minB, out float maxB);

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

            var offset = Vector2.Dot(polyA.Center - polyB.Center, normal);
            if (offset < 0) normal = -normal;

            hit.Depth = depth;
            hit.Normal = normal;

            return true;
        }
        #endregion

        #region Misc.
        static void Project(Box box, Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            var vertices = box.GetVertices();
            float dotProd;

            for (int i = 0; i < vertices.Length; i++)
            {
                dotProd = Vector2.Dot(vertices[i], axis);

                if (min > dotProd) min = dotProd;
                if (max < dotProd) max = dotProd;
            }
        }

        static void Project(Circle circ, Vector2 axis, out float min, out float max)
        {
            var dotProd = Vector2.Dot(circ.Center, axis);

            min = dotProd - circ.Radius;
            max = dotProd + circ.Radius;
        }

        static void Project(Polygon poly, Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            var vertices = poly.GetVertices();
            float dotProd;

            for (int i = 0; i < vertices.Length; i++)
            {
                dotProd = Vector2.Dot(vertices[i], axis);

                if (min > dotProd) min = dotProd;
                if (max < dotProd) max = dotProd;
            }
        }

        static Vector2 GetClosestPointOnPolygon(Polygon poly, Vector2 point)
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
