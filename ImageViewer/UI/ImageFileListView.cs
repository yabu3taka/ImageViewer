using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.Properties;

namespace ImageViewer.UI
{
    partial class ImageFileListView : ListView
    {
        private readonly BackgroundWorker _worker;

        public ImageFileListView()
        {
            InitializeComponent();

            _worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            ImageList imgList = new();
            imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            imgList.TransparentColor = System.Drawing.Color.Transparent;
            imgList.Images.Add((Bitmap)Resources.ResourceManager.GetObject("icon0"));
            imgList.Images.Add((Bitmap)Resources.ResourceManager.GetObject("icon1"));
            imgList.Images.Add((Bitmap)Resources.ResourceManager.GetObject("icon2"));
            imgList.Images.Add((Bitmap)Resources.ResourceManager.GetObject("icon3"));
            imgList.Images.Add((Bitmap)Resources.ResourceManager.GetObject("icon4"));
            StateImageList = imgList;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var tmp = (IReadOnlyList<IFileInfo>)e.Argument;
            foreach (var finfo in tmp)
            {
                if (_worker.CancellationPending)
                {
                    return;
                }
                if (finfo is IFileDetail dinfo)
                {
                    dinfo.LoadDetailedTitle();
                }
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error is not null)
            {
                return;
            }

            var c = _cursor;
            if (c is not null)
            {
                if (c.FileList.Detail)
                {
                    c.FileList.ReSort();
                }
                UpdateFileListInternal(c);
            }
        }

        #region ListViewItem
        public delegate Color BgColorDelegation(IFileInfo finfo);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BgColorDelegation BgColorFunc { get; set; }

        private ListViewItem UpdateListViewItem(FileInfoList fileList, ListViewItem item)
        {
            var finfo = (IFileInfo)item.Tag;
            if (fileList.DeleteMarker[finfo])
            {
                item.StateImageIndex = 1;
            }
            else if (finfo.GetIndexMark())
            {
                item.StateImageIndex = 0;
            }
            else
            {
                item.StateImageIndex = -1;
            }

            if (BgColorFunc is not null)
            {
                item.BackColor = BgColorFunc(finfo);
            }

            return item;
        }
        #endregion

        #region FilePosCursor
        private IFilePosCursor _cursor;

        public void UpdateFileList(IFilePosCursor c)
        {
            _worker?.CancelAsync();

            _cursor = c;
            if (c is not null)
            {
                MultiSelect = c.MultiSelect;
                UpdateFileListInternal(c);

                if (c.FileList.Detail)
                {
                    var tmp = c.FileList.Items;
                    _worker.RunWorkerAsync(tmp);
                }
            }
            else
            {
                Items.Clear();
            }
        }

        private void UpdateFileListInternal(IFilePosCursor c)
        {
            BeginUpdate();
            Items.Clear();

            var v = c.GetIFilePosViewer();
            foreach (var finfo in c.FileList.Items)
            {
                string title = v.GetTitle(finfo);
                var item = new ListViewItem(title) { Tag = finfo };
                Items.Add(UpdateListViewItem(c.FileList, item));
            }
            UpdateFileSelection(true);

            EndUpdate();
        }

        public void UpdateMarkStatus(bool reload = true)
        {
            var c = _cursor;
            if (c is not null)
            {
                if (c.FileList.FilterType == FileFilterType.All)
                {
                    foreach (var item in Items)
                    {
                        UpdateListViewItem(c.FileList, (ListViewItem)item);
                    }
                }
                else
                {
                    if (reload)
                    {
                        c.Reload();
                    }
                    UpdateFileListInternal(c);
                }
            }
        }

        private bool _selectLoop = false;
        private bool _noCursorUpdate = false;

        public void UpdateFileSelection()
        {
            UpdateFileSelection(false);
        }

        private void UpdateFileSelection(bool force)
        {
            if (!force)
            {
                if (_selectLoop)
                {
                    return;
                }
            }

            var c = _cursor;
            if (c is null)
            {
                return;
            }

            _noCursorUpdate = true;
            try
            {
                SelectedIndices.Clear();

                var fhash = new HashSet<string>();
                foreach (var finfo in c.SelectedItems)
                {
                    fhash.Add(finfo.FileName);
                }
                var fkey = c.Current?.FileName ?? "";

                int index = -1;
                for (int i = 0; i < Items.Count; ++i)
                {
                    var finfo = (IFileInfo)Items[i].Tag;
                    if (fhash.Contains(finfo.FileName))
                    {
                        SelectedIndices.Add(i);
                    }
                    if (fkey == finfo.FileName)
                    {
                        index = i;
                    }
                }

                if (index >= 0)
                {
                    EnsureVisible(index);
                    Items[index].Focused = true;
                }
            }
            finally
            {
                _noCursorUpdate = false;
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (_selectLoop)
            {
                return;
            }
            if (_noCursorUpdate)
            {
                return;
            }

            _selectLoop = true;

            var c = _cursor;
            BeginInvoke(() =>
            {
                try
                {
                    if (c is not null)
                    {
                        if (SelectedIndices.Count >= 1)
                        {
                            var list = SelectedItems.OfType<ListViewItem>().Select(item => (IFileInfo)item.Tag).ToList();
                            var cur = SelectedItems.OfType<ListViewItem>().Where(item => item.Focused).Select(item => (IFileInfo)item.Tag).FirstOrDefault();
                            c.SetSelectedItems(list, cur);
                        }
                    }
                }
                finally
                {
                    _selectLoop = false;
                }
            });
        }
        #endregion
    }
}
