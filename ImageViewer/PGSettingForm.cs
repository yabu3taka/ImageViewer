using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class PGSettingForm : Form
    {
        public PGSettingForm()
        {
            InitializeComponent();

            LoadSetting();
        }

        private void BTConvSelect_Click(object sender, EventArgs e)
        {
            if (CmdFileDialog.ShowDialog() == DialogResult.OK)
            {
                TBConvCmd.Text = CmdFileDialog.FileName;
            }
        }

        private void CBConvWait_CheckedChanged(object sender, EventArgs e)
        {
            TBConvOutput.Enabled = CBConvWait.Checked;
        }

        private void BTOk_Click(object sender, EventArgs e)
        {
            SaveSetting();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadSetting()
        {
            var setting = AppSetting.Default;
            TBConvCmd.Text = setting.ConvertCommand;
            TBConvParam.Text = setting.ConvertParam;
            TBConvParamWidth.Text = setting.ConvertParamWidth;
            TBConvOutput.Text = setting.ConvertOutput;
            CBConvWait.Checked = setting.ConvertWait.Value;

            TbUrlCmd.Text = setting.UrlCommand;
        }

        private void SaveSetting()
        {
            var setting = AppSetting.Default;
            setting.ConvertCommand = TBConvCmd.Text;
            setting.ConvertParam = TBConvParam.Text;
            setting.ConvertParamWidth = TBConvParamWidth.Text;
            setting.ConvertOutput = TBConvOutput.Text;
            setting.ConvertWait = CBConvWait.Checked;

            setting.UrlCommand = TbUrlCmd.Text;
            setting.Save();
        }

        private void BtnUrlCmd_Click(object sender, EventArgs e)
        {
            if (CmdFileDialog.ShowDialog() == DialogResult.OK)
            {
                TbUrlCmd.Text = CmdFileDialog.FileName;
            }
        }
    }
}
