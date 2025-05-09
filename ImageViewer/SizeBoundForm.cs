using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ImageViewer.Manager;

namespace ImageViewer
{
    partial class SizeBoundForm : Form
    {
        private readonly FilePosCursor _cursor;
        private readonly List<IFileInfo> _allList;
        private readonly FileWidthHeightCache _sizeCache = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileInfoListCondition ResultCondition { get; private set; }

        public SizeBoundForm(FilePosCursor c)
        {
            InitializeComponent();

            _cursor = c;

            if (c.FileList.Bound is FileWidthHeightBound b)
            {
                TBSizeWidth.Text = ConvTo(b.Width);
                TBSizeHeight.Text = ConvTo(b.Height);
            }

            _allList = c.GetMyManager().GetAllItems();
            SetupSizeInfo();
        }

        private string ConvTo(int val)
        {
            return val <= 0 ? "" : val.ToString();
        }

        private void SetupSizeInfo()
        {
            _allList.ForEach(_sizeCache.Add);
            if (_sizeCache.Count > 0)
            {
                LBSizeWidthMax.Text += _sizeCache.WidthMax;
                LBSizeWidthMin.Text += _sizeCache.WidthMin;

                LBSizeHeightMax.Text += _sizeCache.HeightMax;
                LBSizeHeightMin.Text += _sizeCache.HeightMin;
            }
        }

        private List<IFileInfo> GetTargetList()
        {
            var bound = FileWidthHeightBound.Create(TBSizeWidth.Text, TBSizeHeight.Text);
            if (bound is null)
            {
                return [];
            }

            bound.Cache = _sizeCache;
            var filterFunc = bound.CreateFilter();
            return _allList.Where(filterFunc).ToList();
        }

        private void UpdateTargetList()
        {
            var list = GetTargetList();
            LBTarget.Items.Clear();
            foreach (var finfo in list)
            {
                LBTarget.Items.Add(finfo.Title);
            }
            LBList.Text = "対象：" + list.Count;
        }

        private void TMMain_Tick(object sender, EventArgs e)
        {
            UpdateTargetList();
            TMMain.Stop();
        }

        private void TBSizeWidthHeight_TextChanged(object sender, EventArgs e)
        {
            TMMain.Stop();
            TMMain.Start();
        }

        [GeneratedRegex(@"^\d*$")]
        private static partial Regex NumberRegex();

        private void TBSizeWidthHeight_Validating(object sender, CancelEventArgs e)
        {
            var regex = NumberRegex();
            if (!regex.IsMatch(((TextBox)sender).Text))
            {
                EPWidthHeight.SetError((TextBox)sender, "数字のみ");
            }
            else
            {
                EPWidthHeight.SetError((TextBox)sender, "");
            }
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            var newCond = _cursor.FileList.StartEdit();

            var bound = FileWidthHeightBound.Create(TBSizeWidth.Text, TBSizeHeight.Text);
            if (bound is null)
            {
                if (newCond.Bound is FileWidthHeightBound b)
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
                bound.Cache = _sizeCache;
                newCond.Bound = bound;
            }

            ResultCondition = newCond;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void TBClear_Click(object sender, EventArgs e)
        {
            TBSizeWidth.Text = ""; 
            TBSizeHeight.Text = "";
        }
    }
}
