using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace FrogWorks
{
    public static class Extensions
    {
        #region Numeric
        public static int Abs(this int number)
        {
            return Math.Abs(number);
        }

        public static float Abs(this float number)
        {
            return Math.Abs(number);
        }

        public static float Ceiling(this float number)
        {
            return (float)Math.Ceiling(number);
        }

        public static int Clamp(this int number, int lowest, int highest)
        {
            return MathHelper.Clamp(number, lowest, highest);
        }

        public static float Clamp(this float number, float lowest, float highest)
        {
            return MathHelper.Clamp(number, lowest, highest);
        }

        public static int Divide(this int number, int divider)
        {
            return divider == 0 ? divider : number / divider;
        }

        public static float Divide(this float number, float divider)
        {
            return divider.Abs() < float.Epsilon ? 0f : number / divider;
        }

        public static float Floor(this float number)
        {
            return (float)Math.Floor(number);
        }

        public static byte HexToByte(this char hex)
        {
            return (byte)Math.Max("0123456789ABCDEF".IndexOf(char.ToUpper(hex)), 0);
        }

        public static float Inverse(this float number)
        {
            return 1f / number;
        }

        public static int Lerp(this int number, int target, float amount)
        {
            return (int)Math.Round(number + (target - number) * amount);
        }

        public static float Lerp(this float number, float target, float amount)
        {
            return number + (target - number) * amount;
        }

        public static float Lerp(
            this int number,
            int target,
            float amount,
            float maxDistance)
        {
            var distance = (target - number) * amount;

            distance = distance > maxDistance
                ? maxDistance
                : distance;

            return (int)Math.Round(number + distance);
        }

        public static float Lerp(
            this float number, 
            float target, 
            float amount, 
            float maxDistance)
        {
            var distance = (target - number) * amount;

            distance = distance > maxDistance
                ? maxDistance
                : distance;

            return number + distance;
        }

        public static int Max(this int number, int other)
        {
            return Math.Max(number, other);
        }

        public static int Max(this int number, params int[] numbers)
        {
            var result = number;
            for (int i = 0; i < numbers.Length; i++)
                result = Math.Max(result, numbers[i]);
            return result;
        }

        public static int Max(this int number, IEnumerable<int> numbers)
        {
            var result = number;
            foreach (var otherNumber in numbers)
                result = Math.Max(result, otherNumber);
            return result;
        }

        public static float Max(this float number, float other)
        {
            return Math.Max(number, other);
        }

        public static float Max(this float number, params float[] numbers)
        {
            var result = number;
            for (int i = 0; i < numbers.Length; i++)
                result = Math.Max(result, numbers[i]);
            return result;
        }

        public static float Max(this float number, IEnumerable<float> numbers)
        {
            var result = number;
            foreach (var otherNumber in numbers)
                result = Math.Max(result, otherNumber);
            return result;
        }

        public static int Min(this int number, int other)
        {
            return Math.Min(number, other);
        }

        public static int Min(this int number, params int[] numbers)
        {
            var result = number;
            for (int i = 0; i < numbers.Length; i++)
                result = Math.Min(result, numbers[i]);
            return result;
        }

        public static int Min(this int number, IEnumerable<int> numbers)
        {
            var result = number;
            foreach (var otherNumber in numbers)
                result = Math.Min(result, otherNumber);
            return result;
        }

        public static float Min(this float number, float other)
        {
            return Math.Min(number, other);
        }

        public static float Min(this float number, params float[] numbers)
        {
            var result = number;
            for (int i = 0; i < numbers.Length; i++)
                result = Math.Min(result, numbers[i]);
            return result;
        }

        public static float Min(this float number, IEnumerable<float> numbers)
        {
            var result = number;
            foreach (var otherNumber in numbers)
                result = Math.Min(result, otherNumber);
            return result;
        }

        public static int Mod(this int number, int divisor)
        {
            return (number % divisor + divisor) % divisor;
        }

        public static float Mod(this float number, float divisor)
        {
            return (number % divisor + divisor) % divisor;
        }

        public static int Pow(this int number, int exponent)
        {
            return (int)Math.Pow(number, exponent);
        }

        public static float Pow(this float number, float exponent)
        {
            return (float)Math.Pow(number, exponent);
        }

        public static float Round(this float number)
        {
            return (float)Math.Round(number, MidpointRounding.AwayFromZero);
        }

        public static float Round(this float number, int decimals)
        {
            return (float)Math.Round(number, decimals, MidpointRounding.AwayFromZero);
        }

        public static int Sign(this int number)
        {
            return Math.Sign(number);
        }

        public static float Sign(this float number)
        {
            return Math.Sign(number);
        }

        public static float Sqrt(this float number)
        {
            return (float)Math.Sqrt(number);
        }
        #endregion

        #region String
        public static string PadWithZeros(this int number, int digits)
        {
            return number.ToString().PadLeft(digits, '0');
        }

        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var result = string.Empty;
            char nextChar;
            while ((nextChar = reader.ReadChar()) != 0)
                result += nextChar;
            return result;
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

        public static bool Between(this Point point, Point min, Point max)
        {
            return point.X.Between(min.X, max.X)
                && point.Y.Between(min.Y, max.Y);
        }

        public static bool Between(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return vector.X.Between(min.X, max.X)
                && vector.Y.Between(min.Y, max.Y);
        }

        public static bool WithinRange<T>(this T[] array, int index)
        {
            return 0 <= index && index < array.Length;
        }

        public static bool WithinRange<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && x < array.GetLength(0)
                && y >= 0 && y < array.GetLength(1);
        }

        public static bool WithinRange<T>(this T[,] array, Point point)
        {
            return point.X >= 0 && point.X < array.GetLength(0)
                && point.Y >= 0 && point.Y < array.GetLength(1);
        }

        public static bool WithinRange<T>(this IEnumerable<T> list, int index)
        {
            return 0 <= index && index < list.Count();
        }

        public static IEnumerable<T> Enumerate<T>(params T[] items)
        {
            return items.AsEnumerable();
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

        public static Rectangle SnapToGrid(this Rectangle rect, Point size, Point offset = default(Point))
        {
            return rect.SnapToGrid(size.ToVector2(), offset.ToVector2());
        }

        public static Rectangle SnapToGrid(this Rectangle rect, Vector2 size, Vector2 offset = default(Vector2))
        {
            var upper = (rect.Location.ToVector2() - offset).Divide(size).Floor().ToPoint();
            var lower = ((rect.Location + rect.Size).ToVector2() - offset).Divide(size).Ceiling().ToPoint();
            return new Rectangle(upper, lower - upper);
        }

        public static Rectangle Transform(
            this Rectangle rect,
            Vector2? position = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f)
        {
            var result = rect.ToVertices().Transform(position, origin, scale, angle);
            var lowest = result.Min().Round().ToPoint();
            var highest = result.Max().Round().ToPoint();

            return new Rectangle(
                lowest.X,
                lowest.Y,
                highest.X - lowest.X,
                highest.Y - lowest.Y);
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

        #region Point
        public static Point Abs(this Point point)
        {
            return new Point(Math.Abs(point.X), Math.Abs(point.Y));
        }

        public static Point Clamp(this Point point, Point lowest, Point highest)
        {
            return new Point(
                MathHelper.Clamp(point.X, lowest.X, highest.X),
                MathHelper.Clamp(point.Y, lowest.Y, highest.Y));
        }

        public static Point Divide(this Point point, Point divider)
        {
            return new Point(
                point.X.Divide(divider.X),
                point.Y.Divide(divider.Y));
        }

        public static Point Max(this Point point, Point other)
        {
            return new Point(
                Math.Max(point.X, other.X),
                Math.Max(point.Y, other.Y));
        }

        public static Point Max(this Point point, params Point[] points)
        {
            var result = point;

            for (int i = 0; i < points.Length; i++)
            {
                result = new Point(
                    Math.Max(result.X, points[i].X),
                    Math.Max(result.Y, points[i].Y));
            }

            return result;
        }

        public static Point Max(this Point point, IEnumerable<Point> points)
        {
            var result = point;

            foreach (var otherPoint in points)
            {
                result = new Point(
                    Math.Max(result.X, otherPoint.X),
                    Math.Max(result.Y, otherPoint.Y));
            }

            return result;
        }

        public static Point Max(this IEnumerable<Point> points)
        {
            var result = new Point(int.MinValue, int.MinValue);

            foreach (var point in points)
            {
                result = new Point(
                    Math.Max(result.X, point.X),
                    Math.Max(result.Y, point.Y));
            }

            return result;
        }

        public static Point Min(this Point point, Point other)
        {
            return new Point(
                Math.Min(point.X, other.X),
                Math.Min(point.Y, other.Y));
        }

        public static Point Min(this Point point, params Point[] points)
        {
            var result = point;

            for (int i = 0; i < points.Length; i++)
            {
                result = new Point(
                    Math.Min(result.X, points[i].X),
                    Math.Min(result.Y, points[i].Y));
            }

            return result;
        }

        public static Point Min(this Point point, IEnumerable<Point> points)
        {
            var result = point;

            foreach (var otherPoint in points)
            {
                result = new Point(
                    Math.Min(result.X, otherPoint.X),
                    Math.Min(result.Y, otherPoint.Y));
            }

            return result;
        }

        public static Point Min(this IEnumerable<Point> points)
        {
            var result = new Point(int.MaxValue, int.MaxValue);

            foreach (var point in points)
            {
                result = new Point(
                    Math.Min(result.X, point.X),
                    Math.Min(result.Y, point.Y));
            }

            return result;
        }

        public static Point Sign(this Point point)
        {
            return new Point(Math.Sign(point.X), Math.Sign(point.Y));
        }
        #endregion

        #region Vector
        public static Vector2 AngleToVector(this float angle, float length)
        {
            return new Vector2(
                (float)Math.Cos(angle), 
                (float)Math.Sin(angle)) * length;
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(
                Math.Abs(vector.X), 
                Math.Abs(vector.Y));
        }

        public static Vector2 Clamp(this Vector2 vector, Vector2 lowest, Vector2 highest)
        {
            return new Vector2(
                MathHelper.Clamp(vector.X, lowest.X, highest.X),
                MathHelper.Clamp(vector.Y, lowest.Y, highest.Y));
        }

        public static Vector2 Ceiling(this Vector2 vector)
        {
            return new Vector2(
                (float)Math.Ceiling(vector.X), 
                (float)Math.Ceiling(vector.Y));
        }

        public static float Cross(this Vector2 vector, Vector2 other)
        {
            return vector.X * other.Y - vector.Y * other.X;
        }

        public static Vector2 Cross(this Vector2 vector, float scale)
        {
            return new Vector2(scale * vector.Y, -scale * vector.X);
        }

        public static Vector2 Divide(this Vector2 vector, Vector2 divider)
        {
            return new Vector2(
                vector.X.Divide(divider.X),
                vector.Y.Divide(divider.Y));
        }

        public static Vector2 Divide(this Vector2 vector, float divider)
        {
            return new Vector2(
                vector.X.Divide(divider),
                vector.Y.Divide(divider));
        }

        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2(
                (float)Math.Floor(vector.X), 
                (float)Math.Floor(vector.Y));
        }

        public static Vector2 Inverse(this Vector2 vector)
        {
            return Vector2.One / vector;
        }

        public static Vector2 Lerp(this Vector2 vector, Vector2 target, float amount)
        {
            return vector + (target - vector) * amount;
        }

        public static Vector2 Lerp(
            this Vector2 vector,
            Vector2 target, 
            float amount, 
            float maxDistance)
        {
            var distance = (target - vector) * amount;

            distance = distance.Length() > maxDistance
                ? Vector2.Normalize(distance) * maxDistance
                : distance;

            return vector + distance;
        }

        public static Vector2 Max(this Vector2 vector, Vector2 other)
        {
            return Vector2.Max(vector, other);
        }

        public static Vector2 Max(this Vector2 vector, params Vector2[] vectors)
        {
            var result = vector;
            for (int i = 0; i < vectors.Length; i++)
                result = Vector2.Max(result, vectors[i]);
            return result;
        }

        public static Vector2 Max(this Vector2 vector, IEnumerable<Vector2> vectors)
        {
            var result = vector;
            foreach (var otherVector in vectors)
                result = Vector2.Max(result, otherVector);
            return result;
        }

        public static Vector2 Max(this IEnumerable<Vector2> vectors)
        {
            var result = Vector2.One * float.MinValue;
            foreach (var vector in vectors)
                result = Vector2.Max(result, vector);
            return result;
        }

        public static Vector2 Min(this Vector2 vector, Vector2 other)
        {
            return Vector2.Min(vector, other);
        }

        public static Vector2 Min(this Vector2 vector, params Vector2[] vectors)
        {
            var result = vector;
            for (int i = 0; i < vectors.Length; i++)
                result = Vector2.Min(result, vectors[i]);
            return result;
        }

        public static Vector2 Min(this Vector2 vector, IEnumerable<Vector2> vectors)
        {
            var result = vector;
            foreach (var otherVector in vectors)
                result = Vector2.Min(result, otherVector);
            return result;
        }

        public static Vector2 Min(this IEnumerable<Vector2> vectors)
        {
            var result = Vector2.One * float.MaxValue;
            foreach (var vector in vectors)
                result = Vector2.Min(result, vector);
            return result;
        }

        public static Vector2 Perpendicular(this Vector2 vector, bool counterClockwise = true)
        {
            return counterClockwise
                ? new Vector2(-vector.Y, vector.X)
                : new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2(
                (float)Math.Round(vector.X, MidpointRounding.AwayFromZero),
                (float)Math.Round(vector.Y, MidpointRounding.AwayFromZero));
        }

        public static Vector2 Round(this Vector2 vector, int digits)
        {
            return new Vector2(
                (float)Math.Round(vector.X, digits, MidpointRounding.AwayFromZero),
                (float)Math.Round(vector.Y, digits, MidpointRounding.AwayFromZero));
        }

        public static Vector2 SnapToGrid(
            this Vector2 vector, 
            Vector2 size, 
            Vector2 offset = default)
        {
            return (vector - offset).Divide(size).Floor();
        }

        public static Vector2 SafeNormalize(this Vector2 vector)
        {
            return vector != Vector2.Zero ? Vector2.Normalize(vector) : vector;
        }

        public static Vector2 Sign(this Vector2 vector)
        {
            return new Vector2(Math.Sign(vector.X), Math.Sign(vector.Y));
        }

        public static float VectorToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
        #endregion

        #region Vertices
        public static Vector2 Center(this Vector2[] vertices)
        {
            var sum = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var j = (i + 1).Mod(vertices.Length);
                var center = (vertices[i] + vertices[j]) * .5f;
                sum += center;
            }

            return sum.Divide(vertices.Length);
        }

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

        public static Vector2[] Normalize(this Vector2[] vertices)
        {
            var modified = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                var j = (i + 1).Mod(vertices.Length);
                var edge = Vector2.Normalize(vertices[i] - vertices[j]);
                modified[i] = edge.Perpendicular();
            }

            return modified;
        }

        public static Vector2[] Transform(
            this Vector2[] vertices,
            Vector2? position = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f,
            bool offsetToOrigin = true)
        {
            var result = new Vector2[vertices.Length];
            var location = vertices.Min();
            var offset = location + origin.GetValueOrDefault();
            var absolute = offsetToOrigin
                ? position ?? location
                : position.GetValueOrDefault() + offset;

            var transformMatrix = Matrix.CreateTranslation(new Vector3(-offset, 0f))
                * Matrix.CreateRotationZ(angle)
                * Matrix.CreateScale(new Vector3(scale ?? Vector2.One, 1f))
                * Matrix.CreateTranslation(new Vector3(absolute, 0f));

            Vector2.Transform(vertices, ref transformMatrix, result);

            return result;
        }

        public static Rectangle ToRectangle(this Vector2[] vertices)
        {
            var location = vertices.Min().Round();
            var size = vertices.Max().Round() - location;
            return new Rectangle(location.ToPoint(), size.ToPoint());
        }

        public static Vector2[] ToOrigin(this Vector2[] vertices)
        {
            var origin = -vertices.Min();
            return vertices.Transform(origin);
        }

        public static Vector2[] ToConvexHull(this Vector2[] vertices, int segments = 8)
        {
            if (vertices.Length < 3) return vertices;

            segments = Math.Min(vertices.Length, segments);
            var farthestIndex = 0;
            var farthest = vertices[0].X;

            for (int i = 1; i < segments; i++)
            {
                if (farthest < vertices[i].X)
                {
                    farthestIndex = i;
                    farthest = vertices[i].X;
                }
                else if (farthest == vertices[i].X && vertices[farthestIndex].Y > vertices[i].Y)
                {
                    farthestIndex = i;
                }
            }

            var hullIndices = new int[segments];
            var index = farthestIndex;
            var outCount = 0;

            while (true)
            {
                hullIndices[outCount] = index;
                var nextIndex = 0;

                for (int i = 1; i < segments; i++)
                {
                    if (nextIndex == index)
                    {
                        nextIndex = i;
                        continue;
                    }

                    var hullIndex = hullIndices[outCount];
                    var edgeA = vertices[nextIndex] - vertices[hullIndex];
                    var edgeB = vertices[i] - vertices[hullIndex];
                    var determinant = edgeA.Cross(edgeB);

                    if (determinant < 0 || determinant == 0 && Vector2.Dot(edgeB, edgeB) > Vector2.Dot(edgeA, edgeA))
                        nextIndex = i;
                }

                outCount++;
                index = nextIndex;
                if (nextIndex == farthestIndex) break;
            }

            var hullVertices = new Vector2[segments];

            for (int i = 0; i < outCount; i++)
            {
                var hullIndex = hullIndices[i];
                hullVertices[i] = vertices[hullIndex];
            }

            return hullVertices;
        }

        public static Vector2[] Truncate(this Vector2[] vertices)
        {
            var location = vertices.Min();
            var size = vertices.Max() - location;

            return vertices.Truncate(location, size);
        }

        public static Vector2[] Truncate(this Vector2[] vertices, Vector2 size)
        {
            return vertices.Truncate(Vector2.Zero, size);
        }

        public static Vector2[] Truncate(
            this Vector2[] vertices,
            Vector2 location,
            Vector2 size)
        {
            size = size.Abs();

            var results = new List<Vector2>();
            Vector2 vertex;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertex = vertices[i];

                if (vertex == vertex.Clamp(location, location + size))
                    results.Add((vertex - location).Divide(size));
            }

            return results.ToArray();
        }
        #endregion

        #region Inputs
        public static int SignThreshold(this float number, float threshold)
        {
            return Math.Abs(number) >= threshold ? Math.Sign(number) : 0;
        }

        public static Vector2 SnapAngle(this Vector2 vector, float segments)
        {
            segments = Math.Abs(segments);

            var divider = MathHelper.Pi / (segments > 0f ? segments : 1f);
            var angle = (float)Math.Floor((vector.VectorToAngle() + divider * .5f) / divider) * divider;

            return angle.AngleToVector(vector.Length());
        }

        public static Vector2 SnapAndNormalizeAngle(this Vector2 vector, float segments)
        {
            segments = Math.Abs(segments);

            var divider = MathHelper.Pi / (segments > 0f ? segments : 1f);
            var angle = (float)Math.Floor((vector.VectorToAngle() + divider * .5f) / divider) * divider;

            return angle.AngleToVector(1f);
        }
        #endregion

        #region Reflection
        public static Delegate GetMethod<T>(object instance, string name)
            where T : class
        {
            var flags = BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic;

            var method = instance.GetType().GetMethod(name, flags);

            return method != null
                ? Delegate.CreateDelegate(
                    typeof(T), instance, method, false)
                : null;
        }
        #endregion

        #region Misc.
        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
        }
        #endregion

        #region XML: Attributes
        public static bool HasAttr(this XmlElement element, string name)
        {
            return element.Attributes[name] != null;
        }

        public static string AttrToString(
            this XmlElement element,
            string name,
            string defaultValue = "")
        {
            return !string.IsNullOrEmpty(element.Attributes[name]?.InnerText)
                ? element.Attributes[name].InnerText
                : defaultValue;
        }

        public static bool AttrToBoolean(
            this XmlElement element,
            string name,
            bool defaultValue = false)
        {
            return element.Attributes[name] != null
                ? Convert.ToBoolean(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static int AttrToInt32(
            this XmlElement element,
            string name,
            int defaultValue = 0)
        {
            return element.Attributes[name] != null
                ? Convert.ToInt32(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static float AttrToSingle(
            this XmlElement element,
            string name,
            float defaultValue = 0f)
        {
            return element.Attributes[name] != null
                ? Convert.ToSingle(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static T AttrToEnum<T>(
            this XmlElement element,
            string name,
            T defaultValue = default(T))
            where T : struct
        {
            if (element.HasAttribute(name))
            {
                T enumValue;
                if (Enum.TryParse(element.Attributes[name].InnerText, true, out enumValue))
                    return enumValue;
            }

            return defaultValue;
        }

        public static Color AttrToColor(
            this XmlElement element,
            string name,
            Color defaultValue = default(Color))
        {
            return element.Attributes[name] != null
                ? ColorEX.FromHex(element.Attributes[name].InnerText.Replace("#", ""))
                : defaultValue;
        }

        public static Color AttrToColor(
            this XmlElement element,
            string nameR,
            string nameG,
            string nameB,
            Color defaultValue = default(Color))
        {
            return new Color(
                element.AttrToInt32(nameR, defaultValue.R),
                element.AttrToInt32(nameG, defaultValue.G),
                element.AttrToInt32(nameB, defaultValue.B));
        }

        public static Color AttrToColor(
            this XmlElement element,
            string nameR,
            string nameG,
            string nameB,
            string nameA,
            Color defaultValue = default(Color))
        {
            return new Color(
                element.AttrToInt32(nameR, defaultValue.R),
                element.AttrToInt32(nameG, defaultValue.G),
                element.AttrToInt32(nameB, defaultValue.B),
                element.AttrToInt32(nameA, defaultValue.A));
        }

        public static Point AttrToPoint(
            this XmlElement element,
            string nameX,
            string nameY,
            Point defaultValue = default(Point))
        {
            return new Point(
                element.AttrToInt32(nameX, defaultValue.X),
                element.AttrToInt32(nameY, defaultValue.Y));
        }

        public static Point AttrToPoint(
            this XmlElement element,
            Point defaultValue = default(Point))
        {
            return element.AttrToPoint("x", "y", defaultValue);
        }

        public static Vector2 AttrToVector2(
            this XmlElement element,
            string nameX,
            string nameY,
            Vector2 defaultValue = default(Vector2))
        {
            return new Vector2(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y));
        }

        public static Vector2 AttrToVector2(
            this XmlElement element,
            Vector2 defaultValue = default(Vector2))
        {
            return element.AttrToVector2("x", "y", defaultValue);
        }

        public static Vector3 AttrToVector3(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameZ,
            Vector3 defaultValue = default(Vector3))
        {
            return new Vector3(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y),
                element.AttrToSingle(nameZ, defaultValue.Z));
        }

        public static Vector3 AttrToVector3(
            this XmlElement element,
            Vector3 defaultValue = default(Vector3))
        {
            return element.AttrToVector3("x", "y", "z", defaultValue);
        }

        public static Vector4 AttrToVector4(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameZ,
            string nameW,
            Vector4 defaultValue = default(Vector4))
        {
            return new Vector4(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y),
                element.AttrToSingle(nameZ, defaultValue.Z),
                element.AttrToSingle(nameW, defaultValue.W));
        }

        public static Vector4 AttrToVector4(
            this XmlElement element,
            Vector4 defaultValue = default(Vector4))
        {
            return element.AttrToVector4("x", "y", "z", "w", defaultValue);
        }

        public static Rectangle AttrToRectangle(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameW,
            string nameH,
            Rectangle defaultValue = default(Rectangle))
        {
            return new Rectangle(
                element.AttrToPoint(nameX, nameY, defaultValue.Location),
                element.AttrToPoint(nameW, nameH, defaultValue.Size));
        }

        public static Rectangle AttrToRectangle(
            this XmlElement element,
            Rectangle defaultValue = default(Rectangle))
        {
            return element.AttrToRectangle("x", "y", "width", "height", defaultValue);
        }
        #endregion
    }
}
