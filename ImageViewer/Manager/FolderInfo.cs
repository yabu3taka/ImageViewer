using System;
using System.Collections.Generic;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFolderInfoController
    {
        IFolderInfo GetFolderInfo();
    }

    interface IFolderInfo
    {
        int Count { get; }
        IEnumerable<string> TitleList { get; }
        string ToolTip { get; }
        void OpenDirInfo();
        void OpenDirInfo(int i);
    }

    class FolderInfoSimple : IFolderInfo
    {
        private readonly List<string> _files;

        private FolderInfoSimple(List<string> files)
        {
            _files = files;
        }

        public static FolderInfoSimple Create(string dir)
        {
            var targetFileList = GetTargetFile(dir);
            if (targetFileList.Count == 0)
            {
                return null;
            }
            return new FolderInfoSimple(targetFileList);
        }

        private static List<string> GetTargetFile(string dir)
        {
            var ret = new List<string>();
            if (Directory.Exists(dir))
            {
                ret.AddRange(Directory.GetDirectories(dir, "*.url"));
                ret.Sort();
            }
            return ret;
        }

        public int Count => _files.Count;

        public IEnumerable<string> TitleList =>
            _files.ConvertAll(Path.GetFileNameWithoutExtension);

        public string ToolTip =>
            string.Join(Environment.NewLine, _files.ConvertAll(Path.GetFileNameWithoutExtension));

        public void OpenDirInfo()
        {
            OpenDirInfo(0);
        }

        public void OpenDirInfo(int i)
        {
            FormUtil.OpenFile(_files[i]);
        }
    }
}
