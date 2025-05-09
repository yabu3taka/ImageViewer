using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ImageViewer.Manager;

namespace ImageViewer.UI
{
    partial class SideListControl : UserControl, ISideBarUI
    {
        public SideListControl()
        {
            InitializeComponent();
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

        public bool ListBoxFocused
        {
            get { return LVFile.Focused; }
        }

        public void UpdateOnOpening(SideBarOpenReason reason)
        {
            UpdateGroupBoxUI();
            SetupFilterComboBox();
            NotifySettingChanged();

            if (SelectedTabPage?.SideListType.IsListForcus() ?? false)
            {
                LVFile.Select();
            }
        }

        public void UpdateFileList(SideBarUpdateInfo info)
        {
            if (SelectedTabPage?.SideListType.DirFlg ?? false)
            {
                if (info.OpenReason == SideBarOpenReason.TabPageCursorReplace)
                {
                    return;
                }
            }

            switch (info.UpdateReason)
            {
                case SideBarUpdateReason.MarkSave:
                    UpdateGroupBoxUI();
                    break;
                case SideBarUpdateReason.MarkReset:
                case SideBarUpdateReason.MarkChange:
                    LVFile.UpdateMarkStatus(true);
                    UpdateGroupBoxUI();
                    break;
                default:
                    LVFile.UpdateFileList(SelectedTabPage?.SideList);
                    break;
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
                LBInfoDir.Text = "";
                LBInfoFile.Text = "";
                TTInfoDir.SetToolTip(LBInfoFile, "");
                return;
            }

            LBInfoDir.Text = $"{c.GetMyManager().Title} ({c.FileList.AllCount})";
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

                if (finfo is IFolderInfoController con)
                {
                    SetupFolderInfo(con.GetFolderInfo());
                }
            }
        }

        public void NotifySettingChanged()
        {
            if (SelectedTabPage?.FileCursor.GetMyManager() is IFolderInfoController con)
            {
                SetupFolderInfo(con.GetFolderInfo());
            }
            else
            {
                SetupFolderInfo(null);
            }
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
                TTInfoDir.SetToolTip(LBInfoDir, "");
                LBInfoDir.Enabled = false;

                CMenuOpenInfo.Items.Clear();
            }
        }

        private IFolderInfo GetFolderInfo()
        {
            var c = SelectedTabPage?.FileCursor;
            if (c?.GetMyManager() is IFolderInfoController con1)
            {
                return con1.GetFolderInfo();
            }
            if (c?.Current is IFolderInfoController con2)
            {
                return con2.GetFolderInfo();
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

        #region Side Status
        private void UpdateGroupBoxUI()
        {
            var tabPage = SelectedTabPage;
            if (tabPage is null)
            {
                SideGroupBox.Visible = false;
                return;
            }

            SideGroupBox.Visible = true;

            SideGroupBox.Text = "ファイル";
            if (tabPage.FileCursor.GetMyManager() is IFileIndexController icon)
            {
                SideGroupBox.Text += icon.IndexMarker.Modified ? "(要保存)" : "";
            }
            if (tabPage.FileCursor.GetMyManager() is FileManager sfcon)
            {
                SideGroupBox.Text += sfcon.HasSubAddFolder() ? "(追加中)" : "";
            }
        }
        #endregion

        #region SideListType
        private void SetupFilterComboBox()
        {
            var tabPage = SelectedTabPage;
            if (tabPage is null)
            {
                CBFilter.Items.Clear();
                return;
            }

            CBFilter.BeginUpdate();
            CBFilter.Items.Clear();

            foreach (var type in SideListType.Types)
            {
                if (tabPage.CanSideListType(type))
                {
                    CBFilter.Items.Add(type);
                }
            }
            CBFilter.SelectedItem = tabPage.SideListType;

            CBFilter.EndUpdate();
        }

        private void CBFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var tabPage = SelectedTabPage;
            var type = (SideListType)CBFilter.SelectedItem;
            if (type is not null && tabPage is not null)
            {
                tabPage.SetSideListTypeAndKeepOpen(type);
            }
        }
        #endregion

        #region File List
        private void LVFile_ItemActivate(object sender, EventArgs e)
        {
            MyMain.RunAction();
        }
        #endregion
    }
}
