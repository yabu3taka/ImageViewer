using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ImageViewer.Manager;

namespace ImageViewer
{
    partial class SettingForm : Form
    {
        private readonly FolderTextSetting _folderSetting;

        public SettingForm(FolderTextSetting folderSetting)
        {
            InitializeComponent();

            _folderSetting = folderSetting;

            TBTitle.Text = _folderSetting.TitlesText;
            TBText.Text = _folderSetting.FolderText;
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            _folderSetting.TitlesText = TBTitle.Text;
            _folderSetting.FolderText = TBText.Text;

            _folderSetting.SaveSetting();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void SettingForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    TBMain.SelectedIndex = 0;
                    break;
                case Keys.F6:
                    TBMain.SelectedIndex = 1;
                    break;
                case Keys.F1:
                    BTOk.PerformClick();
                    break;
            }
        }
    }
}
