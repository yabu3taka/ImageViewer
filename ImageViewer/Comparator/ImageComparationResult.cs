using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.Comparator
{
    class ImageCloseSet<T> where T : ImageHash
    {
        private readonly T _target;
        private readonly HashSet<string> _list = FileUtil.CreateHashSet();

        public int Count => _list.Count;

        public string Text => Path.GetFileName(_target.FilePath);

        public ImageCloseSet(T hash)
        {
            _target = hash;
            _list.Add(hash.FilePath);
        }

        public bool AddIfClose(ImageCloseFunc<T> close, T hash)
        {
            if (!close(_target, hash))
            {
                return false;
            }
            _list.Add(hash.FilePath);
            return true;
        }

        public bool Merge(ImageCloseSet<T> ics)
        {
            if (_list.Overlaps(ics._list))
            {
                _list.UnionWith(ics._list);
                return true;
            }
            return false;
        }

        public List<string> ToList()
        {
            var key = _target.FilePath;
            var ret = _list.Where(f => !FileUtil.EqualsPath(f, key)).ToList();
            ret.Sort();
            ret.Insert(0, key);
            return ret;
        }
    }

    interface IImageComparationResult
    {
        int Count { get; }
        string GetViewerText(int i);
        ImageComparationResultViewer GetViewer(int i);
    }

    class ImageComparationResult<T> : IImageComparationResult where T : ImageHash
    {
        private readonly List<ImageCloseSet<T>> _icsList = [];

        public void Add(ImageCloseSet<T> newIcs)
        {
            bool isnew = true;
            foreach (var ics in _icsList)
            {
                if (ics.Merge(newIcs))
                {
                    isnew = false;
                    break;
                }
            }
            if (isnew)
            {
                _icsList.Add(newIcs);
            }
        }

        public void AddIfHasPair(ImageCloseSet<T> newIcs)
        {
            if (newIcs.Count >= 2)
            {
                Add(newIcs);
            }
        }

        public int Count => _icsList.Count;

        public bool NotDeleteFirst { get; set; }

        public string GetViewerText(int i)
        {
            return _icsList[i].Text;
        }

        public ImageComparationResultViewer GetViewer(int i)
        {
            return new ImageComparationResultViewer(_icsList[i].ToList(), NotDeleteFirst);
        }
    }

    class ImageComparationResultViewer(List<string> list, bool notDeleteFirst) : IFileManager, IFileDeleteController
    {
        private readonly List<string> _list = list;
        private readonly bool _notDeleteFirst = notDeleteFirst;

        #region As IFileManager
        public string Title => "";

        public int ForEach(Action<IFileInfo> a, out IFileComparer defComparer)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                a(new MyFileInfo(this, i, _list[i]));
            }
            defComparer = new FileComparerId();
            return _list.Count;
        }

        public bool IsMime(IFileInfo finfo)
        {
            if (finfo?.MyManager is ImageComparationResultViewer)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region As IFileDeleteController
        public IFileMarker CreateDeleteMarker()
        {
            return new FileMarkerFullPath();
        }

        public bool IsDeletable(IFileInfo finfo)
        {
            if (_notDeleteFirst)
            {
                if (finfo.Id == 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region FileInfo Class
        private class MyFileInfo(ImageComparationResultViewer manager, int pos, string path) : IFileInfo
        {
            public IFileManager MyManager => manager;
            public IFileImageInfo GetImageInfo()
            {
                return FileImageInfoSimple.Create(FilePath);
            }

            public int Id { get; } = pos;

            public string FileName => Path.GetFileName(FilePath);
            public string FilePath { get; } = path;
            public string Title => Path.GetFileNameWithoutExtension(FilePath);
        }
        #endregion
    }
}
