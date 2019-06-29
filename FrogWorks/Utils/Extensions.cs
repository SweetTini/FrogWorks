using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class Extensions
    {
        #region Range
        public static bool Between(this int number, int min, int max)
        {
            return min <= number && number <= max;
        }

        public static bool Between(this float number, float min, float max)
        {
            return min <= number && number <= max;
        }

        public static bool WithinRange<T>(this T[] array, int index)
        {
            return 0 <= index && index < array.Length;
        }

        public static bool WithinRange<T>(this IEnumerable<T> list, int index)
        {
            return 0 <= index && index < list.Count();
        }
        #endregion

        #region Rectangle
        public static Rectangle Merge(this Rectangle rect, Rectangle other)
        {
            var x = Math.Min(rect.Left, other.Left);
            var y = Math.Min(rect.Top, other.Top);
            var width = Math.Max(rect.Right, other.Right) - x;
            var height = Math.Max(rect.Bottom, other.Bottom) - y;

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle Intersect(this Rectangle rect, Rectangle other)
        {
            var x = Math.Max(rect.Left, other.Left);
            var y = Math.Max(rect.Top, other.Top);
            var width = Math.Min(rect.Right, other.Right) - x;
            var height = Math.Min(rect.Bottom, other.Bottom) - y;

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle Transform(this Rectangle rect, Vector2? position = null, Vector2? origin = null, Vector2? scale = null, float angle = 0f)
        {
            var result = rect.ToVertices().Transform(position, origin, scale, angle);
            var lowest = result.Min().ToPoint();
            var highest = result.Max().ToPoint();

            return new Rectangle(lowest.X, lowest.Y, highest.X - lowest.X, highest.Y - lowest.Y);
        }

        public static Vector2[] ToVertices(this Rectangle rect)
        {
            return new[]
            {
                new Vector2(rect.Left, rect.Top),
                new Vector2(rect.Right, rect.Top),
                new Vector2(rect.Right, rect.Bottom),
                new Vector2(rect.Left, rect.Bottom)
            };
        }
        #endregion

        #region Vertices
        public static Vector2 Min(this Vector2[] vertices)
        {
            Vector2 lowest = vertices[0];

            foreach (var point in vertices)
                lowest = Vector2.Min(lowest, point);

            return lowest;
        }

        public static Vector2 Max(this Vector2[] vertices)
        {
            Vector2 highest = vertices[0];

            foreach (var point in vertices)
                highest = Vector2.Max(highest, point);

            return highest;
        }

        public static Vector2[] Transform(this Vector2[] vertices, Vector2? position = null, Vector2? origin = null, Vector2? scale = null, float angle = 0f)
        {
            var result = new Vector2[vertices.Length];
            var original = vertices.Min();
            var absolute = original + (origin ?? Vector2.Zero);

            var transformMatrix = Matrix.CreateTranslation(new Vector3(-absolute, 0f))
                * Matrix.CreateRotationZ(angle)
                * Matrix.CreateScale(new Vector3(scale ?? Vector2.One, 1f))
                * Matrix.CreateTranslation(new Vector3(position ?? original, 0f));

            Vector2.Transform(vertices, ref transformMatrix, result);

            return result;
        }
        #endregion
    }
}
