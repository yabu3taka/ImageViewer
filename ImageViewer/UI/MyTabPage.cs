using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;

using ImageViewer.Comparator;
using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.UI
{
    public enum MyTabPageMode
    {
        Pinned,
        Compare,
        File,
        FileAdd,
        FileEdit,
        None
    }

    public enum MySubPageMode
    {
        None,
        FileAdd,
        FileEdit
    }

    class MyTabPage
    {
        private readonly MyTabPageManager _man;

        public MyTabPage(MyTabPageManager tabm, FilePosCursor c, MyTabPageMode mode)
        {
            _man = tabm;
            SetMainFileCursor(c);
            PageMode = mode;
        }

        #region PageMode
        public MyTabPageMode PageMode { get; }

        public MyTabPageMode PageModeInAction
        {
            get
            {
                if (PageMode == MyTabPageMode.File)
                {
                    if (SubPageAdd)
                    {
                        return MyTabPageMode.FileAdd;
                    }
                    else if (SubPageEdit)
                    {
                        return MyTabPageMode.FileEdit;
                    }
                    return MyTabPageMode.File;
                }
                else
                {
                    return PageMode;
                }
            }
        }

        public bool ShownInMenu
        {
            get
            {
                return PageMode == MyTabPageMode.File;
            }
        }

        public bool PageFile
        {
            get
            {
                if (SubPageRunning)
                {
                    return false;
                }
                return PageMode == MyTabPageMode.File;
            }
        }

        public string FileTitle
        {
            get
            {
                string title = _fileCursor.GetMyManager().Title;
                if (_fileCursor.Current is null)
                {
                    return string.IsNullOrEmpty(title) ? "--" : $"{title}\\";
                }
                return $"{title}\\{_fileCursor.Current.Title}";
            }
        }

        private string GetPrefix()
        {
            switch (PageMode)
            {
                case MyTabPageMode.Compare:
                    if (FileCursor.GetMyManager() is ImageComparationResultViewer)
                    {
                        return "比較: ";
                    }
                    return "比較対象: ";

                case MyTabPageMode.Pinned:
                    return "ピン: ";

                case MyTabPageMode.File:
                    if (SubPageAdd)
                    {
                        return "追加画面: ";
                    }
                    if (TargetedPage)
                    {
                        return "対象: ";
                    }
                    if (FileCursor.GetMyManager() is FileManager sfcon)
                    {
                        if (sfcon.HasSubAddFolder())
                        {
                            return "追加中: ";
                        }
                    }
                    break;
            }
            return "";
        }

        public string WindowTitle => GetPrefix() + FileTitle;

        public string MenuTitle
        {
            get
            {
                string prefix = "";
                if (TargetedPage)
                {
                    prefix = "対象: ";
                }
                return prefix + FileTitle;
            }
        }

        public bool TargetedPage { get; set; }
        #endregion

        #region SubPage
        private MySubPageMode _subPageMdode = MySubPageMode.None;
        public MySubPageMode SubPageMode
        {
            get { return _subPageMdode; }
            set
            {
                if (PageMode == MyTabPageMode.File)
                {
                    _subPageMdode = value;
                    if (_subPageMdode == MySubPageMode.None)
                    {
                        SetSubFileCursor(null);
                    }
                    _man.UpdateCurrentTabPageUIIfSelected(this);
                }
            }
        }

        public bool SubPageRunning => _subPageMdode != MySubPageMode.None;
        public bool SubPageAdd => _subPageMdode == MySubPageMode.FileAdd;
        public bool SubPageEdit => _subPageMdode == MySubPageMode.FileEdit;

        public void ToggleSubPageMdode(MySubPageMode mode)
        {
            SubPageMode = _subPageMdode == mode ? MySubPageMode.None : mode;
        }
        #endregion

        #region SyncMove
        private int _oldPos = -1;

        private bool _syncMove = false;
        public bool SyncMove
        {
            get { return _syncMove; }
            set
            {
                if (value)
                {
                    _man.ClearSyncMove();
                    _oldPos = FileCursor.GetMyPos();
                }
                _syncMove = value;
                _man.UpdateStatusMessageUI();
            }
        }
        #endregion

        #region FileCursor
        private FilePosCursor _fileCursor = null;
        private FilePosCursor _fileOrigCursor = null;

        // 画面に表示するFilePosCursor
        public FilePosCursor FileCursor
        {
            get { return _fileCursor; }
            private set
            {
                if (_fileCursor is not null)
                {
                    _fileCursor.SelectionChanged -= this.Bitmap_SelectionChanged;
                }
                _fileCursor = value;
                _fileCursor.SelectionChanged += this.Bitmap_SelectionChanged;
            }
        }

        // ファイル追加・修正後に戻すFilePosCursor
        public FilePosCursor OrigCursor
        {
            get { return _fileOrigCursor; }
        }

        public void SetSubFileCursor(FilePosCursor c)
        {
            if (c is null)
            {
                FileCursor = _fileOrigCursor;
            }
            else
            {
                FileCursor = c;
            }
        }

        public void SetMainFileCursor(FilePosCursor c)
        {
            _fileOrigCursor = c;
            FileCursor = c;
        }

        public void SetMainFileCursorAndResetMode(FilePosCursor c, SideListType type)
        {
            _sideListType = type;

            var newCond = c.FileList.StartEdit();
            newCond.FilterType = type.MainFilterType;
            c.CommitCondition(newCond);

            SetMainFileCursor(c);

            MySubPageMode mode = MySubPageMode.None;
            if (SubPageAdd)
            {
                if (c.GetMyManager() is FileManager sfcon)
                {
                    if (sfcon.HasSubAddFolder())
                    {
                        mode = MySubPageMode.FileAdd;
                    }
                }
            }
            if (SubPageEdit)
            {
                if (c.GetMyManager() is FileManager sfcon)
                {
                    if (sfcon.HasSubEditFolder())
                    {
                        mode = MySubPageMode.FileEdit;
                    }
                }
            }
            SubPageMode = mode;
        }

        private void Bitmap_SelectionChanged(FilePosCursor cursor)
        {
            if (_man.SelectedTabPage == this)
            {
                _man.UpdateCurrentItemUI();

                if (_syncMove && !SubPageRunning)
                {
                    int newPos = FileCursor.GetMyPos();
                    _man.MovePinnedCursor(newPos - _oldPos);
                    _oldPos = newPos;
                }
            }
        }

        public void SetBound(IFileBound b)
        {
            var newCond = FileCursor.FileList.StartEdit();
            newCond.Bound = b;
            SetCondition(newCond);
        }

        public void SetCondition(FileInfoListCondition newCond)
        {
            if (newCond is not null)
            {
                FileCursor.CommitCondition(newCond);
                SyncMove = false;
            }
        }
        #endregion

        #region SideList
        private SideListType _sideListType = SideListType.All;
        private IFileBound _bound = null;

        public IFilePosCursor SideList
        {
            get
            {
                if (SubPageMode != MySubPageMode.None)
                {
                    return FileCursor;
                }
                else
                {
                    return _sideListType.CreateSideList(this);
                }
            }
        }

        public bool CanSideListType(SideListType type)
        {
            if (type.IsAll())
            {
                return true;
            }
            if (SubPageRunning)
            {
                return false;
            }
            if (PageMode == MyTabPageMode.Pinned)
            {
                return false;
            }
            return type.IsSupport(FileCursor.GetMyManager());
        }

        public SideListType SideListType
        {
            get { return _sideListType; }
            set
            {
                if (!CanSideListType(value))
                {
                    return;
                }

                var oldType = _sideListType;
                _sideListType = value;

                var newCond = FileCursor.FileList.StartEdit();
                newCond.FilterType = value.MainFilterType;

                if (value.IsNoBound())
                {
                    if (!oldType.IsNoBound())
                    {
                        _bound = newCond.Bound;
                    }
                    newCond.Bound = null;
                }
                else
                {
                    if (_bound is not null)
                    {
                        newCond.Bound = _bound;
                        _bound = null;
                    }
                }

                SetCondition(newCond);
                FileCursor.GoToNearPosIfNeed();

                if (_man.SelectedTabPage == this)
                {
                    _man.UpdateSurroundingUI(SideBarOpenReason.SideListType);
                }
            }
        }

        public void SetSideListTypeAndKeepOpen(SideListType t)
        {
            _man.SetKeepOpen();
            SideListType = t;
        }

        public void ToggleSideListType(SideListType type)
        {
            if (CanSideListType(type))
            {
                if (SideListType == type)
                {
                    SideListType = SideListType.All;
                }
                else
                {
                    SideListType = type;
                }
            }
        }

        public FileFromToBoundList GetIndexBoundList()
        {
            var fman = FileCursor.GetMyManager();
            if (SideListType.Group.Equals(SideListType))
            {
                return fman.GetGroupBoundList();
            }
            return fman.GetIndexBoundList();
        }
        #endregion

        #region Manager
        public FileManager GetMainFileManager()
        {
            if (_fileOrigCursor.GetMyManager() is FileManager fman)
            {
                return fman;
            }
            return null;
        }

        public IRealFolder GetExplorerFolder()
        {
            if (_fileOrigCursor.Current is IRealFolder folder)
            {
                return folder;
            }
            if (_fileOrigCursor.GetMyManager() is FileManager fman)
            {
                return fman;
            }
            return null;
        }

        public IRealFolder GetDirectFolder()
        {
            if (_fileCursor.Current is IRealFolder folder)
            {
                return folder;
            }
            if (_fileCursor.GetMyManager() is FileManager fman)
            {
                return fman;
            }
            if (_fileCursor.GetMyManager() is SubFileManager sman)
            {
                return sman;
            }
            return null;
        }

        public string GetFolderName()
        {
            if (_fileOrigCursor.GetMyManager() is FileDirManager dman)
            {
                return _fileOrigCursor.Current.FileName;
            }
            if (_fileOrigCursor.GetMyManager() is FileManager fman)
            {
                return Path.GetFileName(fman.FilePath);
            }
            return "";
        }

        public IRealFolder GetGalleryFolder()
        {
            if (_fileOrigCursor.GetMyManager() is FileDirManager dman)
            {
                return new RealFolderSimple(dman.FilePath);
            }
            if (_fileOrigCursor.GetMyManager() is FileManager fman)
            {
                return fman.GetParent();
            }
            return null;
        }

        public IFolderSettingController GetFolderSettingFac()
        {
            if (_fileOrigCursor.Current is IFolderSettingController fsf)
            {
                return fsf;
            }
            else if (_fileOrigCursor.GetMyManager() is IFolderSettingController fsf2)
            {
                return fsf2;
            }
            return null;
        }

        public void ReplaceFileManager(IFileLoader loader)
        {
            if (loader is null)
            {
                return;
            }

            var type = SideListType.CreateForNewPage();

            var result = new FileActionReloader();
            var fman = loader.CreateManager(_man);
            var c = loader.CreateCursor(fman, result);

            _man.SubmitActionResult(result, SideBarUpdateReason.Reload);
            _man.ChangeTabPageCursor(this, c, type, SideBarOpenReason.TabPageCursorReplace);
        }
        #endregion

        #region Ability
        public bool CanChgCursor
        {
            get
            {
                if (TargetedPage)
                {
                    return false;
                }
                return PageFile;
            }
        }

        public bool CanDirNavi
        {
            get
            {
                return PageMode == MyTabPageMode.File;
            }
        }

        public bool CanPageClone
        {
            get
            {
                return PageFile;
            }
        }

        public bool CanPageTarget
        {
            get
            {
                return PageMode == MyTabPageMode.File;
            }
        }

        public bool CanCursorPin
        {
            get
            {
                return PageFile || PageMode == MyTabPageMode.Pinned;
            }
        }

        public bool CanSyncMove
        {
            get
            {
                if (!PageFile)
                {
                    return false;
                }
                if (!_man.HasPinnedTarget())
                {
                    return false;
                }
                return true;
            }
        }

        public bool CanItemBound
        {
            get { return PageFile; }
        }

        public bool CanItemBoundArea
        {
            get
            {
                if (!CanItemBound)
                {
                    return false;
                }
                if (FileCursor.SelectedItems.HasMany())
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanItemUnbound
        {
            get
            {
                if (FileCursor.SelectedItems.HasMany())
                {
                    return false;
                }
                if (FileCursor.FileList.Bound is not null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanIndexNavi
        {
            get
            {
                if (PageMode == MyTabPageMode.Compare)
                {
                    return true;
                }
                if (SubPageRunning)
                {
                    return true;
                }
                return FileCursor.GetMyManager() is IFileIndexController;
            }
        }

        public bool CanIndexBound
        {
            get
            {
                return CanIndexControl && CanItemBound;
            }
        }

        public bool CanIndexControl
        {
            get
            {
                if (SubPageEdit)
                {
                    return false;
                }
                return FileCursor.GetMyManager() is IFileIndexController;
            }
        }

        public bool CanDeleteItem
        {
            get
            {
                if (FileCursor.Current is null)
                {
                    return false;
                }
                if (FileCursor.GetMyManager() is IFileDeleteController dcon)
                {
                    return dcon.IsDeletable(FileCursor.Current);
                }
                return false;
            }
        }

        public bool CanPasteItem
        {
            get
            {
                if (FileCursor.Current is null)
                {
                    return false;
                }
                if (FileCursor.GetMyManager() is IFileCutAndPasteController ccon)
                {
                    return ccon.IsPastable(FileCursor.Current);
                }
                return false;
            }
        }

        public bool CanDeleteControl
        {
            get
            {
                return FileCursor.GetMyManager() is IFileDeleteController;
            }
        }

        public bool CanAddFile
        {
            get
            {
                return PageMode == MyTabPageMode.File;
            }
        }

        public bool CanEditFile
        {
            get
            {
                return PageMode == MyTabPageMode.File;
            }
        }

        public bool CanAddDir
        {
            get
            {
                return GetGalleryFolder() is not null;
            }
        }

        public bool CanRenameFile
        {
            get
            {
                return FileCursor.Current is IRealFile;
            }
        }

        public bool CanRenameFolder
        {
            get
            {
                return FileCursor.GetMyManager() is IRealFolder || FileCursor.Current is IRealFolder;
            }
        }

        public bool CanCompare
        {
            get
            {
                if (SubPageRunning)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region Action: Tab
        public void TabPin()
        {
            if (PageMode == MyTabPageMode.Pinned)
            {
                if (!FileCursor.SelectedItems.HasMany())
                {
                    SetBound(FileCursor.GetSelectionBound());
                }
                else
                {
                    return;
                }
            }
            else
            {
                _man.PinnedCursor = FileCursor;
                FileCursor.ResetSelection();
            }
        }
        #endregion

        #region Action: Bound/Select
        public void AreaSet()
        {
            if (FileCursor.SelectedItems.HasMany())
            {
                SetBound(FileCursor.GetFromToBound());
                FileCursor.ResetSelection();
            }
            else
            {
                SetBound(null);
            }
            _man.UpdateFileListUI(SideBarUpdateReason.ConditionChange);
            _man.UpdateStatusMessageUI();
        }

        public void AreaFile(FileInfoListCondition cond)
        {
            SetCondition(cond);
            _man.UpdateFileListUI(SideBarUpdateReason.ConditionChange);
            _man.UpdateStatusMessageUI();
        }

        public void AreaImgSize(FileInfoListCondition cond)
        {
            SetCondition(cond);
            _man.UpdateFileListUI(SideBarUpdateReason.ConditionChange);
            _man.UpdateStatusMessageUI();
        }

        public void SelectAll()
        {
            if (FileCursor.FileList.Any)
            {
                FileCursor.SetSelectedItems(FileCursor.FileList.Items);
            }
        }
        #endregion

        #region Action: File
        public void SwapFile(int move)
        {
            SideListType slt = SideListType;
            if (SubPageMode != MySubPageMode.None)
            {
                slt = SideListType.All;
            }

            var action = slt.Swap(FileCursor, move);
            if (action is null)
            {
                return;
            }

            var result = action.Commit();
            if (result is not null)
            {
                _man.SubmitActionResult(result, SideBarUpdateReason.FileRename);
            }
        }
        #endregion

        #region Action: Delete
        public IReadOnlyList<IFileInfo> GetExpandedItems()
        {
            return _man.IsSideListFocused() ? SideList.ExpandedItems : FileCursor.ExpandedItems;
        }

        public void DeleteMark()
        {
            bool flag = !FileCursor.CurrentDeleteMark;
            
            foreach (var finfo in GetExpandedItems())
            {
                FileCursor.FileList.DeleteMarker[finfo] = flag;
            }
            var updater = FileActionMarkReloader.Create(FileCursor.GetMyManager(), FileActionMark.Delete);
            _man.SubmitActionResult(updater, SideBarUpdateReason.MarkChange);
        }

        public void DeleteNow()
        {
            var result = new FileActionResult();
            foreach (var finfo in GetExpandedItems())
            {
                result.DoCommit(new FileDeletionActionRecycle(finfo));
            }
            _man.SubmitActionResult(result, SideBarUpdateReason.FileDelete);
        }

        public void DeleteReset()
        {
            FileCursor.FileList.DeleteMarker.Clear();
            var updater = FileActionMarkReloader.Create(FileCursor.GetMyManager(), FileActionMark.Delete);
            _man.SubmitActionResult(updater, SideBarUpdateReason.MarkReset);
        }

        public void DeleteCommit()
        {
            var result = new FileActionResult();
            foreach (var finfo in FileCursor.FileList.GetDeleteMarkedList())
            {
                result.DoCommit(new FileDeletionActionRecycle(finfo));
            }
            FileCursor.FileList.DeleteMarker.Clear();
            _man.SubmitActionResult(result, SideBarUpdateReason.FileDelete);
        }
        #endregion

        #region Action: Index
        public void IndexToggle()
        {
            var icon = (IFileIndexController)FileCursor.GetMyManager();
            var finfo = FileCursor.Current;
            icon.IndexMarker[finfo] = !icon.IndexMarker[finfo];
            var updater = FileActionMarkReloader.Create(FileCursor.GetMyManager(), FileActionMark.Index);
            _man.SubmitActionResult(updater, SideBarUpdateReason.MarkChange);
        }

        public void IndexMove(int move)
        {
            var list = GetIndexBoundList();
            var finfo = list.GetAreaMoved(FileCursor.Current, move);
            if (finfo is null)
            {
                return;
            }

            SetBound(null);
            FileCursor.SetPosIfAssignable(finfo);
            _man.UpdateFileListUI(SideBarUpdateReason.ConditionChange);
        }

        public void IndexIn()
        {
            var list = GetIndexBoundList();
            int pos = list.GetAreaPos(FileCursor.Current);
            if (pos >= 0)
            {
                SetBound(list.GetBound(pos));
                _man.UpdateFileListUI(SideBarUpdateReason.ConditionChange);
            }
        }

        public void IndexReset()
        {
            var icon = (IFileIndexController)FileCursor.GetMyManager();
            icon.IndexMarker.Clear();
            icon.ReloadIndex();

            var updater = FileActionMarkReloader.Create(FileCursor.GetMyManager(), FileActionMark.Index);
            _man.SubmitActionResult(updater, SideBarUpdateReason.MarkReset);
        }

        public void IndexAuto()
        {
            var icon = (IFileIndexController)FileCursor.GetMyManager();
            icon.IndexMarker.Clear();
            var tmp = FileCursor.GetMyManager().GetAllItems().GetFileGroupList();
            icon.IndexMarker.SetAllMark(tmp, true);

            var updater = FileActionMarkReloader.Create(FileCursor.GetMyManager(), FileActionMark.Index);
            _man.SubmitActionResult(updater, SideBarUpdateReason.MarkChange);
        }

        public void IndexSave()
        {
            var icon = (IFileIndexController)FileCursor.GetMyManager();
            icon.SaveIndex();
            _man.UpdateFileListUI(SideBarUpdateReason.MarkSave);
        }
        #endregion
    }
}
