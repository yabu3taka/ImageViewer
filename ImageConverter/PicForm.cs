using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ImageConverter.Util;

namespace ImageConverter
{
    public partial class PicForm : Form
    {
        public PicForm()
        {
            InitializeComponent();

            MainPic.MouseWheel += this.MainPic_MouseWheel;
        }

        private List<string> _files;

        public void ShowPicFolder(string dir)
        {
            _files = FileUtil.GetExtList(".png", ".jpg", ".jpeg")
                .EnumerateFiles(dir)
                .OrderBy(f => f)
                .ToList();

            if (_files.Count == 0)
            {
                MainPic.Image = null;
                return;
            }

            string f = _files[0];
            if (string.IsNullOrEmpty(f))
            {
                MainPic.Image = null;
                return;
            }

            MainPic.ImageLocation = f;
        }

        private void MainPic_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_files.Count == 0)
            {
                return;
            }

            int num = FormUtil.GetWheelTick(e);
            if (num == 0)
            {
                return;
            }

            int pos = _files.IndexOf(MainPic.ImageLocation);
            pos -= num;
            if (pos < 0)
            {
                return;
            }
            if (pos >= _files.Count)
            {
                return;
            }

            MainPic.ImageLocation = _files[pos];
        }

        private static Size? oldSize = null;

        private void PicForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oldSize = Size;
            ((Form1)Owner).PicFormOpened = false;
        }

        private void PicForm_Shown(object sender, EventArgs e)
        {
            if (oldSize.HasValue)
            {
                Size = oldSize.Value;
            }
        }
    }
}
