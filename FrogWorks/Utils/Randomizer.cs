using System;
using System.Collections.Generic;
using System.Linq;

namespace FrogWorks
{
    public static class Randomizer
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

        #region Choosing Items
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

        #region Shuffling Items
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
}
