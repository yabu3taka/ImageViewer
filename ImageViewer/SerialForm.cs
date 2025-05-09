using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;
using System.ComponentModel;

namespace ImageViewer
{
    partial class SerialForm : Form
    {
        private List<IFileInfo> _list = null;
        private readonly IFileBound _origBound;
        private IFileBound _targetBound;
        private readonly FilePosCursor _cursor;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileActionResult ActionResult { get; private set; } = null;

        public SerialForm(FilePosCursor c)
        {
            InitializeComponent();

            CBSortOrder.Items.Clear();
            foreach (string name in FileComparerUtil.GetFileComparerTypeNames())
            {
                CBSortOrder.Items.Add(name);
            }

            _cursor = c.Clone();
            _origBound = _cursor.FileList.Bound;

            var newCond = _cursor.FileList.StartEdit();
            newCond.FilterType = FileFilterType.All;
            _cursor.CommitCondition(newCond);

            if (_cursor.SelectedItems.HasMany())
            {
                CBSerial.Enabled = false;
            }
            else
            {
                if (_origBound is not null)
                {
                    CBSerial.Checked = false;
                }
                else
                {
                    CBSerial.Checked = true;
                }
            }
            UpdateFromList();

            var style = FileNameUtil.CreateAuto(_cursor.Current);
            if (style is not null)
            {
                TBStyle.Text = style.ToString();
            }
            TBStyle.SelectAll();

            TogglePicBox();

            Program.SetupSerialFileNames(TBStyle);

            UpdateToList();

            CBSortOrder.SelectedIndex = 0;
        }

        #region From/To List
        private void UpdateFromList()
        {
            var items = _cursor.SelectedItems;
            if (items.HasMany())
            {
                _list = new List<IFileInfo>(items);
            }
            else
            {
                _list = null;
                bool hasStyle = false;
                if (CBSerial.Checked)
                {
                    var style = FileNameUtil.CreateAuto(_cursor.Current);
                    if (style is not null)
                    {
                        var bound = new FileNameStyleBound(style);
                        _list = _cursor.FileList.QuickList(bound);
                        if (_list.Count > 1)
                        {
                            TBStyle.Text = style.ToString();
                            TBStyle.Select();
                            hasStyle = true;

                            _targetBound = bound;
                        }
                    }
                }
                if (!hasStyle)
                {
                    CBSerial.Checked = false;
                    _list = _cursor.FileList.QuickList(_origBound);
                    _targetBound = _origBound;
                }
            }

            SortList();
        }

        private void SortList()
        {
            _cursor.FileList.CreateFileComparer((FileSortType)CBSortOrder.SelectedIndex).SortList(_list);
            LBFrom.Items.Clear();
            foreach (IFileInfo finfo in _list)
            {
                LBFrom.Items.Add(finfo.FileName);
            }
        }

        private void GenToTextFromSelection()
        {
            string name = (string)LBFrom.SelectedItem;
            if (name is null)
            {
                return;
            }

            var style = FileNameUtil.CreateAuto(name);
            if (style is null)
            {
                return;
            }

            TBStyle.Text = style.ToString();
            TBStyle.SelectAll();
        }

        private void UpdateToList()
        {
            LBTo.Items.Clear();

            var style = FileNameStyle.FromName(TBStyle.Text);
            if (style is null)
            {
                return;
            }

            var trans = _list.SerialConvert((IRealFolder)_cursor.GetMyManager(), style.MakeGenerator());
            if (trans is null)
            {
                return;
            }

            foreach (var item in trans.RenameItems)
            {
                LBTo.Items.Add(Path.GetFileName(item.ToFile.ImageFilePath));
            }
        }
        #endregion

        #region From/To Event
        private void CBSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortList();
        }

        private void CBSerial_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFromList();
            UpdateToList();
        }

        private void LBFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePicBox();
        }

        private void LBFrom_DoubleClick(object sender, EventArgs e)
        {
            GenToTextFromSelection();
        }

        private void LBFrom_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    GenToTextFromSelection();
                    break;
            }
        }

        private void TBStyle_TextChanged(object sender, EventArgs e)
        {
            UpdateToList();
        }

        private void TBStyle_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    BTOk.PerformClick();
                    break;
            }
        }
        #endregion

        #region PicBox
        private bool PicBoxMode
        {
            get { return MainPanel.RowStyles[1].Height != 0; }
        }

        private void UpdatePicBox()
        {
            if (PicBoxMode)
            {
                if (LBFrom.SelectedIndex >= 0)
                {
                    PicBox.ImageLocation = _list[LBFrom.SelectedIndex].FilePath;
                }
                else
                {
                    PicBox.Image = null;
                }
            }
        }

        private void TogglePicBox()
        {
            if (PicBoxMode)
            {
                MainPanel.RowStyles[0] = new RowStyle(SizeType.Percent, 100F);
                MainPanel.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
            }
            else
            {
                MainPanel.RowStyles[0] = new RowStyle(SizeType.Absolute, 0F);
                MainPanel.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
            }
            UpdatePicBox();
        }

        private void BTPrint_Click(object sender, EventArgs e)
        {
            TogglePicBox();
        }
        #endregion

        #region Rename
        private bool HasLimit
        {
            get
            {
                if (_cursor.SelectedItems.HasMany())
                {
                    return true;
                }
                if (_targetBound is not null)
                {
                    return true;
                }
                return false;
            }
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            TBStyle.BackColor = SystemColors.Window;

            var style = FileNameStyle.FromName(TBStyle.Text);
            if (style is null)
            {
                LBStatus.Text = "変更先ファイル名を正しく入力して下さい";
                TBStyle.BackColor = Color.Pink;
                return;
            }

            var trans = _list.SerialConvert((IRealFolder)_cursor.GetMyManager(), style.MakeGenerator());
            if (trans is null)
            {
                LBStatus.Text = "シリアル範囲が足りませんでした";
                TBStyle.BackColor = Color.Pink;
                return;
            }

            if (!trans.Commitable)
            {
                LBStatus.Text = "同じファイルが既にあるため、ファイル名変更ができません";
                TBStyle.BackColor = Color.Pink;
                return;
            }

            if (!HasLimit)
            {
                if (!FormUtil.Confirm(this, "範囲が設定されていませんが、全て変更してもいいですか"))
                {
                    return;
                }
            }

            Program.AddSerialFileName(TBStyle.Text);

            ActionResult = trans.Commit();
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        private void SerialForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    BTPrint.PerformClick();
                    break;
                case Keys.F2:
                    CBSerial.Checked = !CBSerial.Checked;
                    break;
            }
        }
    }
}
