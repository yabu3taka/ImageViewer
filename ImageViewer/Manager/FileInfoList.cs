using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    enum FileFilterType { All, Stable, Delete, Index, Group, AddDir, NameDir }

    static class FileFilterTypeUtil
    {
        public static bool IsSupport(this IFileManager man, FileFilterType type)
        {
            return type switch
            {
                FileFilterType.All => true,
                FileFilterType.Stable or FileFilterType.Delete => man is IFileDeleteController,
                FileFilterType.Index => man is IFileIndexController,
                FileFilterType.Group => man is IFileGroupController,
                FileFilterType.AddDir => man is FileDirManager,
                FileFilterType.NameDir => man is FileDirManager,
                _ => false,
            };
        }
    }

    class FileInfoList
    {
        public FileInfoList(IFileManager fman, FileInfoListCondition cond)
        {
            MyManager = fman;

            if (MyManager is IFileDeleteController dcon)
            {
                DeleteMarker = new FileMarkerFilter(dcon.CreateDeleteMarker(), dcon.IsDeletable);
            }
            else
            {
                DeleteMarker = new FileMarkerEmpty();
            }

            CommitCondition(cond);
        }

        public FileInfoList(IFileManager fman) : this(fman, null)
        {
        }

        public IFileManager MyManager { get; }

        public FileInfoList Clone()
        {
            return new FileInfoList(MyManager, StartEdit());
        }

        public static FileInfoList Create(IFileManager fman, FileFilterType filterType)
        {
            var cond = new FileInfoListCondition
            {
                FilterType = filterType
            };
            return new FileInfoList(fman, cond);
        }

        public static FileInfoList CreateForSideList(IFilePosCursor c, FileFilterType filterType)
        {
            var ret = Create(c.GetMyManager(), filterType);
            ret.DeleteMarker = c.FileList.DeleteMarker;
            return ret;
        }

        #region Condition
        public FileInfoListCondition StartEdit()
        {
            var ret = new FileInfoListCondition
            {
                FileOrder = _fileComparerOutside,
                Bound = Bound,
                FilterType = FilterType
            };
            return ret;
        }

        public void CommitCondition(FileInfoListCondition cond)
        {
            _fileComparerOutside = cond?.FileOrder;
            CommitFilterAndBound(cond);
            ReloadList();
        }
        #endregion

        #region Filter
        public IFileBound Bound { get; private set; }

        public FileFilterType FilterType { get; private set; } = FileFilterType.All;

        private void CommitFilterAndBound(FileInfoListCondition cond)
        {
            Bound = cond?.Bound;
            FilterType = cond?.FilterType ?? FileFilterType.All;
        }

        private List<Func<IFileInfo, bool>> GetFilterList()
        {
            List<Func<IFileInfo, bool>> filterList = [];
            if (Bound is not null)
            {
                filterList.Add(Bound.CreateFilter());
            }

            switch (FilterType)
            {
                case FileFilterType.Stable:
                    filterList.Add(DeleteMarker.NotContains);
                    break;
                case FileFilterType.Delete:
                    filterList.Add(DeleteMarker.Contains);
                    break;
                case FileFilterType.Index:
                    var marker3 = ((IFileIndexController)MyManager).IndexMarker;
                    filterList.Add(marker3.Contains);
                    break;
                case FileFilterType.Group:
                    var marker = ((IFileGroupController)MyManager).GetGroupMarker();
                    filterList.Add(marker.Contains);
                    break;
                case FileFilterType.AddDir:
                    var marker2 = ((FileDirManager)MyManager).GetFileAddDirMarker();
                    filterList.Add(marker2.Contains);
                    break;
            }
            return filterList;
        }
        #endregion

        #region Sort
        private IFileComparer _fileComparerOutside = null;
        private IFileComparer _fileComparerDefault = null;

        public IFileComparer GetComparer()
        {
            GetList();
            return _fileComparerOutside ?? _fileComparerDefault;
        }

        public IFileComparer GetDefaultComparer()
        {
            GetList();
            return _fileComparerDefault;
        }

        public void Sort(List<IFileInfo> files)
        {
            GetComparer().SortList(files);
        }
        #endregion

        #region GetList
        private List<IFileInfo> _list = null;
        private int _allCount = 0;

        private List<IFileInfo> GetList()
        {
            if (_list is null)
            {
                List<IFileInfo> files = [];
                var filters = GetFilterList();
                void getFunc(IFileInfo finfo)
                {
                    foreach (var f in filters)
                    {
                        if (!f(finfo))
                        {
                            return;
                        }
                    }
                    files.Add(finfo);
                }
                _allCount = MyManager.ForEach(getFunc, out _fileComparerDefault);
                files.Sort(_fileComparerOutside ?? _fileComparerDefault);

                _list = files;
            }
            return _list;
        }

        public void Reload()
        {
            ReloadList();
        }

        private void ReloadList()
        {
            _list = null;
            _fileComparerDefault = null;
        }

        public void ReSort()
        {
            if (_fileComparerOutside is not null)
            {
                _list.Sort(_fileComparerOutside);
            }
        }

        public int AllCount => _allCount;

        public IReadOnlyList<IFileInfo> Items => GetList();
        #endregion

        #region List Access
        public int Count => Items.Count;

        public bool Any => Items.Count > 0;

        public bool Detail => GetFirst() is IFileDetail;

        public IFileInfo GetFirst()
        {
            if (!Any)
            {
                return null;
            }
            return Items[0];
        }

        public IFileInfo GetFileInfo(string path)
        {
            foreach (var finfo in Items)
            {
                if (finfo.IsSame(path))
                {
                    return finfo;
                }
            }
            return null;
        }

        public List<IFileInfo> GetFileInfo(List<string> files)
        {
            var hash = Items.CreateDictionary();
            return files.Where(hash.ContainsKey)
                .Select(file => hash[file])
                .ToList();
        }

        public int GetPos(IFileInfo target)
        {
            int pos = 0;
            foreach (IFileInfo finfo in Items)
            {
                if (finfo.IsSame(target))
                {
                    return pos;
                }
                ++pos;
            }
            return -1;
        }

        public bool IsAssignable(IFileInfo target)
        {
            if (!MyManager.IsMime(target))
            {
                return false;
            }
            return GetFileInfo(target.FilePath) is not null;
        }

        public IFileInfo GetNearFileInfo(IFileInfo target)
        {
            if (target is null)
            {
                return Items.Any() ? Items[0] : null;
            }

            IFileInfo retFinfo = null;
            var c = GetComparer();
            foreach (var finfo in Items)
            {
                retFinfo = finfo;
                if (c.Compare(finfo, target) >= 0)
                {
                    return finfo;
                }
            }
            return retFinfo;
        }

        public IFileInfo GetMovedFileInfo(IFileInfo target, int move)
        {
            var c = GetComparer();
            if (move > 0)
            {
                return Items.SkipWhile(finfo => c.Compare(finfo, target) <= 0)
                    .ElementAtOrDefault(move - 1);
            }
            else if (move < 0)
            {
                return Items.TakeWhile(finfo => c.Compare(finfo, target) < 0)
                    .Reverse()
                    .ElementAtOrDefault(-move - 1);
            }
            else
            {
                return target;
            }
        }
        public List<IFileInfo> GetFiltered(IFileBound bound)
        {
            var f = bound.CreateFilter();
            return Items.Where(finfo => f(finfo)).ToList();
        }
        #endregion

        #region Delete Marker
        public IFileMarker DeleteMarker { get; private set; }

        public IEnumerable<IFileInfo> GetDeleteMarkedList()
        {
            return _list.Where(DeleteMarker.Contains);
        }
        #endregion

        #region Quick List
        public List<IFileInfo> QuickList(IFileBound bound)
        {
            var newCond = StartEdit();
            newCond.Bound = bound;
            var list = new FileInfoList(MyManager, newCond);
            return list.GetList();
        }
        #endregion
    }

    class FileInfoListCondition
    {
        public IFileComparer FileOrder { get; set; }

        public IFileBound Bound { get; set; }

        public FileFilterType FilterType { get; set; } = FileFilterType.All;
    }
}
