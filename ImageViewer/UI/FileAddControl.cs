using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.UI
{
    partial class FileAddControl : UserControl, ISideBarUI, ISideBarDragDrop
    {
        public FileAddControl()
        {
            InitializeComponent();

            LVFile.BgColorFunc = BgColor;
        }

        private SingleForm MyMain => (SingleForm)ParentForm;
        private MyTabPage SelectedTabPage => MyMain?.SelectedTabPage;

        #region ISideBarListUI
        public bool NornmalControlFocused
        {
            get
            {
                if (ActiveControl is null)
                {
                    return false;
                }
                return ActiveControl != LVFile;
            }
        }

        public bool ListBoxFocused => LVFile.Focused;

        public void UpdateOnOpening(SideBarOpenReason reason)
        {
            SubFolderManagerForFileAdd sfMan = GetSubFolderMan();
            var list = sfMan.GetSubFolderList();
            UpdateSubFolderList(sfMan, list, -1);

            UpdateUI(sfMan);
            UpdateSizeCountUI(sfMan);
            NotifySettingChanged();

            var screenSize = Screen.GetWorkingArea(ParentForm).Size;
            MyMain.SetWindowSizeTempolary(new Size(400, screenSize.Height * 2 / 3));
        }

        public void UpdateFileList(SideBarUpdateInfo info)
        {
            LVFile.UpdateFileList(SelectedTabPage?.FileCursor);
            if (info.IsFileCountChanged())
            {
                var sfMan = GetSubFolderMan();
                UpdateUI(sfMan);
                UpdateSizeCountUI(sfMan);
                UpdateSubFolderList("");
            }
            else if (info.UpdateReason == SideBarUpdateReason.FolderRename)
            {
                UpdateSubFolderList(info.NewName);
            }
            else if (info.UpdateReason == SideBarUpdateReason.FileRename)
            {
                UpdateSubFolderList("");
            }
            else if (info.IsMarkChanged())
            {
                UpdateMarkStatusUI();
            }
        }

        public void UpdateFileSelection()
        {
            LVFile.UpdateFileSelection();
        }

        public void SetImageInfo(FilePosCursor c, IFileImageInfo imgInfo)
        {
            if (c is null)
            {
                LBInfoFile.Text = "";
                TTInfoDir.SetToolTip(LBInfoFile, "");
                return;
            }

            var finfo = c.Current;
            if (finfo is null)
            {
                LBInfoFile.Text = "";
                TTInfoDir.SetToolTip(LBInfoFile, "");
            }
            else
            {
                LBInfoFile.Text = Path.GetFileName(finfo.FilePath);
                TTInfoDir.SetToolTip(LBInfoFile, imgInfo?.GetToolTip() ?? "");
            }
        }

        public void NotifySettingChanged()
        {
            SetupFolderInfo(GetFolderInfo());
        }
        #endregion

        #region UI
        private SubFolderManagerForFileAdd GetSubFolderMan()
        {
            return SubFolderManagerForFileAdd.Create(SelectedTabPage.FileCursor.GetMyManager());
        }

        private void UpdateMarkStatusUI()
        {
            LVFile.UpdateMarkStatus();
        }

        private void UpdateUI(SubFolderManagerForFileAdd sfMan)
        {
            BTMerge.Enabled = true;

            if (sfMan.Locked)
            {
                BTMerge.SubMenu = null;
                BTMerge.Text = "取消";
                BTCommit.Enabled = true;
            }
            else
            {
                BTCommit.Enabled = false;
                if (sfMan.GetFileCount() <= 0)
                {
                    BTMerge.Enabled = false;
                }

                var cmd = FileConvertExe.CreateFromSetting();
                if (cmd is null)
                {
                    BTMerge.SubMenu = null;
                    BTMerge.Text = "マージ";
                }
                else
                {
                    BTMerge.SubMenu = CMenuConv;
                    BTMerge.Text = "マージ　";
                }
            }
        }

        private void UpdateSizeCountUI(SubFolderManagerForFileAdd sfMan)
        {
            LBInfoDir.Text = $"{sfMan.ParentManager.Title} ({sfMan.GetFileCount()})";
        }
        #endregion

        #region SubFolder
        private void UpdateSubFolderList(SubFolderManagerForFileAdd sfMan, List<SubFolderInfo> list, int index = 0)
        {
            CBFolder.BeginUpdate();
            CBFolder.Items.Clear();

            foreach (var subfolder in list)
            {
                CBFolder.Items.Add(subfolder);
            }

            if (list.Count > 0)
            {
                if (index >= 0)
                {
                    CBFolder.SelectedIndex = index;
                }
                else
                {
                    CBFolder.SelectedIndex = CBFolder.Items.Count - 1;
                }
            }
            else
            {
                ShowSubFileList(sfMan);
            }

            CBFolder.EndUpdate();
        }

        private void AddNew(SubFolderManagerForFileAdd sfMan)
        {
            var list = sfMan.AddNewSubFolder();
            UpdateSubFolderList(sfMan, list, -1);
        }

        private void UpdateSubFolderList(string targetId)
        {
            var sfMan = GetSubFolderMan();
            var list = sfMan.GetSubFolderList();

            if (string.IsNullOrEmpty(targetId))
            {
                if (CBFolder.SelectedIndex >= 0)
                {
                    targetId = ((SubFolderInfo)CBFolder.SelectedItem).Id;
                }
            }

            int pos = list.FindIndex(subfolder => subfolder.Id == targetId);
            if (pos < 0)
            {
                pos = 0;
            }

            UpdateSubFolderList(sfMan, list, pos);
        }

        private void BTAdd_Click(object sender, EventArgs e)
        {
            AddNew(GetSubFolderMan());
        }

        private void TBDel_Click(object sender, EventArgs e)
        {
            var sfMan = GetSubFolderMan();
            var fman = GetSubFileManager(sfMan);
            if (fman is null)
            {
                return;
            }

            string prefix = "";
            if (fman.GetFirst() is not null)
            {
                prefix = "ファイルが存在しますが、";
            }

            if (FormUtil.Confirm(ParentForm, $"{prefix}削除しますか？"))
            {
                var list = sfMan.DeleteSubFolder(fman);
                UpdateSubFolderList(sfMan, list, CBFolder.SelectedIndex - 1);
                UpdateSizeCountUI(sfMan);
            }
        }

        private SubFileManagerForFileAdd GetSubFileManager(SubFolderManagerForFileAdd sfMan)
        {
            if (CBFolder.SelectedIndex < 0)
            {
                return null;
            }
            return sfMan.GetSubFileManager((SubFolderInfo)CBFolder.SelectedItem);
        }

        private void CBFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                MoveSubFolderGroup(-1);
            }
            if (e.KeyCode == Keys.PageDown)
            {
                e.Handled = true;
                MoveSubFolderGroup(1);
            }
        }

        private void CBFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSubFileList(GetSubFolderMan());
        }

        public void MoveSubFolderGroup(int i)
        {
            int newPos = CBFolder.SelectedIndex + i;
            if (newPos < 0)
            {
                return;
            }
            if (newPos >= CBFolder.Items.Count)
            {
                return;
            }
            CBFolder.SelectedIndex = newPos;
        }
        #endregion

        #region File List
        private IFileMarkerAccessor _errMarker = null;
        private readonly IFileMarkerName _ddErrMarker = new FileMarkerNameWithoutExtension();

        private Color BgColor(IFileInfo finfo)
        {
            if (_errMarker?[finfo] ?? false)
            {
                return Color.Pink;
            }
            //else if (_ddErrMarker?[finfo] ?? false)
            //{
            //    return Color.Red;
            //}
            else
            {
                return LVFile.BackColor;
            }
        }

        private void ClearDDError()
        {
            _ddErrMarker.Clear();
            LVFile.BackColor = Color.White;
        }

        private void AddDDError(string file)
        {
            _ddErrMarker.SetName(file, true);
            LVFile.BackColor = Color.Red;
        }

        private void ShowSubFileList(SubFolderManagerForFileAdd sfMan)
        {
            var fman = GetSubFileManager(sfMan);
            FilePosCursor c = null;
            if (fman is null)
            {
                c = FilePosCursor.CreateFirst(sfMan.GetEmptyManager());
            }
            else
            {
                bool noChange = false;
                if (SelectedTabPage.FileCursor.GetMyManager() is SubFileManagerForFileAdd cur)
                {
                    if (FileUtil.EqualsPath(cur.FilePath, fman.FilePath))
                    {
                        noChange = true;
                    }
                }
                if (!noChange)
                {
                    c = FilePosCursor.CreateFirst(fman);
                }
            }
            MyMain.TabManager.ShowSubPage(c);

            ClearDDError();
        }
        #endregion

        #region ISideBarDragDrop
        private IRealFolder GetSubFolderForAdd(SubFolderManagerForFileAdd sfMan)
        {
            return sfMan.GetSubFolder((SubFolderInfo)CBFolder.SelectedItem);
        }

        public void SideBar_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            var sfMan = GetSubFolderMan();
            if (sfMan.Locked)
            {
                return;
            }

            var fman = GetSubFileManager(sfMan);
            if (fman is null)
            {
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                if (FileInfoUtil.ImageExt.IsAnySupported(files))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        public void SideBar_DragDrop(object sender, DragEventArgs e)
        {
            var sfMan = GetSubFolderMan();

            ClearDDError();

            var folder = GetSubFolderForAdd(sfMan);
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            var fileList = FileInfoUtil.ImageExt.FilterSupportedFile(files);
            string lastFilepath = "";
            foreach (string file in fileList)
            {
                string filepath = folder.Combine(Path.GetFileName(file));
                if (File.Exists(filepath))
                {
                    AddDDError(filepath);
                }
                else
                {
                    File.Copy(file, filepath);
                    lastFilepath = filepath;
                }
            }
            if (!string.IsNullOrEmpty(lastFilepath))
            {
                NotifyAddFile(sfMan, lastFilepath);
            }
            else
            {
                UpdateMarkStatusUI();
            }
        }

        private void NotifyAddFile(SubFolderManagerForFileAdd sfMan, string filename)
        {
            var c = SelectedTabPage.FileCursor;
            c.ReloadDeeply();

            if (!string.IsNullOrEmpty(filename))
            {
                c.MoveTo(filename);
            }

            UpdateFileList(new SideBarUpdateInfo(SideBarUpdateReason.FileAdd));
        }
        #endregion

        #region Merge/Commit/Revert
        private void BTMerge_Click(object sender, EventArgs e)
        {
            var sfMan = GetSubFolderMan();
            if (sfMan.Locked)
            {
                sfMan.Unlock();
                UpdateUI(sfMan);
            }
            else
            {
                IFileConvertCmd cmd = FileConvertExe.CreateFromSetting();
                cmd ??= new FileConvertExplorer();
                RunConv(cmd);
            }
        }

        private void MenuConvCmd_Click(object sender, EventArgs e)
        {
            var cmd = FileConvertExe.CreateFromSetting();
            if (cmd is null)
            {
                return;
            }
            RunConv(cmd);
        }

        private void MenuConvCmdWidth_Click(object sender, EventArgs e)
        {
            var cmd = FileConvertExe.CreateFromSettingForWidth();
            if (cmd is null)
            {
                return;
            }
            RunConv(cmd);
        }

        private void MenuConvExplorer_Click(object sender, EventArgs e)
        {
            RunConv(new FileConvertExplorer());
        }

        private void RunConv(IFileConvertCmd cmd)
        {
            var sfMan = GetSubFolderMan();
            _errMarker = sfMan.CreateMargedFolder();
            if (_errMarker is not null)
            {
                UpdateMarkStatusUI();
                FormUtil.ShowError(ParentForm, "重複するファイルがあります。");
                return;
            }

            if (!cmd.Run(sfMan.MergeFolder))
            {
                FormUtil.ShowError(ParentForm, "変換に失敗しました。");
            }
            SelectedTabPage?.FileCursor.NotifyBitmapChange();

            UpdateUI(sfMan);
        }

        private void BTCommit_Click(object sender, EventArgs e)
        {
            var sfMan = GetSubFolderMan();

            if (!sfMan.Commitable)
            {
                sfMan.Unlock();
                FormUtil.ShowError(ParentForm, "マージフォルダーの更新が必要です。");
                UpdateUI(sfMan);
                return;
            }

            var result = sfMan.Commit();
            if (result is null)
            {
                FormUtil.ShowError(ParentForm, "ファイル移動に失敗しました。");
                return;
            }

            sfMan.ClearAll();
            FormUtil.ShowInfo(ParentForm, "ファイルを移動しました。");

            MyMain.TabManager.CommitSubPageAction(result);
        }
        #endregion

        #region Folder Info
        private void SetupFolderInfo(IFolderInfo folderInfo)
        {
            if (folderInfo is not null)
            {
                TTInfoDir.SetToolTip(LBInfoDir, folderInfo.ToolTip);
                LBInfoDir.Enabled = true;

                int pos = 0;
                CMenuOpenInfo.Items.Clear();
                foreach (string title in folderInfo.TitleList)
                {
                    var item = new ToolStripMenuItem(title);
                    int cur = pos;
                    item.Click += (s1, e1) => folderInfo.OpenDirInfo(cur);
                    CMenuOpenInfo.Items.Add(item);
                    pos++;
                }
            }
            else
            {
                TTInfoDir.RemoveAll();
                LBInfoDir.Enabled = false;

                CMenuOpenInfo.Items.Clear();
            }
        }

        private IFolderInfo GetFolderInfo()
        {
            if (SelectedTabPage?.OrigCursor?.FileList?.MyManager is IFolderInfoController con1)
            {
                return con1.GetFolderInfo();
            }
            return null;
        }

        private void LBInfoDir_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GetFolderInfo()?.OpenDirInfo();
            }
        }
        #endregion
    }
}
