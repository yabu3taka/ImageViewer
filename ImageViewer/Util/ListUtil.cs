using System;
using System.Collections.Generic;

namespace ImageViewer.Util
{
    static class ListUtil
    {
        public static int SlidePos<T>(this ICollection<T> list, int i, int move)
        {
            if (i < 0)
            {
                return i;
            }
            return NumberUtil.Limit(i + move, 0, list.Count - 1);
        }

        public static int SlidePos<T>(this IList<T> list, T target, int move)
        {
            return SlidePos(list, list.IndexOf(target), move);
        }

        public static T SlideItemAt<T>(this IList<T> list, int i, int move)
        {
            if (i < 0)
            {
                return default;
            }
            if (list.Count <= 0)
            {
                return default;
            }
            return list[NumberUtil.Limit(i + move, 0, list.Count - 1)];
        }

        public static T SlideItemAt<T>(this IList<T> list, T target, int move)
        {
            return SlideItemAt(list, list.IndexOf(target), move);
        }

        public static T GetFirst<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }
            return list[0];
        }

        public static T GetLast<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }
            return list[^1];
        }

        public static bool Empty<T>(this IReadOnlyList<T> list)
        {
            return list.Count == 0;
        }

        public static bool HasMany<T>(this IReadOnlyList<T> list)
        {
            return list.Count >= 2;
        }

        public static bool HasOne<T>(this IReadOnlyList<T> list)
        {
            return list.Count == 1;
        }

        public static bool HasAny<T>(this IReadOnlyList<T> list)
        {
            return list.Count >= 1;
        }
    }
}
