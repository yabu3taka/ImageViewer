using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.UI
{
    class SideListType : IEquatable<SideListType>
    {
        public static SideListType All { get; } = new(FileFilterType.All);
        public static SideListType Delete { get; } = new(FileFilterType.Delete);
        public static SideListType Index { get; } = new(FileFilterType.Index);
        public static SideListType Group { get; } = new(FileFilterType.Group);
        public static SideListType DirAll { get; } = new(true, FileFilterType.All);

        public static IEnumerable<SideListType> Types
        {
            get
            {
                var ret = new SideListType[]
                {
                    All,
                    new(FileFilterType.Stable),
                    Delete,
                    Index,
                    Group,
                    DirAll,
                    new(true, FileFilterType.AddDir),
                    new(true, FileFilterType.NameDir)
                };
                return ret;
            }
        }

        public bool DirFlg { get; }
        private readonly FileFilterType FilterType;

        public FileFilterType MainFilterType
        {
            get
            {
                if (DirFlg)
                {
                    return FileFilterType.All;
                }

                return FilterType switch
                {
                    FileFilterType.Index or FileFilterType.Group => FileFilterType.All,
                    _ => FilterType,
                };
            }
        }

        private SideListType(FileFilterType filter)
        {
            this.DirFlg = false;
            this.FilterType = filter;
        }

        private SideListType(bool dir, FileFilterType filter)
        {
            this.DirFlg = dir;
            this.FilterType = filter;
        }

        public override string ToString()
        {
            return FilterType switch
            {
                FileFilterType.All => DirFlg ? "フォルダ" : "全て表示",
                FileFilterType.Stable => "維持表示",
                FileFilterType.Delete => "削除確認",
                FileFilterType.Index => "インデックス表示",
                FileFilterType.Group => "グループ表示",
                FileFilterType.AddDir => "フォルダ)追加中",
                FileFilterType.NameDir => "フォルダ)コメント",
                _ => "",
            };
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SideListType);
        }

        public bool Equals(SideListType other)
        {
            return other is not null && this.DirFlg == other.DirFlg && this.FilterType == other.FilterType;
        }

        public override int GetHashCode()
        {
            int slide = DirFlg ? 100 : 0;
            return (int)FilterType + slide;
        }

        public bool IsListForcus()
        {
            return DirFlg;
        }

        public bool IsAll()
        {
            if (DirFlg)
            {
                return false;
            }

            return FilterType == FileFilterType.All;
        }

        public bool IsSupport(IFileManager man)
        {
            if (DirFlg)
            {
                return man is FileManager;
            }

            return man.IsSupport(FilterType);
        }

        public SideListType CreateForNewPage()
        {
            if (DirFlg)
            {
                return this;
            }

            if (IsIndex())
            {
                return this;
            }
            return SideListType.All;
        }

        public bool IsSideOpen()
        {
            if (DirFlg)
            {
                return true;
            }

            return FilterType switch
            {
                FileFilterType.Index or FileFilterType.Delete or FileFilterType.Group => true,
                _ => false,
            };
        }

        public bool IsIndex()
        {
            return FilterType switch
            {
                FileFilterType.Index or FileFilterType.Group => true,
                _ => false,
            };
        }

        public bool IsNoBound()
        {
            return FilterType switch
            {
                FileFilterType.Index or FileFilterType.Delete or FileFilterType.Group => true,
                _ => false,
            };
        }

        public IFilePosCursor CreateSideList(MyTabPage p)
        {
            if (DirFlg)
            {
                return new SideListDirCursor(p, FilterType);
            }

            return FilterType switch
            {
                FileFilterType.Group => new SideListGroupCursor(p),
                FileFilterType.Index => new SideListIndexCursor(p),
                _ => p.FileCursor,
            };
        }

        public IFileAction Swap(FilePosCursor c, int move)
        {
            if (DirFlg)
            {
                return null;
            }

            var fromFinfo = c.Current;
            IFileInfo toFinfo;

            switch (FilterType)
            {
                case FileFilterType.Group:
                    var boundList = c.GetGroupBoundList();
                    int toPos = boundList.GetAreaPos(fromFinfo) + move;
                    toFinfo = boundList.GetAreaStart(toPos);
                    if (toFinfo is null)
                    {
                        return null;
                    }

                    var fromStyle = FileNameUtil.CreateAuto(fromFinfo);
                    var toStyle = FileNameUtil.CreateAuto(toFinfo);
                    return c.GetMyManager().SwapConvert(fromStyle, toStyle);

                case FileFilterType.All:
                    toFinfo = c.FileList.GetMovedFileInfo(fromFinfo, move);
                    if (toFinfo is null)
                    {
                        return null;
                    }
                    return new FileSwapAction(fromFinfo, toFinfo);

                default:
                    return null;
            }
        }
    }

    #region SideListBoundCursorBase
    abstract class SideListBoundCursorBase : IFilePosCursor
    {
        protected readonly FilePosCursor _cursor;

        protected SideListBoundCursorBase(MyTabPage page, FileFilterType t)
        {
            _cursor = page.OrigCursor;
            FileList = FileInfoList.CreateForSideList(_cursor, t);
        }

        public FileInfoList FileList { get; }

        public virtual void Reload()
        {
            FileList.Reload();
        }

        public IFileInfo Current
        {
            get
            {
                var boundList = GetFileFromToBoundList();
                return boundList.GetAreaStart(_cursor.Current);
            }
        }

        public IReadOnlyList<IFileInfo> SelectedItems
        {
            get
            {
                var boundList = GetFileFromToBoundList();
                var posHash = new HashSet<int>();
                foreach (var finfo in _cursor.SelectedItems)
                {
                    posHash.Add(boundList.GetAreaPos(finfo));
                }

                var ret = new List<IFileInfo>();
                foreach (var pos in posHash)
                {
                    ret.Add(boundList.GetAreaStart(pos));
                }
                return ret;
            }
        }

        public IReadOnlyList<IFileInfo> ExpandedItems
        {
            get
            {
                var boundList = GetFileFromToBoundList();
                return boundList.ExpandGroup(_cursor.SelectedItems);
            }
        }

        public bool SetSelection(IFileInfo finfo)
        {
            return _cursor.SetSelection(finfo);
        }

        public bool MultiSelect => _cursor.MultiSelect;

        public bool SetSelectedItems(IReadOnlyList<IFileInfo> selections, IFileInfo finfo = null)
        {
            if (selections.HasMany())
            {
                var boundList = GetFileFromToBoundList();
                selections = boundList.ExpandGroup(selections);
            }
            return _cursor.SetSelectedItems(selections, finfo);
        }

        public IFilePosViewer GetIFilePosViewer()
        {
            return new SideListBoundViewer(GetFileFromToBoundList().GetFileCountHash());
        }

        protected abstract FileFromToBoundList GetFileFromToBoundList();
    }

    class SideListBoundViewer(Dictionary<string, int> countHash) : IFilePosViewer
    {
        public string GetTitle(IFileInfo fileInfo)
        {
            return $"{fileInfo.Title}(C{countHash[fileInfo.FileName]})";
        }
    }

    class SideListGroupCursor(MyTabPage page) : SideListBoundCursorBase(page, FileFilterType.Group)
    {
        protected override FileFromToBoundList GetFileFromToBoundList()
        {
            return _cursor.GetGroupBoundList();
        }
    }

    class SideListIndexCursor(MyTabPage page) : SideListBoundCursorBase(page, FileFilterType.Index)
    {
        protected override FileFromToBoundList GetFileFromToBoundList()
        {
            return _cursor.GetIndexBoundList();
        }
    }
    #endregion

    class SideListDirCursor : IFilePosCursor, IFilePosViewer
    {
        private readonly MyTabPage _page;
        private readonly FilePosCursor _cursor;

        public SideListDirCursor(MyTabPage page, FileFilterType type)
        {
            _page = page;
            _cursor = page.OrigCursor;

            var fman = (FileManager)_cursor.GetMyManager();
            var fdm = new FileDirManager(Path.GetDirectoryName(fman.FilePath));
            switch (type)
            {
                case FileFilterType.NameDir:
                    var cond = new FileInfoListCondition
                    {
                        FilterType = FileFilterType.All,
                        FileOrder = new MyFileComparer()
                    };
                    FileList = new FileInfoList(fdm, cond);
                    break;
                default:
                    FileList = FileInfoList.Create(fdm, type);
                    break;
            }
        }

        public FileInfoList FileList { get; }

        public void Reload()
        {
            FileList.Reload();
        }

        public IFileInfo Current
        {
            get
            {
                var fman = (FileManager)_page.FileCursor.GetMyManager();
                return FileList.GetFileInfo(fman.FilePath);
            }
        }

        public IReadOnlyList<IFileInfo> SelectedItems
        {
            get
            {
                var finfo = Current;
                var ret = new List<IFileInfo>();
                if (finfo is not null)
                {
                    ret.Add(finfo);
                }
                return ret;
            }
        }

        public IReadOnlyList<IFileInfo> ExpandedItems => this.SelectedItems;

        public bool SetSelection(IFileInfo finfo)
        {
            return _cursor.SetSelection(finfo);
        }

        public bool MultiSelect => false;

        public bool SetSelectedItems(IReadOnlyList<IFileInfo> selections, IFileInfo finfo = null)
        {
            finfo ??= selections.GetFirst();
            if (finfo is not null)
            {
                var loader = FileLoaderUtil.Create(finfo.FilePath);
                _page.ReplaceFileManager(loader);
            }
            return true;
        }

        public IFilePosViewer GetIFilePosViewer()
        {
            return this;
        }

        public string GetTitle(IFileInfo fileInfo)
        {
            return fileInfo.Title;
        }

        private class MyFileComparer : IFileComparer
        {
            public int Compare(IFileInfo f1, IFileInfo f2)
            {
                string d1 = ((IFileDetail)f1).DetailTitle;
                string d2 = ((IFileDetail)f2).DetailTitle;

                int c = StringComparer.CurrentCultureIgnoreCase.Compare(d1, d2);
                if (c != 0)
                {
                    if (string.IsNullOrEmpty(d1))
                    {
                        return 1;
                    }
                    if (string.IsNullOrEmpty(d2))
                    {
                        return -1;
                    }

                    return c;
                }

                return StringComparer.OrdinalIgnoreCase.Compare(f1.FileName, f2.FileName);
            }
        }
    }
}
