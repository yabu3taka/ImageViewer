using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageViewer.Manager
{
    #region FileComparer
    interface IFileComparer : IComparer<IFileInfo>
    {
    }

    class FileComparerDescending(IFileComparer fileComparer) : IFileComparer
    {
        public int Compare(IFileInfo f1, IFileInfo f2)
        {
            return fileComparer.Compare(f2, f1);
        }
    }

    class FileComparerSimple : IFileComparer
    {
        public int Compare(IFileInfo f1, IFileInfo f2)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(f1.FileName, f2.FileName);
        }
    }

    class FileComparerId : IFileComparer
    {
        public int Compare(IFileInfo f1, IFileInfo f2)
        {
            return f1.Id.CompareTo(f2.Id);
        }
    }

    class FileComparerFactory
    {
        bool _allNum = true;

        public int GetId(string filename)
        {
            if (int.TryParse(Path.GetFileNameWithoutExtension(filename), out int result))
            {
                return result;
            }

            _allNum = false;
            return -1;
        }

        public IFileComparer Create()
        {
            if (_allNum)
            {
                return new FileComparerId();
            }
            return new FileComparerSimple();
        }
    }
    #endregion

    #region FileSorter
    interface IFileSorter : IFileComparer
    {
        List<IFileInfo> GetSortedList(List<IFileInfo> list);
    }

    abstract class FileKeySorter<T> : IFileSorter where T : IComparable
    {
        public abstract T GetKey(IFileInfo finfo);

        public int Compare(IFileInfo f1, IFileInfo f2)
        {
            return GetKey(f1).CompareTo(GetKey(f2));
        }

        public List<IFileInfo> GetSortedList(List<IFileInfo> list)
        {
            return list.OrderBy(GetKey).ToList();
        }
    }

    class FileKeySorterDescending<T>(FileKeySorter<T> sorter) : IFileSorter where T : IComparable
    {
        public int Compare(IFileInfo f1, IFileInfo f2)
        {
            return sorter.GetKey(f2).CompareTo(sorter.GetKey(f1));
        }

        public List<IFileInfo> GetSortedList(List<IFileInfo> list)
        {
            return [.. list.OrderByDescending(sorter.GetKey)];
        }
    }

    class FileModifiedDate : FileKeySorter<long>
    {
        public override long GetKey(IFileInfo finfo)
        {
            return finfo.LastWriteTime().Ticks;
        }
    }
    #endregion

    enum FileSortType
    {
        FileNameAscend = 0,
        FileNameDescend = 1,
        FileModifiedDateAscend = 2,
        FileModifiedDateDescend = 3,
    }

    static class FileComparerUtil
    {
        public static string[] GetFileComparerTypeNames()
        {
            return [
                "ファイル名（昇順）",
                "ファイル名（降順）",
                "変更日（昇順）",
                "変更日（降順）"
            ];
        }

        public static IFileComparer CreateFileComparer(this FileInfoList list, FileSortType type)
        {
            return type switch
            {
                FileSortType.FileNameDescend => new FileComparerDescending(list.GetDefaultComparer()),
                FileSortType.FileModifiedDateAscend => new FileModifiedDate(),
                FileSortType.FileModifiedDateDescend => new FileKeySorterDescending<long>(new FileModifiedDate()),
                FileSortType.FileNameAscend or _ => list.GetDefaultComparer()
            };
        }

        public static void SortList(this IFileComparer c, List<IFileInfo> list)
        {
            if (c is IFileSorter kc)
            {
                SortList(kc, list);
            }
            else
            {
                list.Sort(c);
            }
        }

        public static void SortList(this IFileSorter kc, List<IFileInfo> list)
        {
            var tmp = kc.GetSortedList(list);
            list.Clear();
            list.AddRange(tmp);
        }
    }
}
