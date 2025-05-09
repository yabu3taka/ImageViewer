using System;
using System.Collections.Generic;
using System.Linq;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    #region Interface
    interface IFilePosCursor
    {
        FileInfoList FileList { get; }
        public void Reload();

        IFileInfo Current { get; }
        IReadOnlyList<IFileInfo> SelectedItems { get; }
        IReadOnlyList<IFileInfo> ExpandedItems { get; }
        bool SetSelection(IFileInfo finfo);

        bool MultiSelect { get; }
        bool SetSelectedItems(IReadOnlyList<IFileInfo> selections, IFileInfo finfo = null);

        IFilePosViewer GetIFilePosViewer();
    }

    interface IFilePosViewer
    {
        string GetTitle(IFileInfo fileInfo);
    }
    #endregion

    #region Util
    static class FilePosCursorUtil
    {
        public static void ReloadDeeply(this IFilePosCursor c)
        {
            c.Reload();
            if (c.FileList.MyManager is IFileRealManager rman)
            {
                rman.Reload();
            }
        }

        public static int GetMyPos(this IFilePosCursor c) => c.FileList.GetPos(c.Current);

        public static void ResetSelection(this IFilePosCursor c)
        {
            c.SetSelection(c.Current);
        }

        public static bool SetPosIfAssignable(this IFilePosCursor c, IFileInfo finfo)
        {
            return c.SetSelection(c.FileList.IsAssignable(finfo) ? finfo : null);
        }

        public static bool GoToStart(this IFilePosCursor c)
        {
            return c.SetSelection(c.FileList.Items.GetFirst());
        }
        public static bool GoToEnd(this IFilePosCursor c)
        {
            return c.SetSelection(c.FileList.Items.GetLast());
        }
        public static bool GoToStartOrEnd(this IFilePosCursor c, int end)
        {
            if (end > 0)
            {
                return c.GoToEnd();
            }
            else
            {
                return c.GoToStart();
            }
        }

        public static bool GoToNearPos(this IFilePosCursor c)
        {
            return c.SetSelection(c.FileList.GetNearFileInfo(c.Current));
        }

        public static bool GoToNearPosIfNeed(this IFilePosCursor c)
        {
            if (c.FileList.IsAssignable(c.Current))
            {
                return true;
            }
            return c.SetSelection(c.FileList.GetNearFileInfo(c.Current));
        }

        public static bool MovePos(this IFilePosCursor c, int move)
        {
            return c.SetSelection(c.FileList.GetMovedFileInfo(c.Current, move));
        }
        public static bool MoveTo(this IFilePosCursor c, string file)
        {
            IFileInfo finfo = c.FileList.GetFileInfo(file);
            return c.SetSelection(finfo is not null ? c.FileList.GetNearFileInfo(finfo) : null);
        }

        public static IFileBound GetFromToBound(this IFilePosCursor c)
        {
            var items = c.SelectedItems;
            return items.Empty() ? null : FileFromToBound.Create(items[0], items[^1], c.FileList.GetComparer());
        }

        public static IFileBound GetSelectionBound(this IFilePosCursor c)
        {
            var items = c.SelectedItems;
            return items.Empty() ? null : new FileSelectionBound(items);
        }

        public static IFileInfo GetSelectionLast(IReadOnlyList<IFileInfo> list, IFileInfo target)
        {
            return list[0].IsSame(target) ? list[^1] : list[0];
        }

        public static void CallInSelectionMode(this IFilePosCursor c, Action<IFilePosCursor> func)
        {
            IFileInfo fromFinfo = GetSelectionLast(c.SelectedItems, c.Current);

            func(c);

            var bound = FileFromToBound.Create(fromFinfo, c.Current, c.FileList.GetComparer());
            var items = c.FileList.GetFiltered(bound);
            c.SetSelectedItems(items);
        }
    }
    #endregion

    class FilePosCursor : IFilePosCursor, IFilePosViewer
    {
        private FilePosCursor(FileInfoList list,
            IFileInfo finfo,
            IReadOnlyList<IFileInfo> selections)
        {
            FileList = list;
            SetSelectionInternal(finfo, selections);
        }

        #region Interface
        public FileInfoList FileList { get; }

        public void Reload()
        {
            FileList.Reload();
        }

        public IFileInfo Current { get; private set; } = null;
        public IReadOnlyList<IFileInfo> SelectedItems { get; private set; } = [];
        public IReadOnlyList<IFileInfo> ExpandedItems => SelectedItems;

        public bool SetSelection(IFileInfo finfo)
        {
            return SetSelectionInternal(finfo);
        }

        public bool MultiSelect => true;

        public bool SetSelectedItems(IReadOnlyList<IFileInfo> selections, IFileInfo finfo = null)
        {
            var list = selections.Where(FileList.MyManager.IsMime).ToList();

            IFileInfo cur;
            if (list.Count <= 0)
            {
                cur = null;
                list = null;
            }
            else if (list.Count == 1)
            {
                cur = list[0];
            }
            else
            {
                FileList.Sort(list);
                cur = finfo ?? Current;
            }
            return SetSelectionInternal(cur, list);
        }

        public IFilePosViewer GetIFilePosViewer()
        {
            return this;
        }

        public string GetTitle(IFileInfo fileInfo)
        {
            return fileInfo.Title;
        }
        #endregion

        #region List
        public void CommitCondition(FileInfoListCondition cond)
        {
            FileList.CommitCondition(cond);
        }
        #endregion

        #region Current / Selection
        public event Action<FilePosCursor> SelectionChanged;

        private bool SetSelectionInternal(IFileInfo finfo, IReadOnlyList<IFileInfo> selections = null)
        {
            if (finfo is null)
            {
                if (FileList.Count > 0)
                {
                    return false;
                }

                // Clear
                SelectedItems = [];
                Current = null;
                SelectionChanged?.Invoke(this);
                return true;
            }

            SelectedItems = selections ?? [finfo];
            Current = finfo;
            SelectionChanged?.Invoke(this);
            return true;
        }

        public void NotifyBitmapChange()
        {
            SelectionChanged?.Invoke(this);
        }

        public IReadOnlyList<IFileInfo> GetLimitedTargets()
        {
            var items = SelectedItems;
            if (items.Count <= 1)
            {
                if (FileList.Bound is not null)
                {
                    return FileList.Items;
                }
            }
            return items;
        }
        #endregion

        #region Delete Mark
        public bool CurrentDeleteMark
        {
            get
            {
                return Current is not null && FileList.DeleteMarker[Current];
            }
            set
            {
                if (Current is not null)
                {
                    FileList.DeleteMarker[Current] = value;
                }
            }
        }
        #endregion

        #region Clone
        public FilePosCursor Clone()
        {
            return new FilePosCursor(FileList.Clone(), Current, SelectedItems);
        }

        public FilePosCursor CloneSelectedBound()
        {
            var newCond = FileList.StartEdit();
            newCond.FilterType = FileFilterType.All;
            if (SelectedItems.HasMany())
            {
                newCond.Bound = this.GetSelectionBound();
            }
            var list = FileList.Clone();
            list.CommitCondition(newCond);
            return new FilePosCursor(list, Current, null);
        }
        #endregion

        #region Create
        public static FilePosCursor CreateFromFile(IFileManager fman, string name)
        {
            var fileList = new FileInfoList(fman);
            var finfo = fileList.GetFileInfo(name);
            if (finfo is null)
            {
                return null;
            }
            return new FilePosCursor(fileList, finfo, null);
        }

        public static FilePosCursor CreateFirst(IFileManager fman)
        {
            var list = new FileInfoList(fman);
            return new FilePosCursor(list, list.GetFirst(), null);
        }

        public static FilePosCursor Create(FileInfoList list, List<IFileInfo> selections)
        {
            list.GetComparer().SortList(selections);
            return new FilePosCursor(list, selections[0], selections);
        }
        #endregion
    }
}
