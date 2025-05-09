using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFileConvertCmd
    {
        string Title { get; }
        bool Run(string folder);
    }

    class FileConvertExe(string command, string param) : IFileConvertCmd
    {
        public string Title => "変換";

        public string Command { get; } = command;
        public string Param { get; } = param;

        public bool Run(string folder)
        {
            string rp = Param.Replace("{}", $"\"{folder}\"");
            var psi = new ProcessStartInfo(Command, rp)
            {
                UseShellExecute = false
            };

            var p = Process.Start(psi);
            return DoAfterProcess(p, folder);
        }

        protected virtual bool DoAfterProcess(Process p, string folder)
        {
            return true;
        }

        public static FileConvertExe CreateFromSetting()
        {
            var setting = AppSetting.Default;
            if (string.IsNullOrEmpty(setting.ConvertCommand))
            {
                return null;
            }
            return CreateFromSettingInternal(setting, setting.ConvertParam);
        }

        public static FileConvertExe CreateFromSettingForWidth()
        {
            var setting = AppSetting.Default;
            if (string.IsNullOrEmpty(setting.ConvertCommand))
            {
                return null;
            }
            return CreateFromSettingInternal(setting, setting.ConvertParamWidth);
        }

        private static FileConvertExe CreateFromSettingInternal(AppSetting setting, String param)
        {
            FileConvertExe ret;
            if (setting.ConvertWait.Value)
            {
                ret = new FileConvertExeWait(setting.ConvertCommand, param, setting.ConvertOutput);
            }
            else
            {
                ret = new FileConvertExe(setting.ConvertCommand, param);
            }
            return ret;
        }
    }

    class FileConvertExeWait(string command, string param, string output) : FileConvertExe(command, param)
    {
        public string Output { get; } = output;

        protected override bool DoAfterProcess(Process p, string folder)
        {
            p.WaitForExit();
            p.Close();

            string fromFolder = Path.Combine(folder, Output);
            if (!Directory.Exists(fromFolder))
            {
                return false;
            }

            string toFolder = folder;
            foreach (string target in FileInfoUtil.ImageExt.GetFilePathList(fromFolder))
            {
                FileInfoUtil.ImageExt.OverwriteMove(target, toFolder);
            }
            return true;
        }
    }

    class FileConvertExplorer : IFileConvertCmd
    {
        public string Title => "エクスプローラで開く";

        public bool Run(string folder)
        {
            FormUtil.OpenFile(folder);
            return true;
        }
    }
}
