using ImageViewer.Manager;
using ImageViewer.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.UI
{
    interface IMyTabPageManagerCallBack
    {
        void UpdateCurrentTabPageUI(SideBarOpenReason reason);
        void UpdateCurrentItemUI();
        void UpdateSurroundingUI(SideBarOpenReason reason);
        void UpdateStoredCursorUI();
        void UpdateFileListUI(SideBarUpdateInfo info);
        void UpdateSideBarUI(SideBarOpenReason reason);
        void UpdateStatusMessageUI();

        bool IsSideListFocused();
    }

    class MyTabPageManager : IFileManagerCache
    {
        private readonly IMyTabPageManagerCallBack _cb;

        public MyTabPageManager(IMyTabPageManagerCallBack cb, string initDir)
        {
            this._cb = cb;
            this._targetDir = initDir;
        }

        #region CallBack
        public void UpdateCurrentTabPageUI(SideBarOpenReason reason = SideBarOpenReason.TabPageChange)
        {
            _cb.UpdateCurrentTabPageUI(reason);
        }

        public void UpdateCurrentItemUI()
        {
            _cb.UpdateCurrentItemUI();
        }

        public void UpdateSurroundingUI(SideBarOpenReason reason)
        {
            _cb.UpdateSurroundingUI(reason);
        }

        public void UpdateStoredCursorUI()
        {
            _cb.UpdateStoredCursorUI();
        }

        public void UpdateFileListUI(SideBarUpdateReason reason)
        {
            UpdateFileListUI(new SideBarUpdateInfo(reason));
        }
        public void UpdateFileListUI(SideBarUpdateInfo info)
        {
            _cb.UpdateFileListUI(info);
        }

        public void UpdateSideBarUI(SideBarOpenReason reason)
        {
            _cb.UpdateSideBarUI(reason);
        }

        public void UpdateStatusMessageUI()
        {
            _cb.UpdateStatusMessageUI();
        }

        public bool IsSideListFocused()
        {
            return _cb.IsSideListFocused();
        }
        #endregion

        #region TabPage
        private readonly List<MyTabPage> _fileTabPages = [];
        private readonly Dictionary<MyTabPageMode, MyTabPage> _tabPageDic = [];
        private MyTabPage _latestTabPage = null;
        private MyTabPage _selectedTabPage = null;

        public MyTabPage SelectedTabPage
        {
            get { return _selectedTabPage; }
            set
            {
                if (_selectedTabPage != value)
                {
                    if (_selectedTabPage?.ShownInMenu ?? false)
                    {
                        _latestTabPage = _selectedTabPage;
                    }
                    _selectedTabPage = value;
                }
                UpdateCurrentTabPageUI();
            }
        }

        private void SetSelectedTabPageAndUpdateUI(MyTabPage p)
        {
            if (p == SelectedTabPage)
            {
                UpdateCurrentTabPageUI();
            }
            else
            {
                SelectedTabPage = p;
            }
        }

        public void UpdateCurrentTabPageUIIfSelected(MyTabPage p, SideBarOpenReason reason = SideBarOpenReason.TabPageChange)
        {
            if (p == SelectedTabPage)
            {
                UpdateCurrentTabPageUI(reason);
            }
        }

        public MyTabPage LatestTabPage => _latestTabPage ?? SelectedTabPage;

        public IEnumerable<MyTabPage> FileTabPages => _fileTabPages;

        public IEnumerable<MyTabPage> AllTabPages
        {
            get
            {
                var ret = new List<MyTabPage>();
                ret.AddRange(_fileTabPages);
                ret.AddRange(_tabPageDic.Values);
                return ret;
            }
        }

        private MyTabPage AddTabPage(FilePosCursor c, MyTabPageMode mode = MyTabPageMode.File)
        {
            MyTabPage tabPage;
            if (mode == MyTabPageMode.File)
            {
                tabPage = new MyTabPage(this, c, mode);
                _fileTabPages.Add(tabPage);
            }
            else
            {
                tabPage = GetTabPage(mode);
                if (tabPage is null)
                {
                    tabPage = new MyTabPage(this, c, mode);
                    _tabPageDic[mode] = tabPage;
                }
                else
                {
                    tabPage.SetMainFileCursor(c);
                }
            }
            return tabPage;
        }

        public void AddTabPageAndSelect(FilePosCursor c, MyTabPageMode mode = MyTabPageMode.File)
        {
            var p = AddTabPage(c, mode);
            SetSelectedTabPageAndUpdateUI(p);
        }

        public MyTabPage GetTabPage(MyTabPageMode mode)
        {
            return _tabPageDic.TryGetValue(mode, out MyTabPage value) ? value : null;
        }

        public void MoveTabPagePos(int move)
        {
            if (SelectedTabPage.ShownInMenu)
            {
                SelectedTabPage = _fileTabPages.SlideItemAt(SelectedTabPage, move);
            }
        }

        public void CloseTabPage(MyTabPage p)
        {
            bool closeMyself = SelectedTabPage == p;
            int curPos = _fileTabPages.IndexOf(p);

            if (p.ShownInMenu)
            {
                _fileTabPages.Remove(p);
            }
            else
            {
                _tabPageDic.Remove(p.PageMode);
            }

            if (_latestTabPage == p)
            {
                _latestTabPage = null;
            }

            if (closeMyself)
            {
                if (p.ShownInMenu)
                {
                    SelectedTabPage = _fileTabPages.SlideItemAt(curPos, 0);
                }
                else
                {
                    SelectedTabPage = _latestTabPage ?? _fileTabPages.FirstOrDefault();
                }
            }
        }

        public void CloseTabPage(MyTabPageMode mode)
        {
            var page = GetTabPage(mode);
            if (page is not null)
            {
                CloseTabPage(page);
            }
        }

        public void ClearSyncMove()
        {
            foreach (var p in _fileTabPages)
            {
                p.SyncMove = false;
            }
        }

        private void RestoreLatestTabPage()
        {
            if (_latestTabPage is not null)
            {
                SelectedTabPage = _latestTabPage;
            }
        }

        public void ToggleTabPage(MyTabPageMode mode, FilePosCursor c)
        {
            if (SelectedTabPage?.PageMode == mode)
            {
                RestoreLatestTabPage();
            }
            else
            {
                AddTabPageAndSelect(c, mode);
            }
        }

        public FileManager GetFromCache(string dir)
        {
            return AllTabPages.Select(p => p.FileCursor.GetMyManager())
                .OfType<FileManager>()
                .FirstOrDefault(fman => fman.IsSame(dir));
        }

        public void ChangeTabPageCursor(MyTabPage p, FilePosCursor c, SideListType type, SideBarOpenReason reason)
        {
            p.SetMainFileCursorAndResetMode(c, type);
            if (p == SelectedTabPage)
            {
                UpdateCurrentTabPageUI(reason);
            }
        }

        public void ChangeTabPageCursorAndSelect(MyTabPage p, FilePosCursor c)
        {
            p.SetMainFileCursor(c);
            SetSelectedTabPageAndUpdateUI(p);
        }

        public void ShowComparePage(FilePosCursor cursor)
        {
            AddTabPageAndSelect(cursor, MyTabPageMode.Compare);
        }

        public void SubmitActionResult(IFileActionResult result, SideBarUpdateReason lct)
        {
            SubmitActionResult(result, new SideBarUpdateInfo(lct));
        }
        public void SubmitActionResult(IFileActionResult result, SideBarUpdateInfo lui)
        {
            var updater = result.CreateUpdater();
            if (updater is null)
            {
                return;
            }

            foreach (var p in AllTabPages)
            {
                updater.Update(p.FileCursor);
                updater.Update(p.OrigCursor);
            }
            if (PinnedCursor is not null)
            {
                updater.Update(PinnedCursor);
            }

            if (lui.UpdateReason != SideBarUpdateReason.Reload)
            {
                UpdateFileListUI(lui);
            }
        }
        #endregion

        #region Stored Cursor
        public bool ResetStoredCursor()
        {
            if (_pinnedCursor is not null)
            {
                PinnedCursor = null;
                return true;
            }
            return false;
        }

        public bool ToggleStoredCursorTabPage()
        {
            if (_pinnedCursor is not null)
            {
                ToggleTabPage(MyTabPageMode.Pinned, _pinnedCursor);
                return true;
            }
            return false;
        }
        #endregion

        #region Stored Cursor: Pinned
        private FilePosCursor _pinnedCursor = null;

        public FilePosCursor PinnedCursor
        {
            get { return _pinnedCursor; }
            set
            {
                if (_pinnedCursor is not null)
                {
                    _pinnedCursor.SelectionChanged -= Pinned_SelectionChanged;
                }

                var c = value;
                if (c is not null)
                {
                    _pinnedCursor = c.CloneSelectedBound();
                    _pinnedCursor.SelectionChanged += Pinned_SelectionChanged;
                }
                else
                {
                    _pinnedCursor = null;
                    ClearSyncMove();
                    CloseTabPage(MyTabPageMode.Pinned);
                }
                UpdateStoredCursorUI();
            }
        }

        public bool HasPinnedTarget()
        {
            return _pinnedCursor?.FileList.Any ?? false;
        }

        public void MovePinnedCursor(int move)
        {
            _pinnedCursor?.MovePos(move);
        }

        private void Pinned_SelectionChanged(FilePosCursor cursor)
        {
            UpdateStoredCursorUI();
        }
        #endregion

        #region Status Message
        public string GetStatusMessage()
        {
            if (SelectedTabPage is null)
            {
                return "";
            }

            var items = SelectedTabPage.FileCursor.SelectedItems;
            if (items.HasMany())
            {
                return $"選択 ({items.Count}個)";
            }
            if (SelectedTabPage.PageMode == MyTabPageMode.Pinned)
            {
                return "ピン止め確認中";
            }
            if (SelectedTabPage.SubPageAdd)
            {
                return "ファイル追加中";
            }
            if (SelectedTabPage.SyncMove)
            {
                return "同期移動中";
            }
            if (SelectedTabPage.FileCursor.FileList.Bound is not null)
            {
                return "範囲制限中";
            }
            return "";
        }
        #endregion

        #region Folder
        private string _targetDir;

        public string GetInitialDirectory()
        {
            if (SelectedTabPage?.FileCursor.GetMyManager() is IRealFolder real)
            {
                return real.FilePath;
            }
            return _targetDir;
        }

        public bool OpenFileLoader(IFileLoader loader, MyTabPage tabPage = null)
        {
            if (loader is null)
            {
                return false;
            }

            FileManager fman = loader.CreateManager(this);
            _targetDir = Path.GetDirectoryName(fman.FilePath);
            var result = new FileActionReloader();
            var cursor = loader.CreateCursor(fman, result);
            SubmitActionResult(result, SideBarUpdateReason.Reload);

            tabPage ??= SelectedTabPage;
            if (tabPage?.CanChgCursor ?? false)
            {
                ChangeTabPageCursorAndSelect(tabPage, cursor);
            }
            else
            {
                AddTabPageAndSelect(cursor);
            }
            return true;
        }
        #endregion

        #region Side Bar
        private SideBarUserInstruction _sideBarUserInstruction = SideBarUserInstruction.Closed;

        public SideBarUserInstruction SideBarUserInstruction
        {
            get
            {
                if (IsMandatorySideOpen())
                {
                    return _sideBarUserInstruction;
                }
                if (IsSideOpenForSelectedTabPage())
                {
                    return SideBarUserInstruction.Opened;
                }
                return _sideBarUserInstruction;
            }
            set
            {
                if (IsMandatorySideOpen())
                {
                    return;
                }

                if (IsSideOpenForSelectedTabPage())
                {
                    if (value == SideBarUserInstruction.Closed)
                    {
                        SelectedTabPage.SideListType = SideListType.All;
                    }
                }
                _sideBarUserInstruction = value;

                UpdateSideBarUI(SideBarOpenReason.SideBarUserInstruction);
            }
        }

        public void SetKeepOpen()
        {
            _sideBarUserInstruction = SideBarUserInstruction.Opened;
        }

        public void ToggleSideBarUserInstruction()
        {
            SideBarUserInstruction = SideBarUserInstruction == SideBarUserInstruction.Opened ? SideBarUserInstruction.Closed : SideBarUserInstruction.Opened;
        }

        private SideBarType _sideBarType = SideBarType.List;

        public SideBarType SideBarType
        {
            get
            {
                if (_sideBarType == SideBarType.List)
                {
                    if (SelectedTabPage?.SubPageAdd ?? false)
                    {
                        return SideBarType.FileAdd;
                    }
                    else if (SelectedTabPage?.SubPageEdit ?? false)
                    {
                        return SideBarType.FileEdit;
                    }
                }
                return _sideBarType;
            }
            set
            {
                bool changed = _sideBarType != value;
                _sideBarType = value;
                if (changed)
                {
                    UpdateSideBarUI(SideBarOpenReason.SideBarType);
                }
            }
        }

        public void ToggleSideBarType(SideBarType type)
        {
            SideBarType = _sideBarType == type ? SideBarType.List : type;
        }

        public bool IsMandatorySideOpen()
        {
            return SideBarType != SideBarType.List;
        }

        public bool IsSideOpenForSelectedTabPage()
        {
            switch (SelectedTabPage?.PageMode)
            {
                case MyTabPageMode.File:
                    if (SelectedTabPage.SideListType.IsSideOpen())
                    {
                        return true;
                    }
                    break;
                case MyTabPageMode.Pinned:
                    var target = LatestTabPage;
                    if (target.SideListType.IsSideOpen())
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public bool IsSideOpen()
        {
            bool sideOpen = IsMandatorySideOpen() || IsSideOpenForSelectedTabPage();
            switch (_sideBarUserInstruction)
            {
                case SideBarUserInstruction.Opened:
                    sideOpen = true;
                    break;
            }

            if (SelectedTabPage is null)
            {
                sideOpen = false;
            }
            return sideOpen;
        }
        #endregion

        #region SubPage
        public void ShowSubPage(FilePosCursor cursor)
        {
            if (cursor is not null)
            {
                SelectedTabPage.SetSubFileCursor(cursor);
                UpdateCurrentItemUI();
                UpdateFileListUI(SideBarUpdateReason.Reload);
            }
        }

        public void CommitSubPageAction(IFileActionResult result)
        {
            SubmitActionResult(result, SideBarUpdateReason.Reload);
            SelectedTabPage.SubPageMode = MySubPageMode.None;
        }
        #endregion
    }
}
