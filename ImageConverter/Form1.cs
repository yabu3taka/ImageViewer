using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageConverter.PortableDevices;
using ImageConverter.Manager;
using ImageConverter.Util;
using ImageConverter.Work;
using ImageConverter.Data;

namespace ImageConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSetting();
        }

        #region タブ制御
        private const int TAB_CONVERT = 0;
        private const int TAB_TOOL = 1;

        private void Form1_Shown(object sender, EventArgs e)
        {
            string errorMess = AppSetting.Default.Validate();
            if (!string.IsNullOrEmpty(errorMess))
            {
                SLbStatus.Text = errorMess;
                TabMain.SelectedIndex = TAB_TOOL;
            }
            else
            {
                TabMain.SelectedIndex = TAB_CONVERT;
                ScanDirList();
            }
        }

        private void TabMain_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case TAB_CONVERT:
                    CloseDirInfoForm();
                    break;
            }
        }

        private void TabMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case TAB_CONVERT:
                    SLbStatus.Text = "";
                    string errorMess = AppSetting.Default.Validate();
                    if (!string.IsNullOrEmpty(errorMess))
                    {
                        SLbStatus.Text = errorMess;
                        e.Cancel = true;
                        return;
                    }
                    break;
            }
        }

        private void TabMain_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case TAB_CONVERT:
                    ScanDirList();
                    break;
            }
        }
        #endregion

        #region 画像/ファイル表示
        private PicForm _picForm = null;
        private ModificationForm _modForm = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PicFormOpened
        {
            get { return ChkPreview.Checked; }
            set { ChkPreview.Checked = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ModFormOpened
        {
            get { return ChkMod.Checked; }
            set { ChkMod.Checked = value; }
        }

        private Point GetDirInfoFormPoint()
        {
            return new Point(Location.X + Size.Width, Location.Y);
        }

        private void OpenDirInfoForm(List<FolderInfo> folders)
        {
            if (ChkPreview.Checked)
            {
                if (folders.Count > 0)
                {
                    _picForm = FormUtil.CreateFormIfNeed(this, _picForm, () => new PicForm
                    {
                        Location = GetDirInfoFormPoint()
                    });
                    _picForm.ShowPicFolder(folders[0].Dir);
                }
                else
                {
                    ClosePicForm();
                }
            }

            if (ChkMod.Checked)
            {
                if (folders.Count > 0)
                {
                    _modForm = FormUtil.CreateFormIfNeed(this, _modForm, () => new ModificationForm
                    {
                        Location = GetDirInfoFormPoint()
                    });
                    _modForm.ShowModFolders(folders);
                }
                else
                {
                    CloseModForm();
                }
            }
        }

        private void CloseDirInfoForm()
        {
            ClosePicForm();
            CloseModForm();
        }

        private void ClosePicForm()
        {
            if (_picForm is not null)
            {
                _picForm.Close();
                _picForm = null;
            }
        }
        private void CloseModForm()
        {
            if (_modForm is not null)
            {
                _modForm.Close();
                _modForm = null;
            }
        }

        private void ChkPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (!ChkPreview.Checked)
            {
                ClosePicForm();
            }
            else
            {
                ChkMod.Checked = false;
                var dirs = GetSelectedFolderInfos();
                    OpenDirInfoForm(dirs);
            }
        }

        private void ChkMod_CheckedChanged(object sender, EventArgs e)
        {
            if (!ChkMod.Checked)
            {
                CloseModForm();
            }
            else
            {
                ChkPreview.Checked = false;
                var dirs = GetSelectedFolderInfos();
                if (dirs.Count > 0)
                {
                    OpenDirInfoForm(dirs);
                }
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            void chl(Form f)
            {
                f.Location = new Point(Location.X + Size.Width, Location.Y);
            }
            FormUtil.ChangeFormIfValid(_picForm, chl);
            FormUtil.ChangeFormIfValid(_modForm, chl);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppSetting.Default.ShowPreview = ChkPreview.Checked;
            AppSetting.Default.Save();
        }
        #endregion

        #region 設定
        private MaskConverter _maskConverter = null;
        private NormalConvertWorkProcessor _normalConvertProc = null;
        private LightConvertWorkProcessor _lightConvertProc = null;

        private void LoadSetting()
        {
            _maskConverter = AppSetting.Default.CreateMaskConverter();
            _normalConvertProc = AppSetting.Default.CreateNormalConvertWorkProcessor(_maskConverter);
            _lightConvertProc = AppSetting.Default.CreateLightConvertWorkProcessor(_normalConvertProc);

            TbSyncDir.Text = AppSetting.Default.SyncDir;

            ChkPreview.Checked = AppSetting.Default.ShowPreview.Value;
        }

        private MaskConvertWorkProcessor GetMaskConvertWorkProcessor()
        {
            if (_lightConvertProc is null)
            {
                return _normalConvertProc;
            }
            return _lightConvertProc;
        }

        private LightConvertWorkProcessor GetLightConvertWorkProcessor()
        {
            if (_lightConvertProc is null)
            {
                throw new MyException("軽量フォルダーを選択して下さい");
            }
            return _lightConvertProc;
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            using SettingDlg settingDlg = new();
            if (settingDlg.ShowDialog() == DialogResult.OK)
            {
                LoadSetting();
            }
        }
        #endregion

        #region ツールタブ
        private void BtnPass_Click(object sender, EventArgs e)
        {
            try
            {
                _maskConverter.SavePassword(_normalConvertProc, TbPass.Text);
                _maskConverter.SavePassword(_lightConvertProc, TbPass.Text);
                SLbStatus.Text = "パスワードファイルを作成しました";
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(GetType(), @"BtnPass_Click", ex);
                SLbStatus.Text = "パスワードファイル作成に失敗しました";
            }
        }

        private void ChkLog_CheckedChanged(object sender, EventArgs e)
        {
            SimpleLogUtil.LogFlag = ChkLog.Checked;
        }
        #endregion

        #region フォルダ一覧
        private Task _scanWorkTask = null;
        private bool IsScanBusy => !(_scanWorkTask?.IsCompleted ?? true);
        private List<FolderInfo> _dirList;

        private void RunScanWork(List<FolderInfo> dirs)
        {
            if (!IsScanBusy)
            {
                _scanWorkTask = Task.Run(() =>
                {
                    foreach (var d in dirs)
                    {
                        d.UpdateHeavyInfo();
                    }
                }).ContinueWith(task => UpdateDirList(dirs), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ScanDirList()
        {
            var dirs = AppSetting.Default.GetFolderList();
            _dirList = dirs;
            UpdateDirListUI(dirs);
            RunScanWork(dirs);
        }

        private void UpdateModifiedDate()
        {
            RunScanWork(_dirList);
        }

        private void UpdateDirListUI(List<FolderInfo> dirs)
        {
            UpdateDirList(dirs);
            LbFolder_SelectedValueChanged(LbFolder, new EventArgs());
        }

        private void CBSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDirListUI(_dirList);
        }

        private void TbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                UpdateDirListUI(_dirList);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void UpdateLbFolderUI(Action a)
        {
            try
            {
                LbFolder.BeginUpdate();
                LbFolder.SelectedValueChanged -= LbFolder_SelectedValueChanged;
                a();
            }
            finally
            {
                LbFolder.SelectedValueChanged += LbFolder_SelectedValueChanged;
                LbFolder_SelectedValueChanged(LbFolder, new EventArgs());
                LbFolder.EndUpdate();
            }
        }

        private readonly WorkSetting _setting = new();
        private void UpdateDirList(List<FolderInfo> paramsDirs)
        {
            List<FolderInfo> dirs = new(paramsDirs);

            foreach (var d in dirs)
            {
                string name = Path.GetFileName(d.Dir);
                d.DiffType = _setting.GetModifiedType(name);
                d.Modification = _setting.GetModification(name);
            }

            if (!string.IsNullOrEmpty(TbFilter.Text))
            {
                dirs.RemoveAll((d) => d.NotMatch(TbFilter.Text));
            }

            dirs.Sort(CmbSort.SelectedIndex switch
            {
                2 => FolderInfo.CompareModified,
                1 => FolderInfo.CompareLastWirteDate,
                _ => FolderInfo.CompareName
            });

            void a()
            {
                var hash = new HashSet<FolderInfo>();
                hash.UnionWith(LbFolder.SelectedItems.Cast<FolderInfo>());

                LbFolder.Items.Clear();
                foreach (var d in dirs)
                {
                    LbFolder.Items.Add(d);
                }

                for (int i = 0; i < LbFolder.Items.Count; i++)
                {
                    LbFolder.SetSelected(i, hash.Contains(dirs[i]));
                }
            }
            UpdateLbFolderUI(a);
        }

        private void BtnAllSelect_Click(object sender, EventArgs e)
        {
            void a()
            {
                for (int i = 0; i < LbFolder.Items.Count; i++)
                {
                    LbFolder.SetSelected(i, true);
                }
            }
            UpdateLbFolderUI(a);
        }
        #endregion

        #region ボタン制御
        private void SetMultiSelectButtonEnable(bool enable)
        {
            BtnConvert.Enabled = enable;
            BtnGSync.Enabled = enable;
            BtnGSyncLight.Enabled = enable;
            BtnSyncTabletCheck.Enabled = enable;
            BtnSyncTabletDel.Enabled = enable;
            BtnSyncTabletCopy.Enabled = enable;
        }

        private void SetOneSelectButtonEnable(bool enable)
        {
            BtnRename.Enabled = enable;
            BtnDelete.Enabled = enable;
        }

        private void SetNoneSelectButtonEnable(bool enable)
        {
            BtnRevert.Enabled = enable;
            BtnAdd.Enabled = enable;
        }

        private List<FolderInfo> GetSelectedFolderInfos()
        {
            return LbFolder.SelectedItems.Cast<FolderInfo>().ToList();
        }

        private List<FolderInfo> UpdateConvertButtonState(bool busy = false)
        {
            SetMultiSelectButtonEnable(false);
            SetOneSelectButtonEnable(false);
            SetNoneSelectButtonEnable(false);

            if (busy)
            {
                return null;
            }

            var dirs = GetSelectedFolderInfos();
            if (!IsConvertBusy)
            {
                if (dirs.Count > 0)
                {
                    SetMultiSelectButtonEnable(true);
                    if (dirs.Count == 1)
                    {
                        SetOneSelectButtonEnable(true);
                    }
                }
                SetNoneSelectButtonEnable(true);
            }

            return dirs;
        }

        private readonly RunningSection _folderSelectedValueChangedSection = new();

        private void LbFolder_SelectedValueChanged(object sender, EventArgs e)
        {
            var dirs = UpdateConvertButtonState();
            if (dirs.Count == 0)
            {
                TbNewName.Text = "";
                SLbStatus.Text = "";
                return;
            }

            // 選択反映
            string targetDir = dirs[0].Dir;
            OpenDirInfoForm(dirs);
            TbNewName.Text = Path.GetFileName(targetDir);

            if (dirs.Count >= 1)
            {
                SLbStatus.Text = "";
            }
            if (dirs.Count >= 2)
            {
                SLbStatus.Text = $"{dirs.Count} 選択";
            }

            _folderSelectedValueChangedSection.Run(() =>
            {
                ChkBlank.Enabled = false;
                ChkNoSmall.Enabled = false;
                if (dirs.Count >= 1)
                {
                    ChkBlank.Enabled = true;
                    ChkNoSmall.Enabled = true;
                    var setting = new FolderPropSetting(targetDir);
                    ChkBlank.Checked = setting.Blank;
                    ChkNoSmall.Checked = new ConverterFolderSetting(targetDir).NoSmall;
                }
            });
        }
        #endregion

        #region 選択
        private List<string> GetSelectedDirs()
        {
            return LbFolder.SelectedItems.Cast<FolderInfo>().Select(d => d.Dir).ToList();
        }

        private List<string> GetMultiSelectedDirs()
        {
            var dirs = GetSelectedDirs();
            if (dirs.Count == 0)
            {
                throw new MyException("フォルダーを選択して下さい");
            }
            return dirs;
        }

        private string GetOneSelectedDir()
        {
            var dirs = GetSelectedDirs();
            if (dirs.Count != 1)
            {
                throw new MyException("フォルダーを１つ選択して下さい");
            }
            return dirs[0];
        }
        #endregion

        #region 変換処理基盤
        private Task _convertWorkTask = null;
        private bool IsConvertBusy => !(_convertWorkTask?.IsCompleted ?? true);

        private async Task RunConvertWork(WorkData work)
        {
            if (!IsConvertBusy)
            {
                UpdateConvertButtonState(true);

                _convertWorkTask = RunConvertWorkInternal(work);
                await _convertWorkTask;

                ReportConvertProgress("Done");

                UpdateConvertButtonState();

                WorkSetting setting = work.Setting;
                _setting.Merge(setting);
                if (setting.RescanDir)
                {
                    ScanDirList();
                }
                if (setting.UpdateModifedDate)
                {
                    UpdateModifiedDate();
                }
                else
                {
                    UpdateDirListUI(_dirList);
                }
            }
        }

        private async Task RunConvertWorkInternal(WorkData work)
        {
            SimpleLogUtil.Clear();

            ReportConvertProgress("Start");

            await Task.Run(() => work.StartSync());

            ReportConvertProgress("SyncSubFolder");

            int dpos = 0;
            foreach (string subdir in work.SubDirs)
            {
                dpos++;

                foreach (var proc in work.ProcList)
                {
                    var target = new SubFolderWorkData(work, proc, subdir);

                    string dirText = $"{target.FromDir}({dpos}/{work.SubDirs.Count})";
                    ReportConvertProgress(dirText, "Start");

                    bool result = await Task.Run(() => target.StartSync());
                    if (!result)
                    {
                        continue;
                    }

                    int fpos = 0;
                    var delList = await Task.Run(() => target.GetDeleteFileList());
                    foreach (var delFile in delList)
                    {
                        fpos++;
                        ReportConvertProgress(dirText, $"{delFile.FileName}({fpos}/{delList.Count}) Delete");
                        await Task.Run(() => target.DeleteFile(delFile));
                    }
                    result = await Task.Run(() => target.AfterDelete());
                    if (result)
                    {
                        continue;
                    }

                    fpos = 0;
                    var pairList = await Task.Run(() => target.GetPairList());
                    foreach (var pair in pairList)
                    {
                        fpos++;
                        ReportConvertProgress(dirText, $"{pair.Key}({fpos}/{pairList.Count}) Copy");
                        await Task.Run(() => target.CopyFile(pair));
                    }

                    ReportConvertProgress(dirText, "Misc Copy");
                    await Task.Run(() => target.CopyMiscFile());
                }
            }

            ReportConvertProgress("Ending");

            await Task.Run(() => work.EndSync());
        }

        private void ReportConvertProgress(string dir, string mess = "")
        {
            SLbStatus.Text = $"{dir}  {mess}";
        }
        #endregion

        #region 変換
        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                var work = new WorkData(AppSetting.Default.FromDir)
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(_lightConvertProc, _normalConvertProc);
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private async void BtnSync_Click(object sender, EventArgs e)
        {
            try
            {
                var setting = AppSetting.CreateEditData();
                setting.SyncDir = TbSyncDir.Text;

                string mess = setting.ValidateSync();
                if (mess is not null)
                {
                    throw new MyException(mess);
                }

                AppSetting.CommitAndSave(setting);

                var work = new WorkData(_normalConvertProc)
                    .SetAllDirToTarget()
                    .AddProc(AppSetting.Default.CreateSyncWorkProcessor());
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private string _beforeFolder = null;

        private string SelectRevertFolder(string desc)
        {
            using FolderBrowserDialog dlg = new();
            string next = FileUtil.GetNextFolder(_beforeFolder);
            if (next is not null)
            {
                dlg.SelectedPath = next;
            }

            dlg.Description = desc;
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                throw new MyException("中止");
            }

            string dir = dlg.SelectedPath;
            _beforeFolder = dir;

            if (Directory.Exists(Path.Combine(AppSetting.Default.FromDir, Path.GetFileName(dir))))
            {
                if (!FormUtil.Confirm(this, "同じフォルダーが存在します。実施しますか？"))
                {
                    throw new MyException("中止");
                }
            }
            return dir;
        }

        private async void BtnRevert_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = SelectRevertFolder("画像を元に戻します");
                var work = new WorkData(Path.GetDirectoryName(dir))
                    .SetTargetDirs(Path.GetFileName(dir))
                    .AddProc(AppSetting.Default.CreateRevertWorkProcessor(_maskConverter))
                    .AddProc(AppSetting.Default.CreateRevertTextWorkProcessor(_maskConverter));
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }
        #endregion

        #region Tablet
        private static WpdWorkProcessorBase GetSyncTablet(Func<PortableDevice, string, WpdWorkProcessorBase> create)
        {
            string dir = AppSetting.Default.TabletToDir;
            if (string.IsNullOrEmpty(dir))
            {
                throw new MyException("端末保管先を入力して下さい");
            }

            var deviceList = PortableDevice.GetDeviceList();
            if (deviceList.Count <= 0)
            {
                throw new MyException("端末を接続してください");
            }

            PortableDevice target;
            if (deviceList.Count <= 1)
            {
                target = deviceList[0];
            }
            else
            {
                using DeviceDlg dlg = new(deviceList, dir);
                dlg.ShowDialog();
                if (dlg.DialogResult != DialogResult.OK)
                {
                    return null;
                }
                target = dlg.Selected;
            }
            return create(target, dir);
        }

        public void DoSyncTabletCheck()
        {
            BtnSyncTabletCheck.PerformClick();
        }

        private async void BtnSyncTabletCheck_Click(object sender, EventArgs e)
        {
            try
            {
                var work = new WorkData(GetMaskConvertWorkProcessor())
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(GetSyncTablet(WpdWorkProcessorCheck.Create));
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private async void BtnSyncTabletDel_Click(object sender, EventArgs e)
        {
            try
            {
                var work = new WorkData(GetMaskConvertWorkProcessor())
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(GetSyncTablet(WpdWorkProcessorDelete.Create));
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        public void DoSyncTabletCopy()
        {
            BtnSyncTabletCopy.PerformClick();
        }

        private async void BtnSyncTabletCopy_Click(object sender, EventArgs e)
        {
            try
            {
                var work = new WorkData(GetMaskConvertWorkProcessor())
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(GetSyncTablet(WpdWorkProcessorCopy.Create));
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }
        #endregion

        #region Google Drive
        private async void BtnGSync_Click(object sender, EventArgs e)
        {
            try
            {
                var p = AppSetting.Default.CreateNormalGDrive() ?? throw new MyException("Drive保管先を入力して下さい");
                var work = new WorkData(_normalConvertProc)
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(p);
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private async void GSyncLight_Click(object sender, EventArgs e)
        {
            try
            {
                var p = AppSetting.Default.CreateLightGDrive() ?? throw new MyException("Drive保管先を入力して下さい");
                var work = new WorkData(GetLightConvertWorkProcessor())
                    .SetTargetDirs(GetMultiSelectedDirs())
                    .AddProc(p);
                await RunConvertWork(work);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }
        #endregion

        #region フォルダー操作
        private IEnumerable<WorkProcessor> GetFolderProcs()
        {
            return new List<WorkProcessor>
            {
                _lightConvertProc,
                _normalConvertProc,
                AppSetting.Default.CreateNormalGDrive(),
                AppSetting.Default.CreateLightGDrive()
            }.Where((p) => p is not null);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string newName = TbNewName.Text;
                if (string.IsNullOrEmpty(newName))
                {
                    throw new MyException("新しい名前を入力して下さい");
                }

                string newDir = Path.Combine(AppSetting.Default.FromDir, newName);
                if (Directory.Exists(newDir))
                {
                    throw new MyException("新しい名前が既にあります");
                }

                if (!FormUtil.Confirm(this, $"{newDir} を作成しますか"))
                {
                    throw new MyException("中止");
                }

                Directory.CreateDirectory(newDir);

                TbNewName.Text = "";
                ScanDirList();
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            try
            {
                string targetFrom = GetOneSelectedDir();
                if (!Directory.Exists(targetFrom))
                {
                    throw new MyException("元フォルダがありません");
                }

                string newName = TbNewName.Text;
                if (string.IsNullOrEmpty(newName))
                {
                    throw new MyException("新しい名前を入力して下さい");
                }

                var oldName = Path.GetFileName(targetFrom);
                if (FileUtil.EqualsPath(oldName, newName))
                {
                    throw new MyException("古い名前と同じです。新しい名前を入力して下さい");
                }

                string newDir = Path.Combine(AppSetting.Default.FromDir, newName);
                if (Directory.Exists(newDir))
                {
                    throw new MyException("新しい名前が既にあります");
                }

                if (!FormUtil.Confirm(this, $"{targetFrom} を {newName} に変更しますか"))
                {
                    throw new MyException("中止");
                }

                Directory.Move(targetFrom, newDir);
                foreach (var proc in GetFolderProcs())
                {
                    proc.StartProc();
                    proc.RenameDirectory(oldName, newName);
                    proc.EndProc();
                }

                TbNewName.Text = "";
                ScanDirList();
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string targetFrom = GetOneSelectedDir();
                if (!Directory.Exists(targetFrom))
                {
                    throw new MyException("元フォルダがありません");
                }

                if (!FormUtil.Confirm(this, $"{targetFrom} を削除しますか"))
                {
                    throw new MyException("中止");
                }

                var oldName = Path.GetFileName(targetFrom);

                Directory.Delete(targetFrom, true);
                foreach (var proc in GetFolderProcs())
                {
                    proc.StartProc();
                    proc.DeleteDirectory(oldName);
                    proc.EndProc();
                }

                ScanDirList();
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private void ChkBlank_CheckedChanged(object sender, EventArgs e)
        {
            if (_folderSelectedValueChangedSection.Running)
            {
                return;
            }

            foreach (string dir in GetSelectedDirs())
            {
                var setting = new FolderPropSetting(dir)
                {
                    Blank = ChkBlank.Checked
                };
                setting.SaveSetting();
            }
        }

        private void ChkNoSmall_CheckedChanged(object sender, EventArgs e)
        {
            if (_folderSelectedValueChangedSection.Running)
            {
                return;
            }

            foreach (string dir in GetSelectedDirs())
            {
                new ConverterFolderSetting(dir)
                {
                    NoSmall = ChkNoSmall.Checked
                };
            }

            foreach (var item in LbFolder.SelectedItems.Cast<FolderInfo>())
            {
                item.UpdateNoSmall();
            }
            UpdateDirListUI(_dirList);
        }
        #endregion

        #region 外部Viewer
        private void LbFolder_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var dirs = GetSelectedDirs();
                if (dirs.Count > 0)
                {
                    OpenViewerProgram(dirs[0]);
                }
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private void SBtnShow_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(AppSetting.Default.FromDir))
                {
                    throw new MyException("変更元フォルダーを選択して下さい");
                }

                var dirs = GetSelectedDirs();
                string target;
                if (dirs.Count > 0)
                {
                    target = dirs[0];
                }
                else
                {
                    target = Directory.EnumerateDirectories(AppSetting.Default.FromDir).Min();
                }
                if (string.IsNullOrEmpty(target))
                {
                    throw new MyException("変更元フォルダーが空です");
                }

                OpenViewerProgram(target);
            }
            catch (MyException ex)
            {
                ShowError(ex);
            }
        }

        private static void OpenViewerProgram(string target)
        {
            if (!File.Exists(AppSetting.Default.ShowProg))
            {
                throw new MyException("プログラムを選択して下さい");
            }
            var pi = new ProcessStartInfo()
            {
                FileName = AppSetting.Default.ShowProg,
                Arguments = target
            };
            Process.Start(pi);
        }
        #endregion

        #region URL
        private void SBtnShow_DropDownOpening(object sender, EventArgs e)
        {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = Path.GetDirectoryName(myAssembly.Location);
            path = Path.Combine(path, "URL");
            if (!Directory.Exists(path))
            {
                return;
            }

            SBtnShow.DropDownItems.Clear();
            foreach (var file in Directory.EnumerateFiles(path, "*.url"))
            {
                var target = file;
                var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
                item.Click += (s1, e1) => FormUtil.OpenFile(target);
                SBtnShow.DropDownItems.Add(item);
            }

            var dirs = GetSelectedDirs();
            if (dirs.Count > 0)
            {
                var doneSet = new HashSet<Uri>();

                SBtnShow.DropDownItems.Add(new ToolStripSeparator());
                foreach (string d in dirs)
                {
                    var setting = new FolderTextSetting(d);
                    var info = setting.GetFolderInfo();
                    if (info is null)
                    {
                        continue;
                    }

                    foreach (string url in info.TitleList)
                    {
                        if (Uri.TryCreate(url, UriKind.Absolute, out Uri targetUri))
                        {
                            if (doneSet.Contains(targetUri))
                            {
                                break;
                            }
                            doneSet.Add(targetUri);

                            var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(setting.FolderTitle));
                            item.Click += (s1, e1) => FormUtil.OpenFile(targetUri.AbsoluteUri);
                            SBtnShow.DropDownItems.Add(item);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        public void ShowError(MyException ex)
        {
            SLbStatus.Text = ex.Message;
        }
    }

    public class MyException(string mess) : Exception(mess)
    {
    }
}
