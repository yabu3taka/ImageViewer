using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFileBound
    {
        Func<IFileInfo, bool> CreateFilter();
    }

    #region FileFromToBound / FileFromToBoundList
    class FileFromToBound : IFileBound
    {
        private readonly IFileInfo _fromFileInfo;
        private readonly IFileInfo _toFileInfo;
        private readonly IFileComparer _comparer;

        private FileFromToBound(IFileInfo fromInfo, IFileInfo toInfo, IFileComparer comparer)
        {
            _fromFileInfo = fromInfo;
            _toFileInfo = toInfo;
            _comparer = comparer;
        }

        public static FileFromToBound Create(IFileInfo fromInfo, IFileInfo toInfo, IFileComparer comparer)
        {
            if (fromInfo is null && toInfo is null)
            {
                return null;
            }
            return new FileFromToBound(fromInfo, toInfo, comparer);
        }

        public Func<IFileInfo, bool> CreateFilter()
        {
            var comparer = _comparer;
            if (_fromFileInfo is null || _toFileInfo is null)
            {
                if (_fromFileInfo is not null)
                {
                    return FilterFrom;
                }
                return FilterTo;
            }
            if (comparer.Compare(_fromFileInfo, _toFileInfo) < 0)
            {
                return GetFilterInternal(_fromFileInfo, _toFileInfo, comparer);
            }
            else
            {
                return GetFilterInternal(_toFileInfo, _fromFileInfo, comparer);
            }
        }

        private bool FilterFrom(IFileInfo finfo)
        {
            return _comparer.Compare(_fromFileInfo, finfo) <= 0;
        }
        private bool FilterTo(IFileInfo finfo)
        {
            return _comparer.Compare(finfo, _toFileInfo) <= 0;
        }

        private static Func<IFileInfo, bool> GetFilterInternal(IFileInfo fromInfo, IFileInfo toInfo, IFileComparer comparer)
        {
            return finfo =>
            {
                if (comparer.Compare(fromInfo, finfo) > 0)
                {
                    return false;
                }
                if (comparer.Compare(finfo, toInfo) > 0)
                {
                    return false;
                }
                return true;
            };
        }
    }

    class FileFromToBoundList
    {
        private readonly FileInfoList _fileList;
        private readonly List<IFileInfo> _indexList;
        private readonly IFileMarker _marker;

        public FileFromToBoundList(IFileManager fman, IFileMarkerAccessor marker)
        {
            _fileList = new FileInfoList(fman);

            _marker = marker.Clone();
            var items = _fileList.Items;
            var finfo = items.GetFirst();
            if (finfo is not null)
            {
                _marker[finfo] = true;
            }

            _indexList = items.Where(_marker.Contains).ToList();
        }

        public int GetAreaPos(IFileInfo target)
        {
            var c = _fileList.GetComparer();
            return _indexList.FindLastIndex(index => c.Compare(index, target) <= 0);
        }

        private IFileInfo GetAreaStartForMove(int pos)
        {
            if (pos >= _indexList.Count)
            {
                return _fileList.Items.GetLast();
            }
            if (pos < 0)
            {
                pos = 0;
            }
            return _indexList[pos];
        }

        public IFileInfo GetAreaStart(int pos)
        {
            if (pos >= _indexList.Count)
            {
                return null;
            }
            if (pos < 0)
            {
                return null;
            }
            return _indexList[pos];
        }

        public IFileInfo GetAreaStart(IFileInfo target)
        {
            int pos = GetAreaPos(target);
            return GetAreaStart(pos);
        }

        public int Count => _indexList.Count;

        public IFileBound GetBound(int pos)
        {
            if (pos >= _indexList.Count - 1)
            {
                return FileFromToBound.Create(_indexList[^1], null, _fileList.GetComparer());
            }
            if (pos < 0)
            {
                pos = 0;
            }
            return FileFromToBound.Create(_indexList[pos], _fileList.GetMovedFileInfo(_indexList[pos + 1], -1), _fileList.GetComparer());
        }

        public IFileInfo GetAreaMoved(IFileInfo finfo, int move)
        {
            int pos = GetAreaPos(finfo);
            if (pos < 0)
            {
                return null;
            }

            if (move < 0)
            {
                if (!_marker[finfo])
                {
                    move += 1;
                }
            }
            return GetAreaStartForMove(pos + move);
        }

        public IReadOnlyList<IFileInfo> GetFileList(int pos)
        {
            var bound = GetBound(pos);
            return _fileList.Items.Where(bound.CreateFilter()).ToList();
        }

        public Dictionary<string, int> GetFileCountHash()
        {
            var ret = FileUtil.CreateDictionary<int>();
            var items = _fileList.Items;
            foreach (int pos in Enumerable.Range(0, Count))
            {
                var bound = GetBound(pos);
                ret[_indexList[pos].FileName] = items.Count(bound.CreateFilter());
            }
            return ret;
        }

        public IReadOnlyList<IFileInfo> ExpandGroup(IEnumerable<IFileInfo> list)
        {
            var maker = new FileInfoUniqueList(new FileMarkerName());
            var items = _fileList.Items;
            foreach (int pos in Enumerable.Range(0, Count))
            {
                var bound = GetBound(pos);
                var filter = bound.CreateFilter();
                foreach (var finfo in list)
                {
                    if (filter(finfo))
                    {
                        maker.AddRange(items.Where(filter));
                        break;
                    }
                }
            }
            return maker.Result;
        }
    }
    #endregion

    class FileNameStyleBound(FileNameStyle style) : IFileBound
    {
        public FileNameStyle NameStyle { get; } = style;

        public Func<IFileInfo, bool> CreateFilter() => Filter;

        private bool Filter(IFileInfo finfo)
        {
            return NameStyle.IsContained(finfo);
        }
    }

    class FileSelectionBound : IFileBound
    {
        private readonly FileMarkerFullPath _marker = new();

        public FileSelectionBound(IEnumerable<IFileInfo> selections)
        {
            foreach (var finfo in selections)
            {
                _marker[finfo] = true;
            }
        }

        public Func<IFileInfo, bool> CreateFilter() => Filter;

        private bool Filter(IFileInfo finfo)
        {
            return _marker[finfo];
        }
    }

    #region FileWidthHeightBound
    class FileWidthHeightCache
    {
        private readonly Dictionary<string, Size> _sizeCache = FileUtil.CreateDictionary<Size>();

        public Size GetImgSize(IFileInfo finfo)
        {
            if (_sizeCache.TryGetValue(finfo.FilePath, out Size value))
            {
                return value;
            }

            var img = finfo.GetBitmap();
            Size s;
            if (img is null)
            {
                s = new Size(0, 0);
            }
            else
            {
                s = img.Size;
            }
            _sizeCache[finfo.FilePath] = s;
            return s;
        }

        public void Add(IFileInfo finfo)
        {
            GetImgSize(finfo);
        }

        public int Count => _sizeCache.Count;

        public int WidthMax => _sizeCache.Values.Max(v => v.Width);
        public int WidthMin => _sizeCache.Values.Min(v => v.Width);

        public int HeightMax => _sizeCache.Values.Max(v => v.Height);
        public int HeightMin => _sizeCache.Values.Min(v => v.Height);
    }

    class FileWidthHeightBound : IFileBound
    {
        public int Width { get; }
        public int Height { get; }
        private FileWidthHeightCache _sizeCache;

        private FileWidthHeightBound(int w, int h)
        {
            Width = w;
            Height = h;
            _sizeCache = new();
        }

        public static FileWidthHeightBound Create(string width, string height)
        {
            return Create(NumberUtil.ToInt32WithDefault(width, -1), NumberUtil.ToInt32WithDefault(height, -1));
        }
        public static FileWidthHeightBound Create(int width, int height)
        {
            if (width <= 0 && height <= 0)
            {
                return null;
            }
            return new FileWidthHeightBound(width, height);
        }

        public FileWidthHeightCache Cache
        {
            set
            {
                if (value is not null)
                {
                    _sizeCache = value;
                }
            }
        }

        public Func<IFileInfo, bool> CreateFilter()
        {
            if (Width <= 0)
            {
                return FilterHeight;
            }
            else if (Height <= 0)
            {
                return FilterWidth;
            }
            else
            {
                return FilterBoth;
            }
        }

        private bool FilterWidth(IFileInfo finfo)
        {
            var s = _sizeCache.GetImgSize(finfo);
            return s.Width >= Width;
        }
        private bool FilterHeight(IFileInfo finfo)
        {
            var s = _sizeCache.GetImgSize(finfo);
            return s.Height >= Height;
        }
        private bool FilterBoth(IFileInfo finfo)
        {
            var s = _sizeCache.GetImgSize(finfo);
            return s.Width >= Width || s.Height >= Height;
        }
    }
    #endregion
}
