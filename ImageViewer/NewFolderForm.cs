using System;
using System.IO;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;
using System.ComponentModel;

namespace ImageViewer
{
    partial class NewFolderForm : Form
    {
        private readonly IRealFolder _parentFolder;
        private readonly string _currentName;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileActionResult ActionResult { get; private set; }

        public NewFolderForm(IRealFolder parentFolder, string currentName)
        {
            InitializeComponent();

            ActionResult = null;
            _parentFolder = parentFolder;
            _currentName = currentName;

            TBNewName.Text = "";
            TBNewName.Select();
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            if (!FormUtil.CheckRequired(TBNewName, "新しい名前を入力して下さい"))
            {
                return;
            }

            string name = TBNewName.Text;
            var action = new FileNewDirAction(_parentFolder.Combine(name));
            if (!action.Commitable)
            {
                FormUtil.ShowValidationError(TBNewName, "同じフォルダが既にあります");
                return;
            }

            ActionResult = action.Commit();
            if (ActionResult is null)
            {
                FormUtil.ShowError(this, "フォルダ作成に失敗しました");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LKFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TBNewName.Text = _currentName;
            TBNewName.Select();
        }
    }
}
