using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageConverter.PortableDevices;
using ImageConverter.Util;
using ImageConverter.Work;

namespace ImageConverter
{
    public partial class DeviceDlg : Form
    {
        private readonly List<PortableDevice> _deviceList;
        private readonly string _toDir;

        public DeviceDlg(List<PortableDevice> deviceList, string toDir)
        {
            InitializeComponent();
            _deviceList = deviceList;
            _toDir = toDir;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (CmbDevice.SelectedItem is null)
            {
                FormUtil.ShowError(this, "選択してください");
                return;
            }

            if (!OkDevice(Selected))
            {
                FormUtil.ShowError(this, "パスワードファイルがない端末です");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkDevice(PortableDevice d)
        {
            return new WpdWorkProcessorCopy(d, _toDir).HasPassFile();
        }

        private void DeviceDlg_Load(object sender, EventArgs e)
        {
            foreach (var d in _deviceList)
            {
                CmbDevice.Items.Add(new Item(d));
            }

            int i = _deviceList.FindIndex(OkDevice);
            if (i >= 0)
            {
                CmbDevice.SelectedIndex = i;
            }
        }

        public PortableDevice Selected => ((Item)CmbDevice.SelectedItem).Device;

        private class Item
        {
            public PortableDevice Device { get; }

            private readonly string _name;

            public Item(PortableDevice device)
            {
                Device = device;
                using var c = device.Connect();
                _name = $"{c.FriendlyName}({device.DeviceId})";
            }

            public override string ToString()
            {
                return _name;
            }
        }
    }
}
