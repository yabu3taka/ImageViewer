using System;
using System.Collections.Generic;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    class FileManager : IFileManager, IRealFolder, IFileRealManager, 
        IFileIndexController, IFileDeleteController, IFileCutAndPasteController, IFileGroupController,
        IFolderInfoController, IFolderSettingController
    {
        private List<string> _list;

        public FileManager(string dir)
        {
            FilePath = dir;
            LoadDir();
            ReloadIndex();
        }

        private void LoadDir()
        {
            _list = FileInfoUtil.ImageExt.GetFileNameList(FilePath);
            _groupMarker = null;
        }

        #region As IFileManager
        public string Title => Path.GetFileName(FilePath);

        public int ForEach(Action<IFileInfo> a, out IFileComparer defComparer)
        {
            var cfac = new FileComparerFactory();
            foreach (var name in _list)
            {
                a(new MyFileInfo(this, cfac, name));
            }
            defComparer = cfac.Create();
            return _list.Count;
        }

        public bool IsMime(IFileInfo finfo)
        {
            if (finfo?.MyManager is FileManager fman)
            {
                return FileUtil.EqualsPath(fman.FilePath, FilePath);
            }
            return false;
        }
        #endregion

        #region As IFileRealManager
        public string FilePath { get; private set; }

        public void Reload()
        {
            LoadDir();
            _indexMarker.RemoveMarkNotInList(_list);
        }

        public void NotifyFolderRenamed(string newDir, IFileActionReloader reloader)
        {
            FilePath = newDir;
            reloader.ForceReload(this);
        }
        #endregion

        #region As IFileDeleteController
        public IFileMarker CreateDeleteMarker()
        {
            return new FileMarkerName();
        }

        public bool IsDeletable(IFileInfo finfo)
        {
            return true;
        }
        #endregion

        #region As IFileCutAndPasteController
        public bool IsPastable(IFileInfo finfo)
        {
            return true;
        }
        #endregion

        #region As IFileIndexController
        private readonly FileMarkerName _indexMarker = new();

        private string IndexFile => Path.Combine(FilePath, "index.dat");

        public IFileMarker IndexMarker => _indexMarker;

        public void ReloadIndex()
        {
            _indexMarker.LoadMaker(IndexFile);
        }

        public void SaveIndex()
        {
            _indexMarker.SaveMaker(IndexFile, this);
        }
        #endregion

        #region As IFileGroupController
        private FileMarkerName _groupMarker = null;

        public IFileMarkerAccessor GetGroupMarker()
        {
            if (_groupMarker is null)
            {
                _groupMarker = new FileMarkerName();
                _groupMarker.SetAllMark(FileInfoUtil.GetAllItems(this).GetFileGroupList(), true);
            }
            return _groupMarker;
        }
        #endregion

        #region SubAddFolderManager/SubEditFolderManager
        public bool HasSubAddFolder()
        {
            return SubFolderManagerForFileAdd.HasSubFolder(FilePath);
        }

        public bool HasSubEditFolder()
        {
            return SubFolderManagerForFileEdit.HasSubFolder(FilePath);
        }
        #endregion

        #region As IFolderInfoController
        public IFolderInfo GetFolderInfo()
        {
            return GetFolderInfo(FilePath);
        }

        public static IFolderInfo GetFolderInfo(string file)
        {
            IFolderInfo ret = new FolderTextSetting(file).GetFolderInfo();
            ret ??= FolderInfoSimple.Create(file);
            return ret;
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

        #region FileInfo Class
        private class MyFileInfo(FileManager manager, FileComparerFactory cfac, string name) : IFileInfo, IFileText, IRealFile
        {
            public IFileManager MyManager => manager;
            public IFileImageInfo GetImageInfo()
            {
                return FileImageInfoSimple.Create(FilePath);
            }

            public int Id { get; } = cfac.GetId(name);

            public string FileName { get; } = name;
            public string FilePath => manager.Combine(FileName);
            public string Title => Path.GetFileNameWithoutExtension(FileName);

            public string PhotoTextPath =>
                manager.Combine(FileUtil.ChangeExt(FileName, ".txt"));

            public string FileText
            {
                get { return FileUtil.ReadFile(PhotoTextPath); }
                set { FileUtil.WriteOrDeleteFile(PhotoTextPath, value); }
            }
        }
        #endregion
    }
}
