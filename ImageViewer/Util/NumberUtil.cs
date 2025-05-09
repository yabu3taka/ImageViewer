using System;

namespace ImageViewer.Util
{
    static class NumberUtil
    {
        public static int CountBit(ulong n)
        {
            int count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }
            return count;
        }

        public static ulong GetBitValue<T>(Func<T, bool> func, T[] ary)
        {
            ulong ret = 0;
            for (int i = 0; i < ary.Length; i++)
            {
                if (func(ary[i]))
                {
                    ret |= (1UL << i);
                }
            }
            return ret;
        }

        public static ulong GetBitValue<T>(Func<T, bool> func, T[,] ary, int y)
        {
            ulong ret = 0;
            int width = ary.GetLength(0);
            for (int i = 0; i < width; i++)
            {
                if (func(ary[i, y]))
                {
                    ret |= (1UL << i);
                }
            }
            return ret;
        }

        public static int ToInt32WithDefault(string value, int def)
        {
            if (!int.TryParse(value, out int iv))
            {
                return def;
            }
            else
            {
                return iv;
            }
        }

        public static T Limit<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }
    }
}
