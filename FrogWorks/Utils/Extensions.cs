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
        public static Rectangle Intersect(this Rectangle rect, Rectangle other)
        {
            var x = Math.Max(rect.Left, other.Left);
            var y = Math.Max(rect.Top, other.Top);
            var width = Math.Min(rect.Right, other.Right) - x;
            var height = Math.Min(rect.Bottom, other.Bottom) - y;

            return new Rectangle(x, y, width, height);
        }
        #endregion
    }
}
