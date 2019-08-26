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
        public static byte HexToByte(char hexValue)
        {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(hexValue));
        }

        public static int Mod(this int number, int divisor)
        {
            return (number % divisor + divisor) % divisor;
        }

        public static float Mod(this float number, float divisor)
        {
            return (number % divisor + divisor) % divisor;
        }

        public static int Clamp(this int number, int lowest, int highest)
        {
            return MathHelper.Clamp(number, lowest, highest);
        }

        public static float Clamp(this float number, float lowest, float highest)
        {
            return MathHelper.Clamp(number, lowest, highest);
        }

        public static float Inverse(this float number)
        {
            return number != 0f ? 1f / number : number;
        }
        #endregion

        #region String
        public static string PadWithZeros(this int number, int digits)
        {
            var result = number.ToString();
            while (result.Length < digits)
                result = "0" + result;
            return result;
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

        public static Rectangle SnapToGrid(this Rectangle rect, Vector2 gridSize, Vector2 offset = default(Vector2))
        {
            var upper = rect.Location.ToVector2().SnapToGrid(gridSize, offset);
            var lower = (rect.Location + rect.Size).ToVector2().SnapToGrid(gridSize, offset);
            return new Rectangle(upper.ToPoint(), (lower - upper).ToPoint());
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

        public static Point Sign(this Point point)
        {
            return new Point(Math.Sign(point.X), Math.Sign(point.Y));
        }
        #endregion

        #region Vector
        public static Vector2 AngleToVector(this float angle, float length)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * length;
        }

        public static float VectorToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
        }

        public static Vector2 Clamp(this Vector2 vector, Vector2 lowest, Vector2 highest)
        {
            return new Vector2(
                MathHelper.Clamp(vector.X, lowest.X, highest.X),
                MathHelper.Clamp(vector.Y, lowest.Y, highest.Y));
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
                divider.X != 0f ? vector.X / divider.X : 0f,
                divider.Y != 0f ? vector.Y / divider.Y : 0f);
        }

        public static Vector2 Divide(this Vector2 vector, float divider)
        {
            return divider != 0f ? vector / divider : Vector2.Zero;
        }

        public static Vector2 Inverse(this Vector2 vector)
        {
            return new Vector2(
                vector.X != 0f ? 1f / vector.X : 0f, 
                vector.Y != 0f ? 1f / vector.Y : 0f);
        }

        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2((float)Math.Round(vector.X), (float)Math.Round(vector.Y));
        }

        public static Vector2 Round(this Vector2 vector, int digits)
        {
            return new Vector2((float)Math.Round(vector.X, digits), (float)Math.Round(vector.Y, digits));
        }

        public static Vector2 SnapToGrid(this Vector2 vector, Vector2 gridSize, Vector2 offset = default(Vector2))
        {
            return (vector - offset).Divide(gridSize).Round();
        }

        public static Vector2 SafeNormalize(this Vector2 vector)
        {
            return vector != Vector2.Zero ? Vector2.Normalize(vector) : vector;
        }

        public static Vector2 Sign(this Vector2 vector)
        {
            return new Vector2(Math.Sign(vector.X), Math.Sign(vector.Y));
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

        public static Vector2[] Normalize(this Vector2[] vertices)
        {
            var modified = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                var j = (i + 1) % vertices.Length;
                modified[i] = Vector2.Normalize((vertices[j] - vertices[i]).Perpendicular());
            }

            return modified;
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

        public static Rectangle ToRectangle(this Vector2[] vertices)
        {
            var location = vertices.Min().Round();
            var size = vertices.Max().Round() - location;
            return new Rectangle(location.ToPoint(), size.ToPoint());
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
        public static Delegate GetMethod<T>(object obj, string name)
            where T : class
        {
            var flags =  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var method = obj.GetType().GetMethod(name, flags);
            return method != null ? Delegate.CreateDelegate(typeof(T), obj, name) : null;
        }
        #endregion

        #region XML: Attributes
        public static bool HasAttribute(this XmlElement element, string name)
        {
            return element.Attributes[name] != null;
        }

        public static string Attribute(this XmlElement element, string name, string defaultValue = "")
        {
            return !string.IsNullOrEmpty(element.Attributes[name]?.InnerText)
                ? element.Attributes[name].InnerText 
                : defaultValue;
        }

        public static bool AttributeToBoolean(this XmlElement element, string name, bool defaultValue = false)
        {
            return element.Attributes[name] != null 
                ? Convert.ToBoolean(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static int AttributeToInteger(this XmlElement element, string name, int defaultValue = 0)
        {
            return element.Attributes[name] != null
                ? Convert.ToInt32(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static float AttributeToFloat(this XmlElement element, string name, float defaultValue = 0f)
        {
            return element.Attributes[name] != null
                ? Convert.ToSingle(element.Attributes[name].InnerText)
                : defaultValue;
        }

        public static Point AttributeToPoint(this XmlElement element, string nameForX, string nameForY, Point defaultValue = default(Point))
        {
            return new Point(
                element.AttributeToInteger(nameForX, defaultValue.X),
                element.AttributeToInteger(nameForY, defaultValue.Y));
        }

        public static Vector2 AttributeToVector2(this XmlElement element, string nameForX, string nameForY, Vector2 defaultValue = default(Vector2))
        {
            return new Vector2(
                element.AttributeToFloat(nameForX, defaultValue.X),
                element.AttributeToFloat(nameForY, defaultValue.Y));
        }

        public static Rectangle AttributeToRectangle(this XmlElement element, Rectangle defaultValue = default(Rectangle))
        {
            return new Rectangle(
                element.AttributeToInteger("x", defaultValue.X),
                element.AttributeToInteger("y", defaultValue.Y),
                element.AttributeToInteger("width", defaultValue.Width),
                element.AttributeToInteger("height", defaultValue.Height));
        }
        #endregion
    }
}
