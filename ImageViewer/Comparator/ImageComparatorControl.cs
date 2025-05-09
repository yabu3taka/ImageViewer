using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using ImageViewer.Manager;
using ImageViewer.UI;
using ImageViewer.Util;

namespace ImageViewer.Comparator
{
    partial class ImageComparatorControl : UserControl, ISideBarUI
    {
        public ImageComparatorControl()
        {
            InitializeComponent();

            CBMethod.SelectedIndex = 0;
            UDNear.Value = 35;
        }

        private SingleForm MyMain => (SingleForm)ParentForm;

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

        public bool ListBoxFocused => LVFile.Focused;

        public void UpdateOnOpening(SideBarOpenReason reason)
        {
        }

        public void UpdateFileList(SideBarUpdateInfo info)
        {
            info.UpdateImageFileListView(LVFile, MyMain?.SelectedTabPage?.FileCursor);
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
        }
        #endregion

        #region Setting
        private void LKNear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (UDNear.Value >= 50)
            {
                UDNear.Value = 35;
            }
            else
            {
                UDNear.Value = 75;
            }
        }

        private void CBMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCompareStatus();
        }

        private void UDNear_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompareStatus();
        }

        private IImageComparationManagerFactory GetFactory()
        {
            return CBMethod.Text switch
            {
                "GrayHash" => new ImageGrayScaleComparator((int)UDNear.Value),
                _ => new ImageAveComparator((int)UDNear.Value),
            };
        }

        private string _method = "";
        private int _near = -1;
        private bool _targetChanged;

        public void CommitSetting()
        {
            _method = CBMethod.Text;
            _near = (int)UDNear.Value;
            _targetChanged = false;
        }

        public bool IsSettingChanged()
        {
            if (!string.Equals(_method, CBMethod.Text))
            {
                return true;
            }
            if (_near != (int)UDNear.Value)
            {
                return true;
            }
            if (_targetChanged)
            {
                return true;
            }
            return false;
        }

        public void UpdateCompareStatus()
        {
            if (IsSettingChanged())
            {
                GBResult.Text = "比較結果(要再実行)";
            }
        }
        #endregion

        #region Target
        private ImageComparationTarget GetTarget()
        {
            var ret = new ImageComparationTarget();
            bool hasTarget = false;
            foreach (var tabPage in MyMain.TabManager.FileTabPages)
            {
                if (tabPage.TargetedPage)
                {
                    ret.AddCursor(tabPage.OrigCursor);
                    hasTarget = true;
                }
            }
            if (!hasTarget)
            {
                ret.AddCursor(MyMain.SelectedTabPage.OrigCursor);
            }
            return ret;
        }
        #endregion

        #region Compare
        private IImageComparationManager _comparation = null;
        private IImageComparationResult _comparationResult = null;

        public bool IsBusy => CompareWorker.IsBusy;

        public void ClearResult()
        {
            _comparationResult = null;
            GBResult.Visible = false;

            CBKeyFile.Items.Clear();
        }

        private void BTRun_Click(object sender, System.EventArgs e)
        {
            if (CompareWorker.IsBusy)
            {
                return;
            }

            var fac = GetFactory();
            _comparation = fac.ReplaceManager(_comparation);

            var target = GetTarget();
            target.SetKeyFiles(MyMain.TabManager.PinnedCursor?.GetLimitedTargets());

            ClearResult();
            CommitSetting();

            var arg = new ImageComparationWorkerArg()
            {
                ComparationManager = _comparation,
                TargetList = target
            };
            CompareWorker.RunWorkerAsync(arg);
        }

        private void CompareWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ((ImageComparationWorkerArg)e.Argument).Process((BackgroundWorker)sender);
        }

        private void CompareWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var s = (ImageComparationProgress)e.UserState;
            MyMain.SetProgressMessage($"{s.Name} {s.Pos}/{s.Num}");
        }

        private void CompareWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error is not null)
            {
                return;
            }

            MyMain.SetProgressMessage("");

            _comparationResult = (IImageComparationResult)e.Result;

            if (_comparationResult.Count > 0)
            {
                GBResult.Visible = true;
                GBResult.Text = $"比較結果({_comparationResult.Count})";

                CBKeyFile.BeginUpdate();
                CBKeyFile.Items.Clear();
                for (int i = 0; i < _comparationResult.Count; ++i)
                {
                    CBKeyFile.Items.Add(_comparationResult.GetViewerText(i));
                }
                CBKeyFile.EndUpdate();

                CBKeyFile.SelectedIndex = -1;
            }
        }
        #endregion

        #region Result Viewer
        private void CBKeyFile_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (CBKeyFile.SelectedIndex >= 0)
            {
                ShowResultGroup(CBKeyFile.SelectedIndex);
            }
        }

        public void MoveResultGroup(int i)
        {
            if (_comparationResult is null)
            {
                return;
            }

            int newPos = CBKeyFile.SelectedIndex + i;
            if (newPos < 0)
            {
                return;
            }
            if (newPos >= _comparationResult.Count)
            {
                return;
            }
            CBKeyFile.SelectedIndex = newPos;

            ShowResultGroup(newPos);
        }

        private void ShowResultGroup(int pos)
        {
            var c = FilePosCursor.CreateFirst(_comparationResult.GetViewer(pos));
            MyMain.TabManager.ShowComparePage(c);
        }
        #endregion

        #region File List
        private void LVFile_ItemActivate(object sender, EventArgs e)
        {
            MyMain.RunAction();
        }
        #endregion
    }
}
