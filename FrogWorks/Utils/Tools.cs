using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace FrogWorks
{
    public static class Tools
    {
        #region Arrays & Lists
        public static bool Between<T>(this int index, T[] array)
        {
            return 0 <= index && index < array.Length;
        }

        public static bool Between<T>(this int index, T[,] array)
        {
            var area = array.GetLength(0) * array.GetLength(1);

            return 0 <= index && index < area;
        }

        public static bool Between<T>(this Point point, T[,] array)
        {
            return 0 <= point.X && point.X < array.GetLength(0)
                && 0 <= point.Y && point.Y < array.GetLength(1);
        }

        public static bool Between<T>(this int index, IEnumerable<T> list)
        {
            return 0 <= index && index < list.Count();
        }

        public static IEnumerable<T> Itemize<T>(params T[] items)
        {
            return items.AsEnumerable();
        }

        public static bool WithinRange<T>(this T[] array, int index)
        {
            return 0 <= index && index < array.Length;
        }

        public static bool WithinRange<T>(this T[,] array, int x, int y)
        {
            return 0 <= x && x < array.GetLength(0)
                && 0 <= y && y < array.GetLength(1);
        }

        public static bool WithinRange<T>(this T[,] array, Point point)
        {
            return 0 <= point.X && point.X < array.GetLength(0)
                && 0 <= point.Y && point.Y < array.GetLength(1);
        }

        public static bool WithinRange<T>(this IEnumerable<T> list, int index)
        {
            return 0 <= index && index < list.Count();
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

        #region Object Swap
        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
        }
        #endregion
    }

    public static class MathEX
    {
        #region Constants
        public const float E = MathHelper.E;
        public const float Log10E = MathHelper.Log10E;
        public const float Log2E = MathHelper.Log2E;
        public const float Pi = MathHelper.Pi;
        public const float PiOver2 = MathHelper.PiOver2;
        public const float PiOver4 = MathHelper.PiOver4;
        public const float TwoPi = MathHelper.TwoPi;
        public const float DegToRad = MathHelper.Pi / 180f;
        public const float RadToDeg = 180f / MathHelper.Pi;
        const string Hex = "0123456789ABCDEF";
        #endregion

        #region Hexadecimals
        public static byte HexToByte(this char hex)
        {
            return (byte)Math.Max(Hex.IndexOf(char.ToUpper(hex)), 0);
        }
        #endregion

        #region Integers
        public static int Abs(this int num)
        {
            return Math.Abs(num);
        }

        public static int Approach(this int num, int target, int distance)
        {
            return num > target
                ? Math.Max(num - distance, target)
                : Math.Min(num + distance, target);
        }

        public static bool Between(this int num, int min, int max)
        {
            var lowest = Math.Min(min, max);
            var highest = Math.Max(min, max);

            return lowest <= num && num <= highest;
        }

        public static int Clamp(this int num, int min, int max)
        {
            var lowest = Math.Min(min, max);
            var highest = Math.Max(min, max);

            return MathHelper.Clamp(num, lowest, highest);
        }

        public static int CountDigits(this int num)
        {
            var digits = 1;
            var target = 10;

            while (Math.Abs(num) >= target)
            {
                digits++;
                target *= 10;
            }

            return digits;
        }

        public static int Divide(this int num, int divider)
        {
            return divider != 0
                ? num / divider
                : divider;
        }

        public static int Lerp(this int num, int target, float amount)
        {
            return (int)Math.Round(num + (target - num) * MathHelper.Clamp(amount, 0f, 1f));
        }

        public static int Lerp(this int num, int target, float amount, int distance)
        {
            var expected = (target - num) * MathHelper.Clamp(amount, 0f, 1f);

            if (Math.Abs(expected) > distance)
                expected = distance * Math.Sign(expected);

            return (int)Math.Round(num + expected);
        }

        public static int Max(this int num, int other)
        {
            return Math.Max(num, other);
        }

        public static int Max(this int num, params int[] nums)
        {
            var result = num;

            for (int i = 0; i < nums.Length; i++)
                result = Math.Max(result, nums[i]);

            return result;
        }

        public static int Max(this int num, IEnumerable<int> nums)
        {
            var result = num;

            foreach (var next in nums)
                result = Math.Max(result, next);

            return result;
        }

        public static int Max(params int[] nums)
        {
            var result = nums[0];

            for (int i = 1; i < nums.Length; i++)
                result = Math.Max(result, nums[i]);

            return result;
        }

        public static int Max(IEnumerable<int> nums)
        {
            var result = int.MinValue;

            foreach (var next in nums)
                result = Math.Max(result, next);

            return result;
        }

        public static int Min(this int num, int other)
        {
            return Math.Min(num, other);
        }

        public static int Min(this int num, params int[] nums)
        {
            var result = num;

            for (int i = 0; i < nums.Length; i++)
                result = Math.Min(result, nums[i]);

            return result;
        }

        public static int Min(this int num, IEnumerable<int> nums)
        {
            var result = num;

            foreach (var next in nums)
                result = Math.Min(result, next);

            return result;
        }

        public static int Min(params int[] nums)
        {
            var result = nums[0];

            for (int i = 1; i < nums.Length; i++)
                result = Math.Min(result, nums[i]);

            return result;
        }

        public static int Min(IEnumerable<int> nums)
        {
            var result = int.MaxValue;

            foreach (var next in nums)
                result = Math.Min(result, next);

            return result;
        }

        public static int Mod(this int num, int divisor)
        {
            return (num % divisor + divisor) % divisor;
        }

        public static int Pow(this int num, int exponent)
        {
            return (int)Math.Pow(num, exponent);
        }

        public static int Sign(this int num)
        {
            return Math.Sign(num);
        }

        public static int Sqrt(this int num)
        {
            return (int)Math.Round(Math.Sqrt(num));
        }
        #endregion

        #region Singles
        public static float Abs(this float num)
        {
            return Math.Abs(num);
        }

        public static float AbsAngleDiff(float radiansA, float radiansB)
        {
            return Math.Abs(AngleDiff(radiansA, radiansB));
        }

        public static float Acos(this float num)
        {
            return (float)Math.Acos(num);
        }

        public static float AngleApproach(float radians, float target, float distance)
        {
            var diff = AngleDiff(radians, target);

            return Math.Abs(diff) < distance
                ? target : radians + MathHelper.Clamp(diff, -distance, distance);
        }

        public static float AngleDiff(float radiansA, float radiansB)
        {
            float diff = Math.Abs(radiansB - radiansA).Mod(MathHelper.TwoPi);

            return diff > MathHelper.Pi
                ? MathHelper.Pi - diff : diff;
        }

        public static float AngleLerp(float radians, float target, float amount)
        {
            return radians + AngleDiff(radians, target) * MathHelper.Clamp(amount, 0f, 1f);
        }

        public static float AngleLerp(float radians, float target, float amount, float distance)
        {
            var expected = AngleDiff(radians, target) * MathHelper.Clamp(amount, 0f, 1f);

            if (Math.Abs(expected) > distance)
                expected = distance * Math.Sign(expected);

            return radians + expected;
        }

        public static float Approach(this float num, float target, float distance)
        {
            return num > target
                ? Math.Max(num - distance, target)
                : Math.Min(num + distance, target);
        }

        public static float Asin(this float num)
        {
            return (float)Math.Asin(num);
        }

        public static float Atan(this float num)
        {
            return (float)Math.Atan(num);
        }

        public static bool Between(this float num, float min, float max)
        {
            var lowest = Math.Min(min, max);
            var highest = Math.Max(min, max);

            return lowest <= num && num <= highest;
        }

        public static float Ceiling(this float num)
        {
            return (float)Math.Ceiling(num);
        }

        public static float Clamp(this float num, float min, float max)
        {
            var lowest = Math.Min(min, max);
            var highest = Math.Max(min, max);

            return MathHelper.Clamp(num, lowest, highest);
        }

        public static float Cos(this float num)
        {
            return (float)Math.Cos(num);
        }

        public static float Cosh(this float num)
        {
            return (float)Math.Cosh(num);
        }

        public static float Divide(this float num, float divider)
        {
            return divider.Abs() > float.Epsilon
                ? num / divider : 0f;
        }

        public static float Floor(this float num)
        {
            return (float)Math.Floor(num);
        }

        public static float Inverse(this float num)
        {
            return 1f / num;
        }

        public static float Lerp(this float num, float target, float amount)
        {
            return num + (target - num) * MathHelper.Clamp(amount, 0f, 1f);
        }

        public static float Lerp(this float num, float target, float amount, float distance)
        {
            var expected = (target - num) * MathHelper.Clamp(amount, 0f, 1f);

            if (Math.Abs(expected) > distance)
                expected = distance * Math.Sign(expected);

            return num + expected;
        }

        public static float Max(this float num, float other)
        {
            return Math.Max(num, other);
        }

        public static float Max(this float num, params float[] nums)
        {
            var result = num;

            for (int i = 0; i < nums.Length; i++)
                result = Math.Max(result, nums[i]);

            return result;
        }

        public static float Max(this float num, IEnumerable<float> nums)
        {
            var result = num;

            foreach (var next in nums)
                result = Math.Max(result, next);

            return result;
        }

        public static float Max(params float[] nums)
        {
            var result = nums[0];

            for (int i = 1; i < nums.Length; i++)
                result = Math.Max(result, nums[i]);

            return result;
        }

        public static float Max(IEnumerable<float> nums)
        {
            var result = float.MinValue;

            foreach (var next in nums)
                result = Math.Max(result, next);

            return result;
        }

        public static float Min(this float num, float other)
        {
            return Math.Min(num, other);
        }

        public static float Min(this float num, params float[] nums)
        {
            var result = num;

            for (int i = 0; i < nums.Length; i++)
                result = Math.Min(result, nums[i]);

            return result;
        }

        public static float Min(this float num, IEnumerable<float> nums)
        {
            var result = num;

            foreach (var next in nums)
                result = Math.Min(result, next);

            return result;
        }

        public static float Min(params float[] nums)
        {
            var result = nums[0];

            for (int i = 1; i < nums.Length; i++)
                result = Math.Min(result, nums[i]);

            return result;
        }

        public static float Min(IEnumerable<float> nums)
        {
            var result = float.MaxValue;

            foreach (var next in nums)
                result = Math.Min(result, next);

            return result;
        }

        public static float Mod(this float num, float divisor)
        {
            return (num % divisor + divisor) % divisor;
        }

        public static float Percent(this float num, float from, float to)
        {
            var lowest = Math.Min(from, to);
            var highest = Math.Max(from, to);

            return MathHelper.Clamp((num - lowest) / highest, 0f, 1f);
        }

        public static float Pow(this float num, float exponent)
        {
            return (float)Math.Pow(num, exponent);
        }

        public static float Round(this float num)
        {
            return (float)Math.Round(num);
        }

        public static float Round(this float num, int decimals)
        {
            return (float)Math.Round(num, decimals);
        }

        public static float Sign(this float num)
        {
            return Math.Sign(num);
        }

        public static float SignAngleDiff(float radiansA, float radiansB)
        {
            return Math.Sign(AngleDiff(radiansA, radiansB));
        }

        public static float Sin(this float num)
        {
            return (float)Math.Sin(num);
        }

        public static float Sinh(this float num)
        {
            return (float)Math.Sinh(num);
        }

        public static float Sqrt(this float num)
        {
            return (float)Math.Sqrt(num);
        }

        public static float Tan(this float num)
        {
            return (float)Math.Tan(num);
        }

        public static float Tanh(this float num)
        {
            return (float)Math.Tanh(num);
        }

        public static float ToDeg(this float radians)
        {
            return MathHelper.ToDegrees(radians);
        }

        public static float ToRad(this float degrees)
        {
            return MathHelper.ToRadians(degrees);
        }

        public static float Truncate(this float num)
        {
            return (float)Math.Truncate(num);
        }

        public static float WrapAngle(this float radians)
        {
            return radians.Mod(MathHelper.TwoPi);
        }

        public static float WrapAngleDeg(this float degrees)
        {
            return degrees.Mod(360f);
        }

        public static float YoYo(float percent)
        {
            percent = MathHelper.Clamp(percent, 0f, 1f);

            return percent <= .5f
                ? percent * 2f
                : 1f - ((percent - .5f) * 2f);
        }
        #endregion

        #region Point
        public static Point Abs(this Point point)
        {
            return new Point(Math.Abs(point.X), Math.Abs(point.Y));
        }

        public static Point Approach(this Point vec, Point target, int distance)
        {
            if (vec == target || distance == 0)
                return vec;

            var diff = (target - vec).ToVector2();
            if (diff.LengthSquared() < distance * distance)
                return target;

            var result = vec.ToVector2() + Vector2.Normalize(diff) * distance;
            return new Point(
                (int)Math.Round(result.X),
                (int)Math.Round(result.Y));
        }

        public static bool Between(this Point point, Point min, Point max)
        {
            var lowest = new Point(
                Math.Min(min.X, max.X),
                Math.Min(min.Y, max.Y));
            var highest = new Point(
                Math.Max(min.X, max.X),
                Math.Max(min.Y, max.Y));

            return lowest.X <= point.X && point.X <= highest.X
                && lowest.Y <= point.Y && point.Y <= highest.Y;
        }

        public static Point Clamp(this Point point, Point min, Point max)
        {
            var lowest = new Point(
                Math.Min(min.X, max.X),
                Math.Min(min.Y, max.Y));
            var highest = new Point(
                Math.Max(min.X, max.X),
                Math.Max(min.Y, max.Y));

            return new Point(
                MathHelper.Clamp(point.X, lowest.X, highest.X),
                MathHelper.Clamp(point.Y, lowest.Y, highest.Y));
        }

        public static Point Divide(this Point point, Point divider)
        {
            var divX = divider.X != 0 ? point.X / divider.X : 0;
            var divY = divider.Y != 0 ? point.Y / divider.Y : 0;

            return new Point(divX, divY);
        }

        public static Point Divide(this Point point, int divider)
        {
            var divX = divider != 0 ? point.X / divider : 0;
            var divY = divider != 0 ? point.Y / divider : 0;

            return new Point(divX, divY);
        }

        public static Point Lerp(this Point point, Point target, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);

            var x = (int)Math.Round(point.X + (target.X - point.X) * amount);
            var y = (int)Math.Round(point.Y + (target.Y - point.Y) * amount);

            return new Point(x, y);
        }

        public static Point Lerp(this Point point, Point target, float amount, int distance)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);

            var expected = (target - point).ToVector2() * amount;
            var distanceSq = distance * distance;

            if (expected.LengthSquared() > distanceSq)
                expected = Vector2.Normalize(expected) * distance;

            return new Point(
                (int)Math.Round(point.X + expected.X),
                (int)Math.Round(point.Y + expected.Y));
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

            foreach (var next in points)
            {
                result = new Point(
                    Math.Max(result.X, next.X),
                    Math.Max(result.Y, next.Y));
            }

            return result;
        }

        public static Point Max(params Point[] points)
        {
            var result = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                result = new Point(
                    Math.Max(result.X, points[i].X),
                    Math.Max(result.Y, points[i].Y));
            }

            return result;
        }

        public static Point Max(IEnumerable<Point> points)
        {
            var result = new Point(int.MinValue, int.MinValue);

            foreach (var next in points)
            {
                result = new Point(
                    Math.Max(result.X, next.X),
                    Math.Max(result.Y, next.Y));
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

        public static Point Min(params Point[] points)
        {
            var result = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                result = new Point(
                    Math.Min(result.X, points[i].X),
                    Math.Min(result.Y, points[i].Y));
            }

            return result;
        }

        public static Point Min(IEnumerable<Point> points)
        {
            var result = new Point(int.MaxValue, int.MaxValue);

            foreach (var next in points)
            {
                result = new Point(
                    Math.Min(result.X, next.X),
                    Math.Min(result.Y, next.Y));
            }

            return result;
        }

        public static Point Multiply(this Point point, int factor)
        {
            return new Point(
                point.X * factor,
                point.Y * factor);
        }

        public static Point Multiply(this Point point, Point factor)
        {
            return new Point(
                point.X * factor.X,
                point.Y * factor.Y);
        }

        public static Point Pow(this Point point, int exponent)
        {
            return new Point(
                (int)Math.Pow(point.X, exponent),
                (int)Math.Pow(point.Y, exponent));
        }

        public static Point Sign(this Point point)
        {
            return new Point(
                Math.Sign(point.X),
                Math.Sign(point.Y));
        }

        public static Point Sqrt(this Point point)
        {
            return new Point(
                (int)Math.Round(Math.Sqrt(point.X)),
                (int)Math.Round(Math.Sqrt(point.Y)));
        }
        #endregion

        #region Vectors (2D)
        public static Vector2 Abs(this Vector2 vec)
        {
            return new Vector2(
                Math.Abs(vec.X),
                Math.Abs(vec.Y));
        }

        public static Vector2 Approach(this Vector2 vec, Vector2 target, float distance)
        {
            if (vec == target || distance == 0f)
                return vec;

            var diff = target - vec;

            return diff.LengthSquared() < distance * distance
                ? vec + Vector2.Normalize(diff) * distance : target;
        }

        public static bool Between(this Vector2 vec, Vector2 min, Vector2 max)
        {
            var lowest = Vector2.Min(min, max);
            var highest = Vector2.Max(min, max);

            return lowest.X <= vec.X && vec.X <= highest.X
                && lowest.Y <= vec.Y && vec.Y <= highest.Y;
        }

        public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
        {
            var lowest = Vector2.Min(min, max);
            var highest = Vector2.Max(min, max);

            return new Vector2(
                MathHelper.Clamp(vec.X, lowest.X, highest.X),
                MathHelper.Clamp(vec.Y, lowest.Y, highest.Y));
        }

        public static Vector2 Ceiling(this Vector2 vec)
        {
            return new Vector2(
                (float)Math.Ceiling(vec.X),
                (float)Math.Ceiling(vec.Y));
        }

        public static float Cross(this Vector2 vec, Vector2 other)
        {
            return vec.X * other.Y - vec.Y * other.X;
        }

        public static Vector2 Cross(this Vector2 vec, float scale)
        {
            return new Vector2(scale * vec.Y, -scale * vec.X);
        }

        public static Vector2 Divide(this Vector2 vec, Vector2 divider)
        {
            var divX = divider.X > float.Epsilon ? vec.X / divider.X : 0f;
            var divY = divider.Y > float.Epsilon ? vec.Y / divider.Y : 0f;

            return new Vector2(divX, divY);
        }

        public static Vector2 Divide(this Vector2 vec, float divider)
        {
            var divX = divider > float.Epsilon ? vec.X / divider : 0f;
            var divY = divider > float.Epsilon ? vec.Y / divider : 0f;

            return new Vector2(divX, divY);
        }

        public static Vector2 Floor(this Vector2 vec)
        {
            return new Vector2(
                (float)Math.Floor(vec.X),
                (float)Math.Floor(vec.Y));
        }

        public static Vector2 FromAngle(this float angle, float length)
        {
            var vec = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));

            return vec * length;
        }

        public static Vector2 Inverse(this Vector2 vec)
        {
            return Vector2.One / vec;
        }

        public static Vector2 KeepX(this Vector2 vec)
        {
            return vec * Vector2.UnitX;
        }

        public static Vector2 KeepY(this Vector2 vec)
        {
            return vec * Vector2.UnitY;
        }

        public static Vector2 Lerp(this Vector2 vec, Vector2 target, float amount)
        {
            return vec + (target - vec) * amount;
        }

        public static Vector2 Lerp(
            this Vector2 vec,
            Vector2 target,
            float amount,
            float distance)
        {
            var expected = (target - vec) * MathHelper.Clamp(amount, 0f, 1f);
            var distanceSq = distance * distance;

            if (expected.LengthSquared() > distanceSq)
                expected = Vector2.Normalize(expected) * distance;

            return vec + expected;
        }

        public static Vector2 Max(this Vector2 vec, Vector2 other)
        {
            return Vector2.Max(vec, other);
        }

        public static Vector2 Max(this Vector2 vec, params Vector2[] vectors)
        {
            var result = vec;

            for (int i = 0; i < vectors.Length; i++)
                result = Vector2.Max(result, vectors[i]);

            return result;
        }

        public static Vector2 Max(this Vector2 vec, IEnumerable<Vector2> vectors)
        {
            var result = vec;

            foreach (var next in vectors)
                result = Vector2.Max(result, next);

            return result;
        }

        public static Vector2 Max(params Vector2[] vectors)
        {
            var result = vectors[0];

            for (int i = 1; i < vectors.Length; i++)
                result = Vector2.Max(result, vectors[i]);

            return result;
        }

        public static Vector2 Max(IEnumerable<Vector2> vectors)
        {
            var result = new Vector2(float.MinValue, float.MinValue);

            foreach (var next in vectors)
                result = Vector2.Max(result, next);

            return result;
        }

        public static Vector2 Min(this Vector2 vec, Vector2 other)
        {
            return Vector2.Min(vec, other);
        }

        public static Vector2 Min(this Vector2 vec, params Vector2[] vecs)
        {
            var result = vec;

            for (int i = 0; i < vecs.Length; i++)
                result = Vector2.Min(result, vecs[i]);

            return result;
        }

        public static Vector2 Min(this Vector2 vec, IEnumerable<Vector2> vecs)
        {
            var result = vec;

            foreach (var next in vecs)
                result = Vector2.Min(result, next);

            return result;
        }

        public static Vector2 Min(params Vector2[] vectors)
        {
            var result = vectors[0];

            for (int i = 1; i < vectors.Length; i++)
                result = Vector2.Min(result, vectors[i]);

            return result;
        }

        public static Vector2 Min(IEnumerable<Vector2> vectors)
        {
            var result = new Vector2(float.MaxValue, float.MaxValue);

            foreach (var next in vectors)
                result = Vector2.Min(result, next);

            return result;
        }

        public static Vector2 Rotate(this Vector2 vec, bool counterClockwise = false)
        {
            return counterClockwise
                ? new Vector2(-vec.Y, vec.X)
                : new Vector2(vec.Y, -vec.X);
        }

        public static Vector2 Round(this Vector2 vec)
        {
            return new Vector2(
                (float)Math.Round(vec.X),
                (float)Math.Round(vec.Y));
        }

        public static Vector2 Round(this Vector2 vec, int digits)
        {
            return new Vector2(
                (float)Math.Round(vec.X, digits),
                (float)Math.Round(vec.Y, digits));
        }

        public static Vector2 ToGrid(this Vector2 vec, Vector2 size, Vector2 offset = default)
        {
            var divX = Math.Abs(size.X) > float.Epsilon ? (vec.X - offset.X) / size.X : 0f;
            var divY = Math.Abs(size.Y) > float.Epsilon ? (vec.Y - offset.Y) / size.Y : 0f;

            return new Vector2(
                (float)Math.Floor(divX),
                (float)Math.Floor(divY));
        }

        public static Vector2 SafeNormalize(this Vector2 vec)
        {
            return vec != Vector2.Zero
                ? Vector2.Normalize(vec)
                : vec;
        }

        public static Vector2 Sign(this Vector2 vec)
        {
            return new Vector2(
                Math.Sign(vec.X),
                Math.Sign(vec.Y));
        }

        public static Vector2 SignX(this Vector2 vec)
        {
            return new Vector2(Math.Sign(vec.X), 0f);
        }

        public static Vector2 SignY(this Vector2 vec)
        {
            return new Vector2(0f, Math.Sign(vec.Y));
        }

        public static Vector2 Snap(this Vector2 vec, Vector2 size, Vector2 offset = default)
        {
            var divX = Math.Abs(size.X) > float.Epsilon ? (vec.X - offset.X) / size.X : 0f;
            var divY = Math.Abs(size.Y) > float.Epsilon ? (vec.Y - offset.Y) / size.Y : 0f;

            return new Vector2(
                (float)Math.Floor(divX * size.X),
                (float)Math.Floor(divY * size.Y));
        }

        public static Vector2 Truncate(this Vector2 vec)
        {
            return new Vector2(
                (float)Math.Truncate(vec.X),
                (float)Math.Truncate(vec.Y));
        }

        public static float ToAngle(this Vector2 vec)
        {
            return (float)Math.Atan2(vec.Y, vec.X);
        }

        public static float ToAngle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.Y - from.X);
        }
        #endregion

        #region Vectors (3D)
        public static Vector3 Round(this Vector3 vec)
        {
            return new Vector3(
                (float)Math.Round(vec.X),
                (float)Math.Round(vec.Y),
                (float)Math.Round(vec.Z));
        }

        public static Vector3 Round(this Vector3 vec, int digits)
        {
            return new Vector3(
                (float)Math.Round(vec.X, digits),
                (float)Math.Round(vec.Y, digits),
                (float)Math.Round(vec.Z, digits));
        }
        #endregion
    }

    public static class StringEX
    {
        public const char CarriageReturn = '\r';
        public const char NewLine = '\n';

        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static bool IsIgnoreCase(this string str, params string[] matches)
        {
            if (!string.IsNullOrEmpty(str))
                foreach (var match in matches)
                    if (str.Equals(match, StringComparison.InvariantCultureIgnoreCase))
                        return true;

            return false;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return str.Substring(0, length);
        }

        public static string Mid(this string str, int index, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            index = MathHelper.Clamp(index, 0, str.Length - 1);
            return str.Substring(index, length);
        }

        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var result = string.Empty;
            char nextChar;

            while ((nextChar = reader.ReadChar()) != 0)
                result += nextChar;

            return result;
        }

        public static string Right(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var index = str.Length - length - 1;
            index = MathHelper.Clamp(index, 0, str.Length - 1);
            return str.Substring(index, length);
        }

        public static string PadWithZeros(this int num, int digits)
        {
            return num.ToString().PadLeft(digits, '0');
        }

        public static string ToHex(this int num)
        {
            return num.ToString("X");
        }
    }

    public static class ColorEX
    {
        public static Color FromRGB(int red, int green, int blue, int alpha)
        {
            return new Color(
                MathHelper.Clamp(red, 0, 255),
                MathHelper.Clamp(green, 0, 255),
                MathHelper.Clamp(blue, 0, 255),
                MathHelper.Clamp(alpha, 0, 255));
        }

        public static Color FromRGB(int red, int green, int blue)
        {
            return FromRGB(red, green, blue, 255);
        }

        public static Color FromHSL(int hue, int saturation, int lightness, int alpha)
        {
            hue = hue.Mod(360);

            var s = MathHelper.Clamp(saturation, 0, 100) / 100f;
            var l = MathHelper.Clamp(lightness, 0, 100) / 100f;
            var c = s * (1f - Math.Abs(2f * l - 1f));
            var x = c * (1f - Math.Abs(((hue / 60f) % 2f) - 1f));
            var m = l - c / 2f;
            float r, g, b;

            switch (hue / 60)
            {
                default: r = c + m; g = x + m; b = m; break;
                case 1: r = x + m; g = c + m; b = m; break;
                case 2: r = m; g = c + m; b = x + m; break;
                case 3: r = m; g = x + m; b = c + m; break;
                case 4: r = x + m; g = m; b = c + m; break;
                case 5: r = c + m; g = m; b = x + m; break;
            }

            return new Color(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f),
                alpha.Clamp(0, 255));
        }

        public static Color FromHSL(int hue, int saturation, int lightness)
        {
            return FromHSL(hue, saturation, lightness, 255);
        }

        public static Color FromHSV(int hue, int saturation, int value, int alpha)
        {
            hue = hue.Mod(360);

            var s = MathHelper.Clamp(saturation, 0, 100) / 100f;
            var v = MathHelper.Clamp(value, 0, 100) / 100f;
            var c = s * v;
            var x = c * (1f - Math.Abs(((hue / 60f) % 2f) - 1f));
            var m = v - c;

            float r, g, b;

            switch (hue / 60)
            {
                default: r = c + m; g = x + m; b = m; break;
                case 1: r = x + m; g = c + m; b = m; break;
                case 2: r = m; g = c + m; b = x + m; break;
                case 3: r = m; g = x + m; b = c + m; break;
                case 4: r = x + m; g = m; b = c + m; break;
                case 5: r = c + m; g = m; b = x + m; break;
            }

            return new Color(
                (int)Math.Round(r * 255f),
                (int)Math.Round(g * 255f),
                (int)Math.Round(b * 255f),
                alpha.Clamp(0, 255));
        }

        public static Color FromHSV(int hue, int saturation, int value)
        {
            return FromHSV(hue, saturation, value, 255);
        }

        public static Color FromCMYK(int cyan, int magneta, int yellow, int black, int alpha)
        {
            var c = MathHelper.Clamp(cyan, 0, 100) / 100f;
            var m = MathHelper.Clamp(magneta, 0, 100) / 100f;
            var y = MathHelper.Clamp(yellow, 0, 100) / 100f;
            var k = MathHelper.Clamp(black, 0, 100) / 100f;

            var r = (int)Math.Round(255f * (1f - c) * (1f - k));
            var g = (int)Math.Round(255f * (1f - m) * (1f - k));
            var b = (int)Math.Round(255f * (1f - y) * (1f - k));

            return new Color(r, g, b, alpha.Clamp(0, 255));
        }

        public static Color FromCMYK(int cyan, int magneta, int yellow, int black)
        {
            return FromCMYK(cyan, magneta, yellow, black, 255);
        }

        public static Color FromHex(string hex)
        {
            if (hex.Length >= 6)
            {
                var r = hex[0].HexToByte() * 16 + hex[1].HexToByte();
                var g = hex[2].HexToByte() * 16 + hex[3].HexToByte();
                var b = hex[4].HexToByte() * 16 + hex[5].HexToByte();
                var a = 255;

                if (hex.Length >= 8)
                    a = hex[6].HexToByte() * 16 + hex[7].HexToByte();

                return new Color(r, g, b, a);
            }

            return Color.Black;
        }

        public static Color Invert(this Color color)
        {
            return new Color(
                255 - color.R,
                255 - color.G,
                255 - color.B,
                color.A);
        }
    }

    public static class RandomEX
    {
        internal static Stack<Random> Cache { get; } = new Stack<Random>();

        public static Random Current { get; private set; } = new Random();

        public static void Push()
        {
            Cache.Push(Current);
            Current = new Random();
        }

        public static void Push(int newSeed)
        {
            Cache.Push(Current);
            Current = new Random(newSeed);
        }

        public static void Push(Random random)
        {
            Cache.Push(Current);
            Current = random;
        }

        public static void Pop()
        {
            Current = Cache.Pop();
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float maxValue)
        {
            return random.NextFloat() * maxValue;
        }

        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return minValue + random.NextFloat(maxValue - minValue);
        }

        #region Choose
        public static T Choose<T>(this Random random, params T[] items)
        {
            return items[random.Next(items.Length)];
        }

        public static T Choose<T>(this Random random, List<T> items)
        {
            return items[random.Next(items.Count)];
        }

        public static T Choose<T>(this Random random, IEnumerable<T> items)
        {
            List<T> list;
            if (items is List<T>) list = items as List<T>;
            else list = items.ToList();
            return list[random.Next(list.Count)];
        }

        public static T Choose<T>(this T[] items)
        {
            return Current.Choose(items);
        }

        public static T Choose<T>(this List<T> items)
        {
            return Current.Choose(items);
        }

        public static T Choose<T>(this IEnumerable<T> items)
        {
            return Current.Choose(items);
        }
        #endregion

        #region Shuffle
        public static void Shuffle<T>(this Random random, params T[] items)
        {
            var range = items.Length;

            while (--range > 0)
            {
                var item = items[range];
                var roll = random.Next(range + 1);
                items[range] = items[roll];
                items[roll] = item;
            }
        }

        public static void Shuffle<T>(this Random random, List<T> items)
        {
            var range = items.Count;

            while (--range > 0)
            {
                var item = items[range];
                var roll = random.Next(range + 1);
                items[range] = items[roll];
                items[roll] = item;
            }
        }

        public static IEnumerable<T> Shuffle<T>(this Random random, IEnumerable<T> items)
        {
            var list = items.ToList();
            var range = list.Count;

            while (--range > 0)
            {
                var item = list[range];
                var roll = random.Next(range + 1);
                yield return list[roll];
                list[range] = list[roll];
                list[roll] = item;
            }
        }

        public static void Shuffle<T>(this T[] items)
        {
            Current.Shuffle(items);
        }

        public static void Shuffle<T>(this List<T> items)
        {
            Current.Shuffle(items);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            return Current.Shuffle(items);
        }
        #endregion
    }

    public static class PlotEX
    {
        #region Vertices
        public static Vector2 Center(this Vector2[] verts)
        {
            Vector2 sum = Vector2.Zero;
            Vector2 center;
            int j;

            for (int i = 0; i < verts.Length; i++)
            {
                j = (i + 1) % verts.Length;
                center = (verts[i] + verts[j]) * .5f;
                sum += center;
            }

            return sum.Divide(verts.Length);
        }

        public static Vector2 Min(this Vector2[] verts)
        {
            Vector2 lowest = verts[0];

            for (int i = 1; i < verts.Length; i++)
                lowest = Vector2.Min(lowest, verts[i]);

            return lowest;
        }

        public static Vector2 Max(this Vector2[] verts)
        {
            Vector2 highest = verts[0];

            for (int i = 1; i < verts.Length; i++)
                highest = Vector2.Max(highest, verts[i]);

            return highest;
        }

        public static Vector2[] Normalize(this Vector2[] verts)
        {
            Vector2[] modified = new Vector2[verts.Length];
            Vector2 edge;
            int j;

            for (int i = 0; i < verts.Length; i++)
            {
                j = (i + 1) % verts.Length;
                edge = Vector2.Normalize(verts[i] - verts[j]);
                modified[i] = edge.Rotate(true);
            }

            return modified;
        }

        public static Vector2[] Reduce(this Vector2[] verts)
        {
            var location = verts.Min();
            var size = verts.Max() - location;

            return verts.Reduce(location, size);
        }

        public static Vector2[] Reduce(this Vector2[] verts, Vector2 size)
        {
            return verts.Reduce(Vector2.Zero, size);
        }

        public static Vector2[] Reduce(this Vector2[] verts, Vector2 location, Vector2 size)
        {
            var results = new List<Vector2>();
            var min = Vector2.Min(location, location + size);
            var max = Vector2.Max(location, location + size);
            Vector2 vert, clamped, normal;

            for (int i = 0; i < verts.Length; i++)
            {
                vert = verts[i];
                clamped = vert.Clamp(min, max);

                if (vert == clamped)
                {
                    normal = (vert - location).Divide(size);
                    results.Add(normal);
                }
            }

            return results.ToArray();
        }

        public static Vector2[] Transform(
            this Vector2[] verts,
            Vector2? position = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f,
            bool offsetToOrigin = true)
        {
            var result = new Vector2[verts.Length];
            var location = verts.Min();
            var offset = location + origin.GetValueOrDefault();
            var absolute = offsetToOrigin
                ? position ?? location
                : position.GetValueOrDefault() + offset;

            var transformMatrix = Matrix.CreateTranslation(new Vector3(-offset, 0f))
                * Matrix.CreateRotationZ(angle)
                * Matrix.CreateScale(new Vector3(scale ?? Vector2.One, 1f))
                * Matrix.CreateTranslation(new Vector3(absolute, 0f));

            Vector2.Transform(verts, ref transformMatrix, result);

            return result;
        }

        public static Rectangle ToBounds(this Vector2[] verts)
        {
            var min = verts.Min().Round().ToPoint();
            var max = verts.Max().Round().ToPoint();

            return new Rectangle(min, max - min);
        }

        public static Vector2[] ToConvexHull(this Vector2[] verts, int segments = 8)
        {
            if (verts.Length < 3)
                return verts;

            segments = Math.Min(verts.Length, segments);

            var farIndex = 0;
            var farX = verts[0].X;

            for (int i = 1; i < segments; i++)
            {
                if (farX < verts[i].X)
                {
                    farIndex = i;
                    farX = verts[i].X;
                }
                else if (farX == verts[i].X
                    && verts[farIndex].Y > verts[i].Y)
                {
                    farIndex = i;
                }
            }

            var hull = new int[segments];
            var index = farIndex;
            var outCount = 0;

            while (true)
            {
                hull[outCount] = index;
                var nextIndex = 0;

                for (int i = 1; i < segments; i++)
                {
                    if (nextIndex == index)
                    {
                        nextIndex = i;
                        continue;
                    }

                    var hullIndex = hull[outCount];
                    var edgeA = verts[nextIndex] - verts[hullIndex];
                    var edgeB = verts[i] - verts[hullIndex];
                    var det = edgeA.Cross(edgeB);

                    var dotProdA = Vector2.Dot(edgeA, edgeA);
                    var dotProdB = Vector2.Dot(edgeB, edgeB);

                    if (det < 0 || det == 0 && dotProdB > dotProdA)
                        nextIndex = i;
                }

                outCount++;
                index = nextIndex;

                if (nextIndex == farIndex)
                    break;
            }

            var result = new Vector2[segments];

            for (int i = 0; i < outCount; i++)
            {
                var hullIndex = hull[i];
                result[i] = verts[hullIndex];
            }

            return result;
        }

        public static Vector2[] ToOrigin(this Vector2[] verts)
        {
            return verts.Transform(-verts.Min());
        }
        #endregion

        #region Rectangles
        public static Rectangle Intersect(this Rectangle rect, Rectangle other)
        {
            var min = MathEX.Max(rect.Location, other.Location);
            var max = MathEX.Min(rect.Location + rect.Size, other.Location + other.Size);

            return new Rectangle(min, max - min);
        }

        public static Rectangle Snap(
            this Rectangle rect,
            Point size,
            Point offset = default)
        {
            return rect.Snap(size.ToVector2(), offset.ToVector2());
        }

        public static Rectangle Snap(
            this Rectangle rect,
            Vector2 size,
            Vector2 offset = default)
        {
            var min = rect.Location.ToVector2();
            var max = (rect.Location + rect.Size).ToVector2();

            min = (min - offset).Divide(size).Floor() * size;
            max = (max - offset).Divide(size).Ceiling() * size;

            return new Rectangle(
                min.ToPoint(),
                (max - min).ToPoint());
        }

        public static Rectangle ToGrid(
            this Rectangle rect,
            Point size,
            Point offset = default)
        {
            return rect.ToGrid(size.ToVector2(), offset.ToVector2());
        }

        public static Rectangle ToGrid(
            this Rectangle rect,
            Vector2 size,
            Vector2 offset = default)
        {
            var min = rect.Location.ToVector2();
            var max = (rect.Location + rect.Size).ToVector2();

            min = (min - offset).Divide(size).Floor();
            max = (max - offset).Divide(size).Ceiling();

            return new Rectangle(
                min.ToPoint(),
                (max - min).ToPoint());
        }

        public static Vector2[] ToVerts(this Rectangle rect)
        {
            var location = rect.Location.ToVector2();
            var size = rect.Size.ToVector2();

            return new[]
            {
                location,
                location + size * Vector2.UnitX,
                location + size,
                location + size * Vector2.UnitY
            };
        }

        public static Rectangle Transform(
            this Rectangle rect,
            Vector2? position = null,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f)
        {
            var result = rect.ToVerts().Transform(position, origin, scale, angle);
            var lowest = result.Min().Round().ToPoint();
            var highest = result.Max().Round().ToPoint();

            return new Rectangle(
                lowest.X,
                lowest.Y,
                highest.X - lowest.X,
                highest.Y - lowest.Y);
        }

        public static Rectangle Union(this Rectangle rect, Rectangle other)
        {
            var min = MathEX.Min(rect.Location, other.Location);
            var max = MathEX.Max(rect.Location + rect.Size, other.Location + other.Size);

            return new Rectangle(min, max - min);
        }
        #endregion
    }

    public static class XmlEX
    {
        #region Attributes
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
            T defaultValue = default)
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
            Color defaultValue = default)
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
            Color defaultValue = default)
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
            Color defaultValue = default)
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
            Point defaultValue = default)
        {
            return new Point(
                element.AttrToInt32(nameX, defaultValue.X),
                element.AttrToInt32(nameY, defaultValue.Y));
        }

        public static Point AttrToPoint(
            this XmlElement element,
            Point defaultValue = default)
        {
            return element.AttrToPoint("x", "y", defaultValue);
        }

        public static Vector2 AttrToVector2(
            this XmlElement element,
            string nameX,
            string nameY,
            Vector2 defaultValue = default)
        {
            return new Vector2(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y));
        }

        public static Vector2 AttrToVector2(
            this XmlElement element,
            Vector2 defaultValue = default)
        {
            return element.AttrToVector2("x", "y", defaultValue);
        }

        public static Vector3 AttrToVector3(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameZ,
            Vector3 defaultValue = default)
        {
            return new Vector3(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y),
                element.AttrToSingle(nameZ, defaultValue.Z));
        }

        public static Vector3 AttrToVector3(
            this XmlElement element,
            Vector3 defaultValue = default)
        {
            return element.AttrToVector3("x", "y", "z", defaultValue);
        }

        public static Vector4 AttrToVector4(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameZ,
            string nameW,
            Vector4 defaultValue = default)
        {
            return new Vector4(
                element.AttrToSingle(nameX, defaultValue.X),
                element.AttrToSingle(nameY, defaultValue.Y),
                element.AttrToSingle(nameZ, defaultValue.Z),
                element.AttrToSingle(nameW, defaultValue.W));
        }

        public static Vector4 AttrToVector4(
            this XmlElement element,
            Vector4 defaultValue = default)
        {
            return element.AttrToVector4("x", "y", "z", "w", defaultValue);
        }

        public static Rectangle AttrToRectangle(
            this XmlElement element,
            string nameX,
            string nameY,
            string nameW,
            string nameH,
            Rectangle defaultValue = default)
        {
            return new Rectangle(
                element.AttrToPoint(nameX, nameY, defaultValue.Location),
                element.AttrToPoint(nameW, nameH, defaultValue.Size));
        }

        public static Rectangle AttrToRectangle(
            this XmlElement element,
            Rectangle defaultValue = default)
        {
            return element.AttrToRectangle("x", "y", "width", "height", defaultValue);
        }
        #endregion
    }
}
