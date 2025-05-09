using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFileMarkerAccessor
    {
        bool Modified { get; }
        int Count { get; }
        bool this[IFileInfo finfo] { get; }
        IFileMarker Clone();

        bool Contains(IFileInfo finfo)
        {
            return this[finfo];
        }
        bool NotContains(IFileInfo finfo)
        {
            return !this[finfo];
        }
    }

    interface IFileMarker : IFileMarkerAccessor
    {
        new bool this[IFileInfo finfo] { get; set; }
        void Clear();
    }

    interface IFileMarkerName : IFileMarker
    {
        void SetName(string name, bool value);
    }

    static class FileMarkerUtil
    {
        public static void SetAllMark(this IFileMarker maker, IEnumerable<IFileInfo> list, bool value)
        {
            foreach (var finfo in list)
            {
                maker[finfo] = value;
            }
        }
    }

    abstract class FileMarkerBase : IFileMarker
    {
        private readonly HashSet<string> _mark = FileUtil.CreateHashSet();

        public bool Modified { get; set; }

        public int Count => _mark.Count;

        public abstract bool this[IFileInfo finfo] { get; set; }
        public abstract IFileMarker Clone();

        #region internal
        protected bool GetMarkInternal(string name)
        {
            return _mark.Contains(name);
        }

        protected void SetMarkInternal(string name, bool value)
        {
            if (value)
            {
                _mark.Add(name);
            }
            else
            {
                _mark.Remove(name);
            }
            Modified = true;
        }

        protected void CopyTo(FileMarkerBase marker)
        {
            foreach (string str in _mark)
            {
                marker._mark.Add(str);
            }
        }
        #endregion

        public void Clear()
        {
            _mark.Clear();
            Modified = true;
        }

        public void LoadMaker(string file, Encoding enc)
        {
            if (File.Exists(file))
            {
                foreach (string line in File.ReadLines(file, enc))
                {
                    SetMarkInternal(line, true);
                }
            }
            Modified = false;
        }

        public void LoadMaker(string file)
        {
            LoadMaker(file, Encoding.UTF8);
        }

        public void SaveMaker(string file, IRealFolder folder, Encoding enc)
        {
            var list = new List<string>(_mark);
            FileUtil.Sort(list);

            var output = new StringBuilder();
            foreach (string str in list)
            {
                if (folder.HasFile(str))
                {
                    output.AppendLine(str.ToLowerInvariant());
                }
            }
            FileUtil.WriteOrDeleteFile(file, output.ToString(), enc);

            Modified = false;
        }

        public void SaveMaker(string file, IRealFolder folder)
        {
            SaveMaker(file, folder, new UTF8Encoding(false));
        }

        public void RemoveMarkNotInList(List<string> files)
        {
            _mark.IntersectWith(files);
        }
    }

    class FileMarkerName : FileMarkerBase
    {
        public override bool this[IFileInfo finfo]
        {
            get => GetMarkInternal(finfo.FileName);
            set => SetMarkInternal(finfo.FileName, value);
        }

        public override IFileMarker Clone()
        {
            var ret = new FileMarkerName();
            CopyTo(ret);
            return ret;
        }
    }

    class FileMarkerNameWithoutExtension : FileMarkerBase, IFileMarkerName
    {
        public override bool this[IFileInfo finfo]
        {
            get => GetMarkInternal(Path.GetFileNameWithoutExtension(finfo.FileName));
            set => SetMarkInternal(Path.GetFileNameWithoutExtension(finfo.FileName), value);
        }

        public override IFileMarker Clone()
        {
            var ret = new FileMarkerNameWithoutExtension();
            CopyTo(ret);
            return ret;
        }

        public void SetName(string name, bool value)
        {
            SetMarkInternal(Path.GetFileNameWithoutExtension(name), value);
        }
    }

    class FileMarkerFullPath : FileMarkerBase
    {
        public override bool this[IFileInfo finfo]
        {
            get => GetMarkInternal(finfo.FilePath);
            set => SetMarkInternal(finfo.FilePath, value);
        }

        public override IFileMarker Clone()
        {
            var ret = new FileMarkerFullPath();
            CopyTo(ret);
            return ret;
        }
    }

    class FileInfoUniqueList(FileMarkerBase marker)
    {
        public List<IFileInfo> Result { get; } = [];

        public void Add(IFileInfo finfo)
        {
            if (!marker[finfo])
            {
                marker[finfo] = true;
                Result.Add(finfo);
            }
        }

        public void AddRange(IEnumerable<IFileInfo> list)
        {
            foreach (var tmp in list)
            {
                Add(tmp);
            }
        }
    }

    class FileMarkerFilter(IFileMarker orig, Func<IFileInfo, bool> filter) : IFileMarker
    {
        public bool this[IFileInfo finfo]
        {
            get => orig[finfo];
            set
            {
                if (filter(finfo))
                {
                    orig[finfo] = value;
                }
            }
        }

        public bool Modified => orig.Modified;

        public int Count => orig.Count;

        public void Clear()
        {
            orig.Clear();
        }

        public IFileMarker Clone()
        {
            return new FileMarkerFilter(orig.Clone(), filter);
        }
    }

    class FileMarkerEmpty : IFileMarker
    {
        public bool this[IFileInfo finfo]
        {
            get => false;
            set
            {
            }
        }

        public bool Modified => false;

        public int Count => 0;

        public void Clear()
        {
        }

        public IFileMarker Clone()
        {
            return new FileMarkerEmpty();
        }
    }
}
