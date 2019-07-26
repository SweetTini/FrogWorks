using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public static class Collision
    {
        #region Raycasting
        public static bool Cast(this Ray ray, Shape other, out Raycast hit)
        {
            hit = new Raycast(ray);

            if (other is Circle)
                return ray.Cast(other as Circle, out hit);
            else if (other is RectangleF)
                return ray.Cast(other as RectangleF, out hit);
            else if (other is Polygon)
                return ray.Cast(other as Polygon, out hit);

            return false;
        }

        public static bool Cast(this Ray ray, Circle other, out Raycast hit)
        {
            hit = new Raycast(ray);

            var m = other.Center - ray.Position;
            var c = Vector2.Dot(m, m) - other.Radius * other.Radius;
            var b = Vector2.Dot(m, ray.Normal);
            var distance = b * b - c;
            if (distance < 0f) return false;

            var t = -b - (float)Math.Sqrt(distance);

            if (t.Between(0f, ray.Distance))
            {
                var impact = ray.ApplyImpact(t);
                hit.Impact = t;
                hit.Normal = Vector2.Normalize(impact - other.Center);
                return true;
            }

            return false;
        }

        public static bool Cast(this Ray ray, RectangleF other, out Raycast hit)
        {
            hit = new Raycast(ray);

            var inverse = ray.Normal.Inverse();
            var dA = (other.Position - ray.Position) * inverse;
            var dB = (other.Position + other.Size - ray.Position) * inverse;
            var vA = Vector2.Min(dA, dB);
            var vB = Vector2.Max(dA, dB);
            var lowest = Math.Max(vA.X, vA.Y);
            var highest = Math.Min(vB.X, vB.Y);

            if (highest >= 0f && highest >= lowest && lowest <= ray.Distance)
            {
                var center = ray.ApplyImpact(lowest) - (other.Position + other.Size * .5f);
                var centerAbs = center.Abs();
                hit.Normal = center.Sign() * (centerAbs.X > centerAbs.Y ? Vector2.UnitX : Vector2.UnitY);
                hit.Impact = lowest;
                return true;
            }

            return false;
        }

        public static bool Cast(this Ray ray, Polygon other, out Raycast hit)
        {
            hit = new Raycast(ray);

            var lowest = 0f;
            var highest = ray.Distance;
            var index = -1;

            for (int i = 0; i < other.Count; i++)
            {
                var numerator = Vector2.Dot(other.Normals[i], other.Vertices[i] - ray.Position);
                var denominator = Vector2.Dot(other.Normals[i], ray.Normal);
                if (denominator == 0f && numerator < 0f) return false;

                if (denominator < 0f && numerator < lowest * denominator)
                {
                    lowest = numerator / denominator;
                    index = i;
                }
                else if (denominator > 0f && numerator < highest * denominator)
                {
                    highest = numerator / denominator;
                }

                if (highest < lowest) return false;
            }

            if (index > -1)
            {
                hit.Normal = other.Normals[index];
                hit.Impact = lowest;
                return true;
            }

            return false;
        }
        #endregion

        #region Shape
        public static bool Collide(this Shape shape, Shape other)
        {
            if (shape is Circle)
                return (shape as Circle).Collide(other);
            else if (shape is RectangleF)
                return (shape as RectangleF).Collide(other);
            else if (other is Polygon)
                return (shape as Polygon).Collide(other);

            return false;
        }

        public static bool Collide(this Shape shape, Shape other, out Manifold hit)
        {
            hit = new Manifold();

            if (shape is Circle)
                return (shape as Circle).Collide(other, out hit);
            else if (shape is RectangleF)
                return (shape as RectangleF).Collide(other, out hit);
            else if (other is Polygon)
                return (shape as Polygon).Collide(other, out hit);

            return false;
        }
        #endregion

        #region Circle
        public static bool Collide(this Circle circle, Shape other)
        {
            if (other is Circle)
                return circle.Collide(other as Circle);
            else if (other is RectangleF)
                return circle.Collide(other as RectangleF);
            else if (other is Polygon)
                return circle.Collide(other as Polygon);

            return false;
        }

        public static bool Collide(this Circle circle, Shape other, out Manifold hit)
        {
            hit = new Manifold();

            if (other is Circle)
                return circle.Collide(other as Circle, out hit);
            else if (other is RectangleF)
                return circle.Collide(other as RectangleF, out hit);
            else if (other is Polygon)
                return circle.Collide(other as Polygon, out hit);

            return false;
        }

        public static bool Collide(this Circle circle, Circle other)
        {
            var radius = circle.Radius + other.Radius;
            return (other.Center - circle.Center).LengthSquared() < radius * radius;
        }

        public static bool Collide(this Circle circle, Circle other, out Manifold hit)
        {
            hit = new Manifold();

            var distance = other.Center - circle.Center;
            var distanceSq = Vector2.Dot(distance, distance);
            var radius = circle.Radius + other.Radius;

            if (distanceSq < radius * radius)
            {
                var length = (float)Math.Sqrt(distanceSq);
                var normal = length != 0f ? distance * length.Inverse() : Vector2.UnitY;

                hit.Depth = radius - length;
                hit.Normal = normal;
                hit.ContactPoint = other.Center - normal * other.Radius;
                return true;
            }

            return false;
        }

        public static bool Collide(this Circle circle, RectangleF other)
        {
            var distance = circle.Center - circle.Center.Clamp(other.Position, other.Position + other.Size);
            return distance.LengthSquared() < circle.Radius * circle.Radius;
        }

        public static bool Collide(this Circle circle, RectangleF other, out Manifold hit)
        {
            hit = new Manifold();

            var distance = circle.Center.Clamp(other.Position, other.Position + other.Size) - circle.Center;
            var distanceSq = Vector2.Dot(distance, distance);
            var radiusSq = circle.Radius * circle.Radius;

            if (distanceSq < radiusSq)
            {
                if (distanceSq != 0f)
                {
                    var length = (float)Math.Sqrt(distanceSq);
                    var normal = Vector2.Normalize(distance);

                    hit.Depth = circle.Radius - length;
                    hit.Normal = normal;
                    hit.ContactPoint = circle.Center + normal * length;
                }
                else
                {
                    var center = (other.Position * 2f + other.Size) / 2f;
                    var halfExtent = other.Size / 2f;
                    var length = circle.Center - center;
                    var lengthAbs = length.Abs();
                    var lengthOffset = halfExtent - lengthAbs;

                    var depth = lengthOffset.X < lengthOffset.Y ? lengthOffset.X : lengthOffset.Y;
                    var normal = lengthOffset.X < lengthOffset.Y ? Vector2.UnitX : Vector2.UnitY;
                    var factor = lengthOffset.X < lengthOffset.Y ? length.X : length.Y;
                    normal *= factor < 0 ? 1f : -1f;

                    hit.Depth = circle.Radius + depth;
                    hit.Normal = normal;
                    hit.ContactPoint = circle.Center - normal * depth;
                }

                return true;
            }

            return false;
        }

        public static bool Collide(this Circle circle, Polygon other)
        {
            return GJK.Collide(circle, other);
        }

        public static bool Collide(this Circle circle, Polygon other, out Manifold hit)
        {
            hit = new Manifold();

            Vector2 startpoint, endpoint;
            var distance = GJK.Distance(circle, other, out startpoint, out endpoint);

            if (distance != 0f)
            {
                var normal = endpoint - startpoint;
                var depth = Vector2.Dot(normal, normal);

                if (depth < circle.Radius * circle.Radius)
                {
                    depth = (float)Math.Sqrt(depth);
                    hit.Depth = circle.Radius - depth;
                    hit.Normal = normal * depth.Inverse();
                    hit.ContactPoint = endpoint;
                    return true;
                }
            }
            else
            {
                Plane plane;
                var maxDistance = float.MinValue;
                var index = -1;


                for (int i = 0; i < other.Count; i++)
                {
                    plane = other.GetPlane(i);
                    distance = plane.Distance(circle.Center);
                    if (distance > circle.Radius)
                        return false;

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        index = i;
                    }
                }

                plane = other.GetPlane(index);
                hit.Depth = circle.Radius - maxDistance;
                hit.Normal = -other.Normals[index];
                hit.ContactPoint = plane.Project(circle.Center);
                return true;
            }

            return false;
        }
        #endregion

        #region Rectangle
        public static bool Collide(this RectangleF rect, Shape other)
        {
            if (other is Circle)
                return rect.Collide(other as Circle);
            else if (other is RectangleF)
                return rect.Collide(other as RectangleF);
            else if (other is Polygon)
                return rect.Collide(other as Polygon);

            return false;
        }

        public static bool Collide(this RectangleF rect, Shape other, out Manifold hit)
        {
            hit = new Manifold();

            if (other is Circle)
                return rect.Collide(other as Circle, out hit);
            else if (other is RectangleF)
                return rect.Collide(other as RectangleF, out hit);
            else if (other is Polygon)
                return rect.Collide(other as Polygon, out hit);

            return false;
        }

        public static bool Collide(this RectangleF rect, Circle other)
        {
            return other.Collide(rect);
        }

        public static bool Collide(this RectangleF rect, Circle other, out Manifold hit)
        {
            var collided = other.Collide(rect, out hit);
            hit.Normal *= -1f;
            return collided;
        }

        public static bool Collide(this RectangleF rect, RectangleF other)
        {
            return other.Right > rect.Left && rect.Right > other.Left 
                && other.Bottom > rect.Top && rect.Bottom > other.Top;
        }

        public static bool Collide(this RectangleF rect, RectangleF other, out Manifold hit)
        {
            hit = new Manifold();

            var centerA = (rect.Position * 2f + rect.Size) / 2f;
            var centerB = (other.Position * 2f + other.Size) / 2f;
            var halfExtentA = rect.Size / 2f;
            var halfExtentB = other.Size / 2f;
            var distance = centerB - centerA;
            var lengthOffset = halfExtentA + halfExtentB - distance.Abs();

            if (lengthOffset.X < 0f || lengthOffset.Y < 0f)
                return false;

            var depth = lengthOffset.X < lengthOffset.Y ? lengthOffset.X : lengthOffset.Y;
            var normal = lengthOffset.X < lengthOffset.Y ? Vector2.UnitX : Vector2.UnitY;
            var factor = lengthOffset.X < lengthOffset.Y ? distance.X : distance.Y;
            normal *= factor < 0f ? -1f : 1f;

            hit.Depth = depth;
            hit.Normal = normal;
            hit.ContactPoint = centerA + normal * halfExtentA;
            return true;
        }

        public static bool Collide(this RectangleF rect, Polygon other)
        {
            return GJK.Collide(rect, other);
        }

        public static bool Collide(this RectangleF rect, Polygon other, out Manifold hit)
        {
            var rectPoly = new Polygon(rect.ToVertices());
            return rectPoly.Collide(other, out hit);
        }
        #endregion

        #region Polygon
        public static bool Collide(this Polygon polygon, Shape other)
        {
            if (other is Circle)
                return polygon.Collide(other as Circle);
            else if (other is RectangleF)
                return polygon.Collide(other as RectangleF);
            else if (other is Polygon)
                return polygon.Collide(other as Polygon);

            return false;
        }

        public static bool Collide(this Polygon polygon, Shape other, out Manifold hit)
        {
            hit = new Manifold();

            if (other is Circle)
                return polygon.Collide(other as Circle, out hit);
            else if (other is RectangleF)
                return polygon.Collide(other as RectangleF, out hit);
            else if (other is Polygon)
                return polygon.Collide(other as Polygon, out hit);

            return false;
        }

        public static bool Collide(this Polygon polygon, Circle other)
        {
            return GJK.Collide(polygon, other);
        }

        public static bool Collide(this Polygon polygon, Circle other, out Manifold hit)
        {
            var collided = other.Collide(polygon, out hit);
            hit.Normal *= -1f;
            return collided;
        }

        public static bool Collide(this Polygon polygon, RectangleF other)
        {
            return GJK.Collide(polygon, other);
        }

        public static bool Collide(this Polygon polygon, RectangleF other, out Manifold hit)
        {
            var collided = other.Collide(polygon, out hit);
            hit.Normal *= -1f;
            return collided;
        }

        public static bool Collide(this Polygon polygon, Polygon other)
        {
            return GJK.Collide(polygon, other);
        }

        public static bool Collide(this Polygon polygon, Polygon other, out Manifold hit)
        {
            hit = new Manifold();

            int polyIndex, otherIndex;
            var polyDepth = polygon.GetMinIntersectionDepth(other, out polyIndex);
            var otherDepth = other.GetMinIntersectionDepth(polygon, out otherIndex);

            if (polyDepth >= 0f || otherDepth >= 0f) return false;

            var isPolyDeep = polyDepth * .95f > otherDepth + .01f;
            var refPoly = isPolyDeep ? polygon : other;
            var incPoly = isPolyDeep ? other : polygon;
            var refPolyIndex = isPolyDeep ? polyIndex : otherIndex;
            var flip = !isPolyDeep;

            Plane plane;
            var line = incPoly.GetClosestLine(refPoly, refPolyIndex);

            if (!line.TryClipping(refPoly, refPolyIndex, out plane)) return false;

            hit = line.KeepDeepestIntersection(plane);
            if (flip) hit.Normal *= -1f;

            return true;
        }
        #endregion
    }
}
