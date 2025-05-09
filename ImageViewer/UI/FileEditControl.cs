using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.UI
{
    partial class FileEditControl : UserControl, ISideBarUI
    {
        public FileEditControl()
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
            var sfMan = GetSubFolderMan();
            UpdateUI(sfMan);
            NotifySettingChanged();
        }

        public void UpdateFileList(SideBarUpdateInfo info)
        {
            info.UpdateImageFileListView(LVFile, SelectedTabPage?.FileCursor);
        }

        public void UpdateFileSelection()
        {
            LVFile.UpdateFileSelection();
        }

        public void SetImageInfo(FilePosCursor c, IFileImageInfo imgInfo)
        {
        }

        public void NotifySettingChanged()
        {
            var sfMan = GetSubFolderMan();
            if (sfMan.HasFolder())
            {
                BTConv.SubMenu = null;
                BTConv.Text = "取消";
            }
            else
            {
                var cmd = FileConvertExe.CreateFromSetting();
                if (cmd is null)
                {
                    BTConv.SubMenu = null;
                    BTConv.Text = "変換";
                }
                else
                {
                    BTConv.SubMenu = CMenuConv;
                    BTConv.Text = "変換　";
                }
            }
        }
        #endregion

        #region UI
        private SubFolderManagerForFileEdit GetSubFolderMan()
        {
            return SubFolderManagerForFileEdit.Create(SelectedTabPage.FileCursor.GetMyManager());
        }

        private void UpdateUI(SubFolderManagerForFileEdit sfMan)
        {
            FilePosCursor c;
            if (sfMan.HasFolder())
            {
                var fman = sfMan.GetSubFileManager();
                c = FilePosCursor.CreateFirst(fman);
            }
            else
            {
                c = SelectedTabPage?.OrigCursor.CloneSelectedBound();
            }
            MyMain.TabManager.ShowSubPage(c);

            if (sfMan.HasFolder())
            {
                BTCommit.Enabled = true;
            }
            else
            {
                BTCommit.Enabled = false;
            }
            NotifySettingChanged();
        }
        #endregion

        #region Convert/Commit/Revert
        private void BTConv_Click(object sender, EventArgs e)
        {
            var sfMan = GetSubFolderMan();
            if (sfMan.HasFolder())
            {
                try
                {
                    sfMan.ClearAll();
                }
                catch
                {
                    FormUtil.ShowError(ParentForm, "削除に失敗しました。");
                }
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

        private void MenuConvExplorer_Click(object sender, EventArgs e)
        {
            RunConv(new FileConvertExplorer());
        }

        private void RunConv(IFileConvertCmd cmd)
        {
            var sfMan = GetSubFolderMan();
            sfMan.AddFileToEditFolder(SelectedTabPage?.FileCursor.FileList.Items);
            if (!cmd.Run(sfMan.EditFolder))
            {
                FormUtil.ShowError(ParentForm, "変換に失敗しました。");
            }
            UpdateUI(sfMan);
        }

        private void BTCommit_Click(object sender, EventArgs e)
        {
            var sfMan = GetSubFolderMan();

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
    }
}
