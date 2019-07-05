using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class Extensions
    {
        #region Numerics
        public static int Mod(this int number, int divisor)
        {
            return (number % divisor + divisor) % divisor;
        }

        public static float Mod(this float number, float divisor)
        {
            return (number % divisor + divisor) % divisor;
        }
        #endregion

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

        public static bool WithinRange<T>(this T[,] array, int columnIndex, int rowIndex)
        {
            return columnIndex >= 0 && columnIndex < array.GetLength(0)
                && rowIndex >= 0 && rowIndex < array.GetLength(1);
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
            var lowest = result.Min().Round().ToPoint();
            var highest = result.Max().Round().ToPoint();

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

        #region Vectors
        public static Vector2 Clamp(this Vector2 vector, Vector2 lowest, Vector2 highest)
        {
            return new Vector2(
                MathHelper.Clamp(vector.X, lowest.X, highest.X),
                MathHelper.Clamp(vector.Y, lowest.Y, highest.Y));
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2((float)Math.Round(vector.X), (float)Math.Round(vector.Y));
        }

        public static Vector2 Round(this Vector2 vector, int digits)
        {
            return new Vector2((float)Math.Round(vector.X, digits), (float)Math.Round(vector.Y, digits));
        }
        #endregion

        #region Vertices
        public static Vector2 Min(this Vector2[] vertices)
        {
            Vector2 lowest = vertices[0];

            for (int i = 1; i < vertices.Length; i++)
                lowest = Vector2.Min(lowest, vertices[i]);

            return lowest;
        }

        public static Vector2 Max(this Vector2[] vertices)
        {
            Vector2 highest = vertices[0];

            for (int i = 1; i < vertices.Length; i++)
                highest = Vector2.Max(highest, vertices[i]);

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
