using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    class FileDirManager(string dir) : IFileManager, IFileRealManager
    {
        public IFileMarkerAccessor GetFileAddDirMarker()
        {
            var marker = new FileMarkerName();
            var dirs = Directory.EnumerateDirectories(FilePath);
            foreach (string dir in dirs)
            {
                var finfo = new MyFileInfo(this, Path.GetFileName(dir));
                if (SubFolderManagerForFileAdd.HasSubFolder(finfo.FilePath))
                {
                    marker[finfo] = true;
                }
            }
            return marker;
        }

        #region As IFileManager
        public string Title => Path.GetFileName(FilePath);

        public int ForEach(Action<IFileInfo> a, out IFileComparer defComparer)
        {
            var dirs = Directory.GetDirectories(FilePath);
            foreach (string dir in dirs)
            {
                a(new MyFileInfo(this, Path.GetFileName(dir)));
            }
            defComparer = new FileComparerSimple();
            return dirs.Length;
        }

        public bool IsMime(IFileInfo finfo)
        {
            if (finfo?.MyManager is FileDirManager dman)
            {
                return FileUtil.EqualsPath(dman.FilePath, FilePath);
            }
            return false;
        }
        #endregion

        #region As IFileRealManager
        public string FilePath { get; } = dir;

        public void Reload()
        {
        }

        public void NotifyFolderRenamed(string newDir, IFileActionReloader reloader)
        {
        }
        #endregion

        #region FileInfo Class
        private class MyFileInfo(FileDirManager manager, string name) : IFileInfo, IFileDetail, IRealFolder,
            IFolderSettingController, IFolderInfoController
        {
            public IFileManager MyManager => manager;
            public IFileImageInfo GetImageInfo()
            {
                var fman = new FileManager(FilePath);
                return MyFileImageInfo.Create(fman);
            }

            public int Id => -1;

            public string FileName { get; } = name;
            public string FilePath => Path.Combine(manager.FilePath, FileName);

            private string _folderTitle = "";

            public string Title
            {
                get
                {
                    string postfix = string.IsNullOrEmpty(_folderTitle) ? "" : $" ({_folderTitle})";
                    return Path.GetFileNameWithoutExtension(FileName) + postfix;
                }
            }
            public string DetailTitle { get { return _folderTitle; } }

            public void LoadDetailedTitle()
            {
                _folderTitle = GetFolderTextSetting().FolderTitle;
            }

            #region As IFolderInfoController
            public IFolderInfo GetFolderInfo()
            {
                return FileManager.GetFolderInfo(FilePath);
            }
            #endregion

            #region As IFolderSettingController
            public FolderTextSetting GetFolderTextSetting()
            {
                return new FolderTextSetting(FilePath);
            }
            public FolderPropSetting GetFolderPropSetting()
            {
                return new FolderPropSetting(FilePath);
            }
            #endregion
        }

        private class MyFileImageInfo : IFileImageInfo
        {
            private MyFileImageInfo(Bitmap img, string filesize)
            {
                this.FileSize = filesize;
                this.Bitmap = img;
            }

            public static IFileImageInfo Create(FileManager fman)
            {
                var list = fman.GetAllItems();
                var img = list.GetFirst()?.GetBitmap();
                if (img is null)
                {
                    return null;
                }
                return new MyFileImageInfo(img, $"{list.Count}個");
            }

            public Bitmap Bitmap { get; }
            public Size ImageSize => Bitmap.Size;
            public string FileSize { get; }
        }
        #endregion
    }
}
