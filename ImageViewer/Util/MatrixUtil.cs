using System;
using System.Linq;

namespace ImageViewer.Util
{
    static class MatrixUtil
    {
        public static T[,] Calc<T>(Func<T, T, T> func, T[,] from, T[,] to)
        {
            if (from.Length != to.Length || from.GetLength(0) != to.GetLength(0))
            {
                return null;
            }

            int width = from.GetLength(0);
            int height = from.GetLength(1);
            T[,] ret = new T[width, height];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    ret[x, y] = func(from[x, y], to[x, y]);
                }
            }
            return ret;
        }

        public static short[,] CalcDifference(short[,] from, short[,] to)
        {
            return Calc((x, y) => (short)(x - y), from, to);
        }

        public static short Average(short[,] m)
        {
            return (short)m.Cast<short>().Average(v => v);
        }

        public static void Change<T>(Func<T, T> m, T[,] from)
        {
            int width = from.GetLength(0);
            int height = from.GetLength(1);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    from[x, y] = m(from[x, y]);
                }
            }
        }

        public static void Slide(short[,] m, short v)
        {
            Change(x => (short)(x - v), m);
        }

        public static T Min<T>(T[,] m)
        {
            return m.Cast<T>().Min();
        }
    }
}
