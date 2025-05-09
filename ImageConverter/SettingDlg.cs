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

using ImageConverter.Util;

namespace ImageConverter
{
    public partial class SettingDlg : Form
    {
        public SettingDlg()
        {
            InitializeComponent();
        }

        private void SettingDlg_Load(object sender, EventArgs e)
        {
            LoadSetting();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            string mess = SaveSetting();
            if (mess is not null)
            {
                FormUtil.ShowError(this, mess);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static int ToInt(NumericUpDown c)
        {
            return (int)c.Value;
        }

        private void LoadSetting()
        {
            var setting = AppSetting.Default;
            TbFromDir.Text = setting.FromDir;
            TbToDir.Text = setting.ToDir;

            TbLightWeightToDir.Text = setting.LightWeightToDir;
            TbLightWeightQuality.Value = setting.LightWeightQuality.Value;
            TbLightWeightWidth.Value = setting.LightWeightWidth.Value;
            TbLightWeightHeight.Value = setting.LightWeightHeight.Value;

            TbGDriveNormalToDir.Text = setting.GDriveNormalToDir;
            TbGDriveLightToDir.Text = setting.GDriveLightToDir;

            TbTabletDir.Text = setting.TabletToDir;

            TbProg.Text = setting.ShowProg;
            TbUrlCmd.Text = setting.UrlCommand;
        }

        private string SaveSetting()
        {
            var setting = AppSetting.CreateEditData();
            setting.FromDir = TbFromDir.Text;
            setting.ToDir = TbToDir.Text;

            setting.LightWeightToDir = TbLightWeightToDir.Text;
            setting.LightWeightQuality = ToInt(TbLightWeightQuality);
            setting.LightWeightWidth = ToInt(TbLightWeightWidth);
            setting.LightWeightHeight = ToInt(TbLightWeightHeight);

            setting.GDriveNormalToDir = TbGDriveNormalToDir.Text;
            setting.GDriveLightToDir = TbGDriveLightToDir.Text;

            setting.TabletToDir = TbTabletDir.Text;

            setting.ShowProg = TbProg.Text;
            setting.UrlCommand = TbUrlCmd.Text;

            string mess = setting.Validate();
            if (mess is not null)
            {
                return mess;
            }

            AppSetting.CommitAndSave(setting);
            return null;
        }

        private static void SelectFolder(TextBox c)
        {
            using FolderBrowserDialog dlg = new();
            dlg.SelectedPath = c.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                c.Text = dlg.SelectedPath;
            }
        }

        private void BtnFromDir_Click(object sender, EventArgs e)
        {
            SelectFolder(TbFromDir);
        }

        private void BtnToDir_Click(object sender, EventArgs e)
        {
            SelectFolder(TbToDir);
        }

        private void BtnLightWeightDir_Click(object sender, EventArgs e)
        {
            SelectFolder(TbLightWeightToDir);
        }

        private void BtnProg_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dlg = new();
            dlg.FileName = TbProg.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TbProg.Text = dlg.FileName;
            }
        }

        private void BtnUrlCmd_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dlg = new();
            dlg.FileName = TbUrlCmd.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TbUrlCmd.Text = dlg.FileName;
            }
        }
    }
}
