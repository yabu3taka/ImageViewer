using System;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Util;
using System.ComponentModel;

namespace ImageViewer
{
    partial class BoundForm : Form
    {
        private readonly FilePosCursor _cursor;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileInfoListCondition ResultCondition { get; private set; }

        public BoundForm(FilePosCursor c)
        {
            InitializeComponent();

            _cursor = c;

            LBFile.Text = c.Current.FileName;

            FileNameStyle fstyle;
            if (c.FileList.Bound is FileNameStyleBound b)
            {
                fstyle = b.NameStyle;
            }
            else
            {
                fstyle = FileNameUtil.CreateAuto(c.Current);
            }
            if (fstyle is not null)
            {
                TBFilter.Text = fstyle.ToString();
            }

            Program.SetupFilterFileNames(TBFilter);
            TBFilter.SelectAll();
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            var newCond = _cursor.FileList.StartEdit();

            if (TBFilter.Text == "")
            {
                if (newCond.Bound is FileNameStyleBound)
                {
                    newCond.Bound = null;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }
            }
            else
            {
                var style = FileNameStyle.FromName(TBFilter.Text);
                if (style is null)
                {
                    FormUtil.ShowValidationError(TBFilter, "フィルターを正しく入力して下さい");
                    return;
                }

                Program.AddFilterFileName(TBFilter.Text);

                newCond.Bound = new FileNameStyleBound(style);
            }

            ResultCondition = newCond;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
