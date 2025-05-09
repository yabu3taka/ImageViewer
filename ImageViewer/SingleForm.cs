using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.UI;
using ImageViewer.Util;

namespace ImageViewer
{
    partial class SingleForm : Form, IMyTabPageManagerCallBack
    {
        private readonly ToolStripLabel StatusText;

        private List<string> _openFiles = null;
        public SingleForm(List<string> files)
        {
            InitializeComponent();

            StatusText = new ToolStripLabel();
            MenuStrip.Items.Add(StatusText);

            MouseWheel += this.PicBox_MouseWheel;
            PicBox.MouseWheel += this.PicBox_MouseWheel;
            SubPicBox.MouseWheel += this.SubPicBo_MouseWheel;

            _openFiles = files;

            _picBoxMouse = new MouseEventManager();
            _picBoxMouse.Rocker += PicBox_Rocker;
            _picBoxMouse.MouseClick += PicBox_MouseClick;
            _picBoxMouse.MouseDoubleClick += PicBox_MouseDoubleClick;

            TabManager = new MyTabPageManager(this, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        }

        private void SingleForm_Load(object sender, EventArgs e)
        {
            Location = new Point(0, 0);
            AdjustMode = false;

            var loader = FileLoaderUtil.Create(_openFiles);
            if (!TabManager.OpenFileLoader(loader))
            {
                UpdateCurrentTabPageUI(SideBarOpenReason.TabPageChange);
            }
            _openFiles = null;
        }

        #region TabPage
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MyTabPageManager TabManager { get; private set; }
        public MyTabPage SelectedTabPage => TabManager.SelectedTabPage;
        #endregion

        #region PicBox
        private void UpdateImageUI()
        {
            IFileImageInfo imgInfo = null;
            var tabPage = SelectedTabPage;

            if (tabPage is null)
            {
                PicBox.CurImg = null;
                Text = "";
            }
            else
            {
                var finfo = tabPage?.FileCursor?.Current;
                if (finfo is null)
                {
                    PicBox.CurImg = null;
                    Text = tabPage.WindowTitle;
                }
                else
                {
                    imgInfo = finfo.GetImageInfo();
                    if (imgInfo is not null)
                    {
                        Text = $"{tabPage.WindowTitle} {imgInfo.ImageSize.ToDimString()}";
                    }
                    else
                    {
                        Text = tabPage.WindowTitle;
                    }
                    PicBox.CurImg = imgInfo?.Bitmap;
                }
            }

            CurrentSideBarUI.SetImageInfo(tabPage?.FileCursor, imgInfo);

            NotifyPicBoxChange();
        }

        private int _ratio = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ImgRatio
        {
            get { return _ratio; }
            set
            {
                int nv = NumberUtil.Limit(value, -5, 5);
                if (_ratio != nv)
                {
                    _ratio = nv;

                    PicBox.ImgRatio = Math.Pow(1.3, _ratio);
                    NotifyPicBoxChange();
                }
            }
        }

        private void NotifyPicBoxChange()
        {
            if (!AdjustMode)
            {
                UpdateFormSize();
            }
        }
        #endregion

        #region SubPicBox
        private void UpdateSubPicBoxSize()
        {
            if (SubPicBox.Visible)
            {
                SubPicBox.Size = SubPicBox.Image.Size.LimitSize(MainPanel.Size.ScaleDown(3));
            }
        }

        private void UpdateSubPicBoxVisibility()
        {
            var pageMode = SelectedTabPage?.PageMode ?? MyTabPageMode.None;
            var visible = pageMode switch
            {
                MyTabPageMode.Pinned => false,
                _ => SubPicBox.Image is not null,
            };
            SubPicBox.Visible = visible;
        }

        private void SetSubImage(Image img)
        {
            SubPicBox.Image = img;
            UpdateSubPicBoxVisibility();
            UpdateSubPicBoxSize();
        }

        public void UpdateStoredCursorUI()
        {
            FilePosCursor c = TabManager.PinnedCursor;
            if (c is not null)
            {
                SetSubImage(c.Current.GetBitmap());
            }
            else
            {
                SetSubImage(null);
            }
            UpdateStatusMessageUI();
        }
        #endregion

        #region Side Bar
        private bool SideOpened => !MainSplitter.Panel1Collapsed;

        public void UpdateSideBarUI(SideBarOpenReason reason)
        {
            var sideBatType = TabManager.SideBarType;
            var sideOpen = TabManager.IsSideOpen();

            ChangeSideBarUI(sideBatType);

            if (reason == SideBarOpenReason.TabPageChange || reason == SideBarOpenReason.TabPageCursorReplace)
            {
                RestoreWindowSize();
            }

            bool oldSideOpend = SideOpened;
            MainSplitter.Panel1Collapsed = !sideOpen;

            if (sideOpen)
            {
                UpdateOnOpeningUI(reason);
            }

            if (oldSideOpend != sideOpen)
            {
                UpdateFormSizeLater();
            }

            if (SideOpened)
            {
                bool updateListFlg = reason != SideBarOpenReason.SideBarUserInstruction;
                if (!oldSideOpend || updateListFlg)
                {
                    var info = new SideBarUpdateInfo(SideBarUpdateReason.ShowSideBar)
                    {
                        OpenReason = reason
                    };
                    UpdateFileListUI(info);
                }
            }
        }

        private ISideBarUI CurrentSideBarUI
        {
            get
            {
                if (CtrlComparator.Visible)
                {
                    return CtrlComparator;
                }
                if (CtrlFileAdd.Visible)
                {
                    return CtrlFileAdd;
                }
                if (CtrlFileEdit.Visible)
                {
                    return CtrlFileEdit;
                }
                return CtrlSideList;
            }
        }

        private ISideBarUI GetSideBarUI(SideBarType type)
        {
            return type switch
            {
                SideBarType.Compare => CtrlComparator,
                SideBarType.FileAdd => CtrlFileAdd,
                SideBarType.FileEdit => CtrlFileEdit,
                _ => CtrlSideList,
            };
        }

        private void ChangeSideBarUI(SideBarType type)
        {
            var sideBarUI = GetSideBarUI(type);
            if (!sideBarUI.Visible)
            {
                CtrlSideList.Visible = false;
                CtrlComparator.Visible = false;
                CtrlFileAdd.Visible = false;
                CtrlFileEdit.Visible = false;
                sideBarUI.Visible = true;
            }
        }

        private void UnfocusSideBarUI()
        {
            ActiveControl = null;
        }

        private void UpdateOnOpeningUI(SideBarOpenReason reason)
        {
            if (SideOpened)
            {
                CurrentSideBarUI.UpdateOnOpening(reason);
            }
        }

        public void UpdateFileListUI(SideBarUpdateInfo info)
        {
            if (SideOpened)
            {
                CurrentSideBarUI.UpdateFileList(info);
            }
        }

        private void UpdateFileSelectionUI()
        {
            if (SideOpened)
            {
                CurrentSideBarUI.UpdateFileSelection();
            }
        }

        public bool IsSideListFocused()
        {
            return CtrlSideList.ListBoxFocused;
        }
        #endregion

        #region Window Size
        private void MainSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            UpdateFormSizeLater();
        }

        private bool AdjustMode
        {
            get { return PicBox.Adjust; }
            set
            {
                PicBox.Adjust = value;
                if (value)
                {
                    MaximizeBox = true;
                    FormBorderStyle = FormBorderStyle.Sizable;
                }
                else
                {
                    MaximizeBox = false;
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                }
                UpdateFormMaximumSize();
                UpdateFormSizeLater();
            }
        }

        private void UpdateFormMaximumSize()
        {
            if (AdjustMode)
            {
                MaximumSize = new Size(0, 0);
            }
            else
            {
                var screenSize = Screen.GetWorkingArea(this).Size;
                MaximumSize = new Size(screenSize.Width - DesktopLocation.X, screenSize.Height - DesktopLocation.Y);
            }
        }

        private void MainPanel_Resize(object sender, EventArgs e)
        {
            UpdateSubPicBoxSize();
        }

        private void SingleForm_ResizeEnd(object sender, EventArgs e)
        {
            UpdateFormMaximumSize();
            UpdateFormSize();
        }

        private bool _registFormSize = false;
        public void UpdateFormSize()
        {
            if (!AdjustMode)
            {
                Size newSize = this.Size - MainSplitter.Panel2.Size + PicBox.CurImgSize;
                if (newSize.Height > MaximumSize.Height)
                {
                    newSize.Width += SystemInformation.VerticalScrollBarWidth;
                }
                if (newSize.Width > MaximumSize.Width)
                {
                    newSize.Height += SystemInformation.HorizontalScrollBarHeight;
                }
                this.Size = newSize;
            }
            _registFormSize = false;
        }

        private void UpdateFormSizeLater()
        {
            if (!_registFormSize)
            {
                _registFormSize = true;
                BeginInvoke(new Action(() => UpdateFormSize()));
            }
        }

        private bool _resotreSize = false;
        public void SetWindowSizeTempolary(Size s)
        {
            if (!AdjustMode)
            {
                _resotreSize = true;
                AdjustMode = true;
                this.Size = s;
            }
        }

        private void RestoreWindowSize()
        {
            if (_resotreSize)
            {
                AdjustMode = false;
            }
            _resotreSize = false;
        }
        #endregion

        #region Update UI
        public void UpdateCurrentTabPageUI(SideBarOpenReason reason)
        {
            UpdateSurroundingUI(reason);
            UpdateImageUI();
            UpdateSubPicBoxVisibility();
            UpdateMenuUI();
        }

        public void UpdateCurrentItemUI()
        {
            UpdateImageUI();
            UpdateFileSelectionUI();
            UpdateStatusMessageUI();
        }

        public void UpdateSurroundingUI(SideBarOpenReason reason)
        {
            UpdateSideBarUI(reason);
            UpdateStatusMessageUI();
        }
        #endregion

        #region Mouse: DragDrop
        private void SingleForm_DragEnter(object sender, DragEventArgs e)
        {
            if (CurrentSideBarUI is ISideBarDragDrop dad)
            {
                dad.SideBar_DragEnter(sender, e);
            }
            else
            {
                e.Effect = DragDropEffects.None;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                    if (FileInfoUtil.ImageExt.IsAnySupported(files))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void SingleForm_DragDrop(object sender, DragEventArgs e)
        {
            if (CurrentSideBarUI is ISideBarDragDrop dad)
            {
                dad.SideBar_DragDrop(sender, e);
            }
            else
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                var fileList = FileInfoUtil.ImageExt.FilterSupportedFile(files);
                var loader = FileLoaderUtil.Create(fileList);
                TabManager.OpenFileLoader(loader);
                UnfocusSideBarUI();
            }
        }
        #endregion

        #region Mouse: PicBox
        private readonly MouseEventManager _picBoxMouse;

        private void PicBox_Rocker(object sender, int direction)
        {
            TabManager.MoveTabPagePos(direction);
        }

        private void MainPanel_Click(object sender, EventArgs e)
        {
            UnfocusSideBarUI();
        }

        private void PicBox_MouseClick(object sender, MouseEventArgs e)
        {
            PicBox.ClickForSelection(e);
        }

        private void PicBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UnfocusSideBarUI();
            RunAction();
        }

        private void PicBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (SelectedTabPage is null)
            {
                return;
            }

            HandledMouseEventArgs eventArgs = e as HandledMouseEventArgs;
            eventArgs.Handled = true;

            int num = FormUtil.GetWheelTick(e);
            if (num == 0)
            {
                return;
            }

            Keys key = ModifierKeys;
            if ((key & Keys.Control) == Keys.Control)
            {
                ImgRatio += num;
            }
            else if ((key & Keys.Shift) == Keys.Shift)
            {
                TabManager.MoveTabPagePos(-num);
            }
            else
            {
                SelectedTabPage.FileCursor.MovePos(-num);
            }
        }

        private void PicBox_MouseDown(object sender, MouseEventArgs e)
        {
            UnfocusSideBarUI();

            var dragManager = PicBox.CreateDragEventManager(e) ?? MouseDragScrollManager.Create(MainPanel);
            _picBoxMouse.MouseDown(sender, e, dragManager);
        }

        private void PicBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_picBoxMouse.NormalMove)
            {
                var newCursor = PicBox.IsOverLayerParts(e) ? Cursors.Hand : Cursors.Default;
                if (PicBox.Cursor != newCursor)
                {
                    PicBox.Cursor = newCursor;
                }
            }

            _picBoxMouse.MouseMove(sender, e);
        }

        private void PicBox_MouseUp(object sender, MouseEventArgs e)
        {
            _picBoxMouse.MouseUp(sender, e);
        }
        #endregion

        #region Mouse: SubPicBox
        private void SubPicBo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (TabManager.PinnedCursor is null)
            {
                return;
            }

            HandledMouseEventArgs eventArgs = e as HandledMouseEventArgs;
            eventArgs.Handled = true;

            int num = FormUtil.GetWheelTick(e);
            if (num == 0)
            {
                return;
            }

            Keys key = ModifierKeys;
            if (key == Keys.None)
            {
                TabManager.PinnedCursor.MovePos(-num);
            }
        }

        private void SubPicBox_DoubleClick(object sender, EventArgs e)
        {
            TabManager.ToggleStoredCursorTabPage();
        }
        #endregion

        #region Status Message
        public void SetProgressMessage(string mess)
        {
            if (string.IsNullOrEmpty(mess))
            {
                UpdateStatusMessageUI();
            }
            else
            {
                StatusText.Text = mess;
            }
        }

        public void UpdateStatusMessageUI()
        {
            StatusText.Text = TabManager.GetStatusMessage();
        }
        #endregion

        #region Action / Cancel / Result
        public bool RunAction()
        {
            var tabPage = SelectedTabPage;
            if (tabPage is null)
            {
                return false;
            }
            switch (tabPage.PageModeInAction)
            {
                case MyTabPageMode.Compare:
                    //OpenFileLoader(FileLoader.Create(tabPage.FileCursor.Current.FilePath));
                    break;

                case MyTabPageMode.FileAdd:
                    break;

                case MyTabPageMode.FileEdit:
                    break;

                case MyTabPageMode.File:
                    if (tabPage.SideListType.DirFlg)
                    {
                        tabPage.SideListType = SideListType.All;
                    }
                    else
                    {
                        UnfocusSideBarUI();
                    }
                    break;
            }
            return true;
        }

        public bool CancelAction()
        {
            var tabPage = SelectedTabPage;
            if (tabPage is null)
            {
                return false;
            }
            switch (tabPage.PageModeInAction)
            {
                case MyTabPageMode.Pinned:
                    TabManager.CloseTabPage(tabPage);
                    break;

                case MyTabPageMode.Compare:
                    TabManager.CloseTabPage(tabPage);
                    CtrlComparator.ClearResult();
                    break;

                case MyTabPageMode.FileAdd:
                case MyTabPageMode.FileEdit:
                    tabPage.SubPageMode = MySubPageMode.None;
                    break;

                case MyTabPageMode.File:
                    if (SideListType.Delete.Equals(tabPage.SideListType))
                    {
                        tabPage.SideListType = SideListType.All;
                    }
                    else
                    {
                        if (tabPage.FileCursor.SelectedItems.HasMany())
                        {
                            tabPage.FileCursor.ResetSelection();
                        }
                        else if (TabManager.ResetStoredCursor())
                        {
                        }
                    }
                    break;
            }
            return true;
        }
        #endregion

        #region Menu DropDown
        private void UpdateMenuUI()
        {
            if (SelectedTabPage is not null)
            {
                FormUtil.SetMenuItemAvailable(MenuManage, true);
                FormUtil.SetMenuItemAvailable(MenuIndex, true);
            }
            else
            {
                FormUtil.SetMenuItemAvailable(MenuManage, false);
                FormUtil.SetMenuItemAvailable(MenuIndex, false);
            }
        }

        private void MenuFile_DropDownOpening(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;
            bool naviOk = tabPage?.CanDirNavi ?? false;
            MenuDirNext.Enabled = naviOk;
            MenuDirBack.Enabled = naviOk;

            MenuText.Enabled = tabPage?.FileCursor.Current is IFileText;
            MenuExplorer.Enabled = tabPage?.GetExplorerFolder() is not null;
            MenuOpenInfo.Enabled = tabPage?.FileCursor.GetMyManager() is IFolderInfoController;
            MenuFolderSetting.Enabled = tabPage?.GetFolderSettingFac() is not null;

            MenuCompareMode.Enabled = tabPage?.CanCompare ?? false;
        }

        private void MenuTab_DropDownOpening(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;
            FormUtil.ClearMenuItems(MenuTab.DropDownItems, MenuTabDeli);

            foreach (var p in TabManager.FileTabPages)
            {
                var item = new ToolStripMenuItem(p.MenuTitle) { Checked = tabPage == p };
                item.Click += (s1, e1) => TabManager.SelectedTabPage = p;
                MenuTab.DropDownItems.Add(item);
            }

            MenuTabCreate.Enabled = tabPage?.CanPageClone ?? false;
            MenuTabClose.Enabled = tabPage is not null;

            MenuTabTarget.Enabled = tabPage?.CanPageTarget ?? false;
            MenuTabTarget.Checked = tabPage?.TargetedPage ?? false;

            MenuTabPin.Enabled = tabPage?.CanCursorPin ?? false;
            MenuTabPin.Checked = TabManager.PinnedCursor is not null;

            MenuTabSync.Enabled = tabPage?.CanSyncMove ?? false;
            MenuTabSync.Checked = tabPage?.SyncMove ?? false;
        }

        private void MenuManage_DropDownOpening(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;

            // ファイル名変更
            bool renameOk = tabPage?.CanRenameFile ?? false;
            MenuRenameSwapUp.Enabled = renameOk;
            MenuRenameSwapDown.Enabled = renameOk;
            MenuRenameSerial.Enabled = renameOk;
            MenuRename.Enabled = renameOk;

            // ファイル追加
            MenuAddFile.Enabled = tabPage?.CanAddFile ?? false;
            MenuAddFile.Checked = tabPage?.SubPageAdd ?? false;

            // ファイル編集
            MenuEditFile.Enabled = tabPage?.CanEditFile ?? false;
            MenuEditFile.Checked = tabPage?.SubPageEdit ?? false;

            // フォルダ追加
            MenuAddFolder.Enabled = tabPage?.CanAddDir ?? false;
            MenuRenameFolder.Enabled = tabPage?.CanRenameFolder ?? false;

            // ファイル削除
            bool deleteOk = tabPage?.CanDeleteItem ?? false;
            MenuDeleteMark.Enabled = deleteOk;
            MenuDeleteMark.Checked = tabPage?.FileCursor.CurrentDeleteMark ?? false;
            MenuDeleteNow.Enabled = deleteOk;

            bool deleteCon = tabPage?.CanDeleteControl ?? false;
            MenuDeleteCommit.Enabled = deleteCon;
            MenuDeleteReset.Enabled = deleteCon;

            // Cut&Paste
            MenuCutFile.Enabled = tabPage?.CanPasteItem ?? false;
            MenuPasteFile.Enabled = IsPastable();
        }

        private void MenuPrint_DropDownOpening(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;

            // ズーム
            MenuZoomAdjust.Checked = AdjustMode;
            MenuZoomIn.Enabled = !AdjustMode;
            MenuZoomOut.Enabled = !AdjustMode;
            MenuZoomReset.Enabled = !AdjustMode;

            // サイドバー
            MenuSideView.Checked = SideOpened;
            MenuSideView.Enabled = !TabManager.IsMandatorySideOpen();

            // フィルター
            MenuFilterIndex.Checked = SideListType.Index.Equals(tabPage?.SideListType);
            MenuFilterGroup.Checked = SideListType.Group.Equals(tabPage?.SideListType);
            MenuFilterDir.Checked = SideListType.DirAll.Equals(tabPage?.SideListType);

            MenuFilterIndex.Enabled = tabPage?.CanSideListType(SideListType.Index) ?? false;
            MenuFilterGroup.Enabled = tabPage?.CanSideListType(SideListType.Group) ?? false;
            MenuFilterDir.Enabled = tabPage?.CanSideListType(SideListType.DirAll) ?? false;

            // 範囲指定
            MenuAreaSet.Text = tabPage?.CanItemUnbound ?? false ? "範囲設定解除" : "範囲設定";
            MenuAreaSet.Enabled = (tabPage?.CanItemBoundArea ?? false) || (tabPage?.CanItemUnbound ?? false);
            MenuAreaSet.Checked = tabPage?.FileCursor.FileList.Bound is FileFromToBound;

            bool boundOk = tabPage?.CanItemBound ?? false;
            MenuAreaFile.Enabled = boundOk;
            MenuAreaFile.Checked = tabPage?.FileCursor.FileList.Bound is FileNameStyleBound;

            MenuAreaImgSize.Enabled = boundOk;
            MenuAreaImgSize.Checked = tabPage?.FileCursor.FileList.Bound is FileWidthHeightBound;
        }

        private void MenuIndex_DropDownOpening(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;

            // インデックス移動
            bool indexNaviOk = tabPage?.CanIndexNavi ?? false;
            MenuIndexBack.Enabled = indexNaviOk;
            MenuIndexNext.Enabled = indexNaviOk;

            bool indexBoundOk = tabPage?.CanIndexBound ?? false;
            MenuIndexIn.Enabled = indexBoundOk;

            // インデックス操作
            bool indexConOk = tabPage?.CanIndexControl ?? false;

            MenuIndexMark.Enabled = indexConOk;
            MenuIndexMark.Checked = tabPage?.FileCursor.Current?.GetIndexMark() ?? false;

            MenuIndexReset.Enabled = indexConOk;
            MenuIndexAuto.Enabled = indexConOk;
            MenuIndexSave.Enabled = indexConOk;
        }
        #endregion

        #region Key Action
        private void Common_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (CurrentSideBarUI.NornmalControlFocused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Escape:
                case Keys.Delete:

                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:

                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.PageDown:
                    e.IsInputKey = true;
                    break;
            }
        }

        private bool DoKeyLeftRight(KeyEventArgs e, int right)
        {
            if (e.Alt)
            {
                TabManager.MoveTabPagePos(right);
                return true;
            }
            else
            {
                if (!CurrentSideBarUI.ListBoxFocused)
                {
                    TabManager.MoveTabPagePos(right);
                    return true;
                }
            }
            return false;
        }

        private bool DoKeyUpDown(KeyEventArgs e, int down)
        {
            if (e.Alt)
            {
                SwapFileOrder(down);
                return false;
            }
            else
            {
                if (!CurrentSideBarUI.ListBoxFocused)
                {
                    if (e.Shift)
                    {
                        SelectedTabPage.FileCursor.CallInSelectionMode(c => c.MovePos(down));
                    }
                    else
                    {
                        SelectedTabPage.FileCursor.MovePos(down);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool DoKeyPageUpPageDown(KeyEventArgs e, int down)
        {
            if (e.Alt)
            {
                AddZoomRatio(-down);
            }
            else if (e.Shift)
            {
                if (SelectedTabPage.PageMode == MyTabPageMode.Compare)
                {
                    MoveByIndex(down);
                }
                else if (SelectedTabPage.SubPageAdd)
                {
                    MoveByIndex(down);
                }
                else
                {
                    SelectedTabPage.FileCursor.CallInSelectionMode(c => MoveByIndex(down));
                }
            }
            else
            {
                MoveByIndex(down);
            }
            return true;
        }

        private bool DoKeyHomeEnd(KeyEventArgs e, int end)
        {
            if (e.Shift)
            {
                SelectedTabPage.FileCursor.CallInSelectionMode(c => c.GoToStartOrEnd(end));
            }
            else
            {
                SelectedTabPage.FileCursor.GoToStartOrEnd(end);
            }
            return true;
        }

        private void Common_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentSideBarUI.NornmalControlFocused)
            {
                return;
            }

            if (SelectedTabPage is null)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = RunAction();
                    break;
                case Keys.Escape:
                    e.Handled = CancelAction();
                    break;
                case Keys.Delete:
                    MenuDeleteNow.PerformClick();
                    e.Handled = true;
                    break;

                case Keys.Left:
                    e.Handled = DoKeyLeftRight(e, -1);
                    break;
                case Keys.Right:
                    e.Handled = DoKeyLeftRight(e, 1);
                    break;

                case Keys.Up:
                    e.Handled = DoKeyUpDown(e, -1);
                    break;
                case Keys.Down:
                    e.Handled = DoKeyUpDown(e, 1);
                    break;

                case Keys.Home:
                    if (e.Alt)
                    {
                        MenuZoomReset.PerformClick();
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = DoKeyHomeEnd(e, -1);
                    }
                    break;
                case Keys.End:
                    e.Handled = DoKeyHomeEnd(e, 1);
                    break;

                case Keys.PageUp:
                    e.Handled = DoKeyPageUpPageDown(e, -1);
                    break;
                case Keys.PageDown:
                    e.Handled = DoKeyPageUpPageDown(e, 1);
                    break;
            }
        }

        private void Common_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CurrentSideBarUI.NornmalControlFocused)
            {
                return;
            }

            if (SelectedTabPage is null)
            {
                return;
            }

            switch (e.KeyChar)
            {
                case '<':
                case ',':
                    MenuDirBack.PerformClick();
                    break;
                case '>':
                case '.':
                    MenuDirNext.PerformClick();
                    break;

                case 'd':
                    MenuDeleteMark.PerformClick();
                    break;
                case 'i':
                    MenuIndexMark.PerformClick();
                    break;
            }
        }
        #endregion

        #region Menu: Tab
        private void MenuTabCreate_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanPageClone ?? false)
            {
                TabManager.AddTabPageAndSelect(SelectedTabPage.OrigCursor.Clone());
            }
        }

        private void MenuTabTarget_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanPageTarget ?? false)
            {
                SelectedTabPage.TargetedPage = !SelectedTabPage.TargetedPage;
            }
        }

        private void MenuTabClose_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage is not null)
            {
                TabManager.CloseTabPage(SelectedTabPage);
            }
        }

        private void MenuTabPin_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanCursorPin ?? false)
            {
                SelectedTabPage.TabPin();
            }
        }

        private void MenuTabPinBack_Click(object sender, EventArgs e)
        {
            TabManager.ToggleStoredCursorTabPage();
        }

        private void MenuTabSync_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanSyncMove ?? false)
            {
                SelectedTabPage.SyncMove = !SelectedTabPage.SyncMove;
            }
        }
        #endregion

        #region Menu: Open Folder
        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            ImageFileDialog.InitialDirectory = TabManager.GetInitialDirectory();
            if (ImageFileDialog.ShowDialog() == DialogResult.OK)
            {
                var loader = FileLoaderUtil.Create(new List<string>(ImageFileDialog.FileNames));
                TabManager.OpenFileLoader(loader);
            }
            UnfocusSideBarUI();
        }

        private void MenuDirOpen_Click(object sender, EventArgs e)
        {
            if (ImageDirDialog.ShowDialog() == DialogResult.OK)
            {
                var loader = FileLoaderUtil.Create(ImageDirDialog.SelectedPath);
                TabManager.OpenFileLoader(loader);
            }
            UnfocusSideBarUI();
        }

        private void GoNextFileManager(int move)
        {
            if (SelectedTabPage?.CanDirNavi ?? false)
            {
                var fman = SelectedTabPage.GetMainFileManager();
                var loader = FileLoaderUtil.CreateNext(fman.FilePath, move);
                SelectedTabPage.ReplaceFileManager(loader);
            }
        }

        private void MenuDirBack_Click(object sender, EventArgs e)
        {
            GoNextFileManager(-1);
        }

        private void MenuDirNext_Click(object sender, EventArgs e)
        {
            GoNextFileManager(1);
        }
        #endregion

        #region Menu: Zoom In/Out
        private void MenuFullShow_Click(object sender, EventArgs e)
        {
            MenuStrip.Visible = !MenuStrip.Visible;
            UpdateFormSizeLater();
        }

        private void MenuZoomAdjust_Click(object sender, EventArgs e)
        {
            AdjustMode = !AdjustMode;
        }

        private void AddZoomRatio(int notch)
        {
            if (SelectedTabPage is not null && !AdjustMode)
            {
                ImgRatio += notch;
            }
        }

        private void MenuZoomIn_Click(object sender, EventArgs e)
        {
            AddZoomRatio(1);
        }

        private void MenuZoomOut_Click(object sender, EventArgs e)
        {
            AddZoomRatio(-1);
        }

        private void MenuZoomReset_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage is not null && !AdjustMode)
            {
                ImgRatio = 0;
            }
        }
        #endregion

        #region Menu: Filter
        private void MenuSideView_Click(object sender, EventArgs e)
        {
            TabManager.ToggleSideBarUserInstruction();
        }

        private void MenuFilterIndex_Click(object sender, EventArgs e)
        {
            SelectedTabPage?.ToggleSideListType(SideListType.Index);
        }

        private void MenuFilterGroup_Click(object sender, EventArgs e)
        {
            SelectedTabPage?.ToggleSideListType(SideListType.Group);
        }

        private void MenuFilterDir_Click(object sender, EventArgs e)
        {
            SelectedTabPage?.ToggleSideListType(SideListType.DirAll);
        }
        #endregion

        #region Menu: Bound/Select
        private void MenuAreaSet_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanItemBound ?? false)
            {
                SelectedTabPage.AreaSet();
            }
        }

        private void MenuAreaFile_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanItemBound ?? false)
            {
                using var f = new BoundForm(SelectedTabPage.FileCursor);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    SelectedTabPage.AreaFile(f.ResultCondition);
                }
                UnfocusSideBarUI();
            }
        }

        private void MenuAreaImgSize_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanItemBound ?? false)
            {
                using var f = new SizeBoundForm(SelectedTabPage.FileCursor);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    SelectedTabPage.AreaImgSize(f.ResultCondition);
                }
                UnfocusSideBarUI();
            }
        }

        private void MenuSelectAll_Click(object sender, EventArgs e)
        {
            SelectedTabPage.SelectAll();
        }
        #endregion

        #region Menu: Setting / Folder Info
        private void MenuExplorer_Click(object sender, EventArgs e)
        {
            var targetFolder = SelectedTabPage?.GetExplorerFolder();
            if (targetFolder is not null)
            {
                FormUtil.OpenFile(targetFolder.FilePath);
            }
        }

        private void MenuOpenInfo_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.FileCursor.GetMyManager() is IFolderInfoController con)
            {
                con.GetFolderInfo()?.OpenDirInfo();
            }
        }

        private void MenuText_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.FileCursor.Current is IFileText ft)
            {
                using var f = new TextForm(ft.FileText);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    ft.FileText = f.PhotoText;
                }
                UnfocusSideBarUI();
            }
        }

        private void MenuFolderSetting_Click(object sender, EventArgs e)
        {
            IFolderSettingController folderSettingFac = SelectedTabPage?.GetFolderSettingFac();
            if (folderSettingFac is not null)
            {
                using var f = new SettingForm(folderSettingFac.GetFolderTextSetting());
                if (f.ShowDialog() == DialogResult.OK)
                {
                    CurrentSideBarUI.NotifySettingChanged();
                }
                UnfocusSideBarUI();
            }
        }
        #endregion

        #region Menu: Delete
        private void MenuDeleteMark_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanDeleteItem ?? false)
            {
                SelectedTabPage.DeleteMark();
            }
        }

        private void MenuDeleteNow_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanDeleteItem ?? false)
            {
                SelectedTabPage.DeleteNow();
                UnfocusSideBarUI();
            }
        }

        private bool ComfirmAndGetDeleteController(string mess)
        {
            if (SelectedTabPage?.CanDeleteControl ?? false)
            {
                if (FormUtil.Confirm(this, mess))
                {
                    return true;
                }
            }
            return false;
        }

        private void MenuDeleteReset_Click(object sender, EventArgs e)
        {
            if (ComfirmAndGetDeleteController("削除マークを取消しますか？"))
            {
                SelectedTabPage.DeleteReset();
            }
        }

        private void MenuDeleteCommit_Click(object sender, EventArgs e)
        {
            if (ComfirmAndGetDeleteController("削除マークが付いたファイルを削除しますか？"))
            {
                SelectedTabPage.DeleteCommit();
            }
        }
        #endregion

        #region Menu: Add/Rename Folder
        private void MenuAddFolder_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanAddDir ?? false)
            {
                using var f = new NewFolderForm(SelectedTabPage.GetGalleryFolder(), SelectedTabPage.GetFolderName());
                f.ShowDialog();
                if (f.ActionResult is not null)
                {
                    TabManager.SubmitActionResult(f.ActionResult, SideBarUpdateReason.FolderAdd);
                }
                UnfocusSideBarUI();
            }
        }

        private void MenuRenameFolder_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanRenameFolder ?? false)
            {
                using var f = new RenameForm(SelectedTabPage.GetDirectFolder());
                if (SelectedTabPage.FileCursor.GetMyManager() is SubFileManager sman)
                {
                    f.CheckFunc = (parentFinfo, t) =>
                    {
                        if (!SubFolderManagerForFileAdd.IsValidSubFolder(t.Text))
                        {
                            FormUtil.ShowValidationError(t, "aで始まるフォルダ名を指定してください。");
                            return false;
                        }
                        return true;
                    };
                }
                f.ShowDialog();
                if (f.ActionResult is not null)
                {
                    var lui = new SideBarUpdateInfo(SideBarUpdateReason.FolderRename)
                    {
                        NewName = f.NewName
                    };
                    TabManager.SubmitActionResult(f.ActionResult, lui);
                }
                UnfocusSideBarUI();
            }
        }
        #endregion

        #region Menu: Add/Edit/Rename File
        private void MenuAddFile_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanAddFile ?? false)
            {
                SelectedTabPage.ToggleSubPageMdode(MySubPageMode.FileAdd);
            }
        }

        private void MenuEditFile_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanEditFile ?? false)
            {
                SelectedTabPage.ToggleSubPageMdode(MySubPageMode.FileEdit);
            }
        }

        private void MenuRenameSerial_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanRenameFile ?? false)
            {
                using var f = new SerialForm(SelectedTabPage.FileCursor);
                f.ShowDialog(this);
                if (f.ActionResult is not null)
                {
                    TabManager.SubmitActionResult(f.ActionResult, SideBarUpdateReason.FileRename);
                }
                UnfocusSideBarUI();
            }
        }

        private void MenuRename_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanRenameFile ?? false)
            {
                if (SelectedTabPage.FileCursor.SelectedItems.HasMany())
                {
                    MenuRenameSerial.PerformClick();
                }
                else
                {
                    using var f = new RenameForm((IRealFile)SelectedTabPage.FileCursor.Current);
                    f.ShowDialog();
                    if (f.ActionResult is not null)
                    {
                        TabManager.SubmitActionResult(f.ActionResult, SideBarUpdateReason.FileRename);
                    }
                    UnfocusSideBarUI();
                }
            }
        }

        private void SwapFileOrder(int move)
        {
            if (SelectedTabPage?.CanRenameFile ?? false)
            {
                SelectedTabPage.SwapFile(move);
            }
        }
        private void MenuRenameSwapUp_Click(object sender, EventArgs e)
        {
            SwapFileOrder(-1);
        }
        private void MenuRenameSwapDown_Click(object sender, EventArgs e)
        {
            SwapFileOrder(1);
        }
        #endregion

        #region Menu: Index
        private void MenuIndexMark_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanIndexControl ?? false)
            {
                SelectedTabPage.IndexToggle();
            }
        }

        private void MoveByIndex(int move)
        {
            if (SelectedTabPage?.CanIndexNavi ?? false)
            {
                switch (SelectedTabPage?.PageModeInAction)
                {
                    case MyTabPageMode.Compare:
                        this.CtrlComparator.MoveResultGroup(move);
                        break;

                    case MyTabPageMode.FileAdd:
                        this.CtrlFileAdd.MoveSubFolderGroup(move);
                        break;

                    case MyTabPageMode.FileEdit:
                        break;

                    default:
                        SelectedTabPage.IndexMove(move);
                        break;
                }
            }
        }

        private void MenuIndexBack_Click(object sender, EventArgs e)
        {
            MoveByIndex(-1);
        }

        private void MenuIndexNext_Click(object sender, EventArgs e)
        {
            MoveByIndex(1);
        }

        private void MenuIndexIn_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanIndexBound ?? false)
            {
                SelectedTabPage.IndexIn();
            }
        }

        private bool ComfirmAndGetIndexController(string mess)
        {
            if (SelectedTabPage?.CanIndexControl ?? false)
            {
                var type = SelectedTabPage.SideListType;
                if (type.IsIndex())
                {
                    return true;
                }
                if (FormUtil.Confirm(this, mess))
                {
                    return true;
                }
            }
            return false;
        }

        private void MenuIndexReset_Click(object sender, EventArgs e)
        {
            if (ComfirmAndGetIndexController("インデックスをリセットしますか？"))
            {
                SelectedTabPage.IndexReset();
            }
        }

        private void MenuIndexAuto_Click(object sender, EventArgs e)
        {
            if (ComfirmAndGetIndexController("インデックスを自動生成しますか？"))
            {
                SelectedTabPage.IndexAuto();
            }
        }

        private void MenuIndexSave_Click(object sender, EventArgs e)
        {
            if (ComfirmAndGetIndexController("インデックスを保存しますか？"))
            {
                SelectedTabPage.IndexSave();
            }
        }
        #endregion

        #region Menu: Misc
        private void MenuPGSetting_Click(object sender, EventArgs e)
        {
            using var f = new PGSettingForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                CurrentSideBarUI.NotifySettingChanged();
            }
        }

        private void MenuGifPlay_Click(object sender, EventArgs e)
        {
            PicBox.StartGifAnimation();
        }

        private void MenuQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Image Compare
        private void MenuCompareMode_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanCompare ?? false)
            {
                TabManager.ToggleSideBarType(SideBarType.Compare);
            }
        }
        #endregion

        #region Copy&Paste
        private void MenuCutFile_Click(object sender, EventArgs e)
        {
            if (SelectedTabPage?.CanPasteItem ?? false)
            {
                List<string> list = [];
                foreach (var finfo in SelectedTabPage.GetExpandedItems())
                {
                    list.Add(finfo.FilePath);
                }

                string[] fileNames = list.ToArray();
                IDataObject data = new DataObject(DataFormats.FileDrop, fileNames);

                byte[] bs = [(byte)DragDropEffects.Move, 0, 0, 0];
                System.IO.MemoryStream ms = new(bs);
                data.SetData("Preferred DropEffect", ms);

                Clipboard.SetDataObject(data);
            }
        }

        private void MenuPasteFile_Click(object sender, EventArgs e)
        {
            if (IsPastable())
            {
                var targetFolder = SelectedTabPage?.GetExplorerFolder();
                if (targetFolder is not null)
                {
                    var result = PasteFiles(targetFolder);
                    TabManager.SubmitActionResult(result, SideBarUpdateReason.FileAdd);
                }
            }
        }

        private bool IsPastable()
        {
            var targetFolder = SelectedTabPage?.GetExplorerFolder();
            if (targetFolder is null)
            {
                return false;
            }

            IDataObject data = Clipboard.GetDataObject();
            if (data is null)
            {
                return false;
            }
            if (!data.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            List<string> files = GetSupportedFile(data);
            if (!files.HasAny())
            {
                return false;
            }
            return true;
        }

        private static List<string> GetSupportedFile(IDataObject data)
        {
            string[] files = (string[])data.GetData(DataFormats.FileDrop);
            return FileInfoUtil.ImageExt.FilterSupportedFile(files);
        }

        private static FileActionResult PasteFiles(IRealFolder destDir)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data != null && data.GetDataPresent(DataFormats.FileDrop))
            {
                DragDropEffects dde = GetPreferredDropEffect(data);
                bool copyFlg = dde == (DragDropEffects.Copy | DragDropEffects.Link);

                List<string> files = GetSupportedFile(data);

                var trans = new FileActionTransaction();
                foreach (string sourcePath in files)
                {
                    if (copyFlg)
                    {
                        trans.Add(new FileCopyAction(new RealFileSimple(sourcePath), destDir));
                    }
                    else
                    {
                        trans.Add(new FileRenameAction(new RealFileSimple(sourcePath), destDir));
                    }
                }
                if (trans.Commitable)
                {
                    return trans.Commit();
                }
            }
            return null;
        }

        private static DragDropEffects GetPreferredDropEffect(IDataObject data)
        {
            if (data != null)
            {
                var ms = (System.IO.MemoryStream)data.GetData("Preferred DropEffect");
                if (ms != null)
                {
                    return (DragDropEffects)ms.ReadByte();
                }
            }
            return DragDropEffects.None;
        }
        #endregion
    }
}
