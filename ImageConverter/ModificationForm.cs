using ImageConverter.Data;
using ImageConverter.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageConverter
{
    public partial class ModificationForm : Form
    {
        public ModificationForm()
        {
            InitializeComponent();
        }

        public void ShowModFolders(List<FolderInfo> folders)
        {
            CBList.Items.Clear();
            foreach (var folder in folders)
            {
                if (folder.Modification is not null)
                {
                    foreach (var info in folder.Modification.GetInfos(Path.GetFileName(folder.Dir)))
                    {
                        CBList.Items.Add(info);
                    }
                }
            }
        }

        private void CBList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ((WorkModification.InfoData)CBList.Items[e.Index]).Flag = e.NewValue == CheckState.Checked;
        }

        private static Size? oldSize = null;

        private void PicForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oldSize = Size;
            ((Form1)Owner).ModFormOpened = false;
        }

        private void PicForm_Shown(object sender, EventArgs e)
        {
            if (oldSize.HasValue)
            {
                Size = oldSize.Value;
            }
        }

        private void BtnRecheck_Click(object sender, EventArgs e)
        {
            ((Form1)Owner).DoSyncTabletCheck();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            ((Form1)Owner).DoSyncTabletCopy();
        }
    }
}