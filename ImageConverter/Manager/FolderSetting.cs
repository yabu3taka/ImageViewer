using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ImageConverter.Util;

namespace ImageConverter.Manager
{
    interface IFolderSettingController
    {
        FolderTextSetting GetFolderTextSetting();
        FolderPropSetting GetFolderPropSetting();
    }

    class FolderPropSetting
    {
        private const string SETTING_BLANK = "BLANK";

        private readonly string _settingFile;

        public FolderPropSetting(string dir)
        {
            _settingFile = Path.Combine(dir, "setting.dat");
            LoadSetting();
        }

        #region Setting
        public bool Blank { get; set; } = false;

        private void LoadSetting()
        {
            if (File.Exists(_settingFile))
            {
                foreach (string line in File.ReadLines(_settingFile, Encoding.UTF8))
                {
                    if (SETTING_BLANK == line)
                    {
                        Blank = true;
                    }
                }
            }
        }

        public void SaveSetting()
        {
            var output = new StringBuilder();

            if (Blank)
            {
                output.AppendLine(SETTING_BLANK);
            }

            FileUtil.WriteOrDeleteFile(_settingFile, output.ToString());
        }
        #endregion
    }

    class FolderTextSetting
    {
        private readonly string _folderTextFile;

        public FolderTextSetting(string dir)
        {
            _folderTextFile = Path.Combine(dir, "folder.txt");
            LoadFolderText();
        }

        public string FolderText { get; set; } = "";

        private string[] _titles;
        public string TitlesText
        {
            get { return string.Join(Environment.NewLine, _titles); }
            set
            {
                string[] deli = [Environment.NewLine];
                _titles = value.Split(deli, StringSplitOptions.RemoveEmptyEntries).
                    Select(f => f.Trim()).ToArray();
            }
        }

        public string FolderTitle
        {
            get
            {
                if (_titles.Length == 0)
                {
                    return "";
                }
                return _titles[0];
            }
        }

        public FolderInfoText GetFolderInfo()
        {
            if (_titles.Length == 0)
            {
                return null;
            }
            return new FolderInfoText(_titles);
        }

        #region Load/Save
        private const string START_TITLE = "==Start:Titles";
        private const string END_TITLE = "==End:Titles";

        private void LoadFolderText()
        {
            var titles = new StringBuilder();
            var mess = new StringBuilder();
            var target = mess;

            if (File.Exists(_folderTextFile))
            {
                foreach (string line in File.ReadLines(_folderTextFile, Encoding.UTF8))
                {
                    if (line == START_TITLE)
                    {
                        target = titles;
                    }
                    else if (line == END_TITLE)
                    {
                        target = mess;
                    }
                    else
                    {
                        target.AppendLine(line);
                    }
                }
            }

            FolderText = mess.ToString();
            TitlesText = titles.ToString();
        }

        public void SaveSetting()
        {
            var output = new StringBuilder();

            if (_titles.Length > 0)
            {
                output.AppendLine(START_TITLE);
                foreach (string title in _titles)
                {
                    output.AppendLine(title);
                }
                output.AppendLine(END_TITLE);
            }

            if (!string.IsNullOrEmpty(FolderText))
            {
                output.Append(FolderText);
            }

            FileUtil.WriteOrDeleteFile(_folderTextFile, output.ToString());
        }
        #endregion
    }

    class FolderInfoText(string[] titles)
    {
        public int Count => titles.Length;
        public IEnumerable<string> TitleList => titles;
        public string ToolTip => string.Join(Environment.NewLine, titles);

        public void OpenDirInfo()
        {
            foreach (string str in titles)
            {
                if (Uri.TryCreate(str, UriKind.Absolute, out _))
                {
                    FormUtil.OpenFile(str);
                    return;
                }
            }
            OpenDirInfo(0);
        }

        public void OpenDirInfo(int i)
        {
            OpenStr(titles[i].Trim());
        }

        private static void OpenStr(string str)
        {
            if (Uri.TryCreate(str, UriKind.Absolute, out _))
            {
                FormUtil.OpenFile(str);
            }
            else
            {
                Clipboard.SetText(str);
            }
        }
    }
}
