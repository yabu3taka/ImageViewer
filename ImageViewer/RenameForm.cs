using System;
using System.IO;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;
using System.ComponentModel;

namespace ImageViewer
{
    partial class RenameForm : Form
    {
        private readonly IRealFolder _targetFolder;
        private readonly IRealFile _targetFile;
        private readonly IRealFolder _parentFolder;
        private readonly string _origExt;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileActionResult ActionResult { get; private set; }

        public string NewName
        {
            get { return TBNewName.Text; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IRealFolder, TextBox, bool> CheckFunc { get; set; }

        private void InitFirst()
        {
            InitializeComponent();

            ActionResult = null;
        }

        public RenameForm(IRealFolder folder)
        {
            InitFirst();

            _targetFolder = folder;
            _parentFolder = folder.GetParent();
            _origExt = "";
            InitForm(Path.GetFileName(folder.FilePath));

            LKNewName.Text = "新しい名前";
            LKNewName.Enabled = false;
        }

        public RenameForm(IRealFile file)
        {
            InitFirst();

            _targetFile = file;
            _parentFolder = file.GetParent();
            _origExt = Path.GetExtension(file.FileName);
            InitForm(file.FileName);
        }

        private void InitForm(string filename)
        {
            LBOldName.Text = filename;
            TBNewName.Text = filename;
            FormUtil.SelectFileNameExceptFileName(TBNewName);

            Text = TargetName + "変更";
        }

        private bool IsTargetDir()
        {
            return _targetFolder is not null;
        }

        private string TargetName
        {
            get
            {
                if (IsTargetDir())
                {
                    return "フォルダ名";
                }
                return "ファイル名";
            }
        }

        private IFileAction GetFileAction()
        {
            if (IsTargetDir())
            {
                string name = TBNewName.Text;
                return new FileRenameDirAction(_targetFolder, _parentFolder.Combine(name));
            }
            else
            {
                string name = FileUtil.AppendExt(TBNewName.Text, _origExt);
                return new FileRenameAction(_targetFile, _parentFolder, name);
            }
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            if (!FormUtil.CheckRequired(TBNewName, "新しい名前を入力して下さい"))
            {
                return;
            }

            if (CheckFunc is not null)
            {
                if (!CheckFunc(_parentFolder, TBNewName))
                {
                    return;
                }
            }

            var action = GetFileAction();
            if (!action.Commitable)
            {
                FormUtil.ShowValidationError(TBNewName, $"同じ{TargetName}が既にあります");
                return;
            }

            ActionResult = action.Commit();
            if (ActionResult is null)
            {
                FormUtil.ShowError(this, "名前変更に失敗しました");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void AutoName()
        {
            if (IsTargetDir())
            {
                return;
            }

            var style = FileNameUtil.CreateAuto(TBNewName.Text);
            if (style is null)
            {
                return;
            }

            string origName = FileUtil.AppendExt(TBNewName.Text, _origExt);
            TBNewName.Text = _parentFolder.FindNonExist(origName, style.MakeInitGenerator());
            FormUtil.SelectFileNameExceptFileName(TBNewName);
        }

        private void RenameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    AutoName();
                    break;
            }
        }

        private void LKNewName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AutoName();
        }
    }
}
