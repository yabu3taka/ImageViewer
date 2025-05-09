using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    #region SubFolderManager
    class SubFolderManager
    {
        public static FileManager GetFileManager(IFileManager man)
        {
            if (man is FileManager fman)
            {
                return fman;
            }
            if (man is SubFileManager sfman)
            {
                return sfman.ParentManager;
            }
            if (man is FileEmptyManager eman)
            {
                return (FileManager)eman.InternalManager;
            }
            return null;
        }
    }
    #endregion

    #region SubFolderManagerForFileAdd
    class SubFolderInfo : IEquatable<SubFolderInfo>, IComparable<SubFolderInfo>
    {
        public string Id { get; }
        public string Title { get; }

        public SubFolderInfo(string id)
        {
            this.Id = id;
        }

        public SubFolderInfo(string id, string title)
        {
            this.Id = id;
            this.Title = title;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Title) ? Id : $"{Title} ({Id})";
        }

        public override bool Equals(object obj)
        {
            if (obj is SubFolderInfo v)
            {
                return this.Id == v.Id;
            }
            return false;
        }

        public bool Equals(SubFolderInfo other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private static int CompareTitle(string x, string y)
        {
            if (string.IsNullOrEmpty(x))
            {
                if (string.IsNullOrEmpty(y))
                {
                    return 0;
                }
                return 1;
            }
            else
            {
                if (string.IsNullOrEmpty(y))
                {
                    return -1;
                }
                return x.CompareTo(y);
            }
        }

        public int CompareTo(SubFolderInfo y)
        {
            SubFolderInfo x = this;
            int cmp = CompareTitle(x.Title, y.Title);
            if (cmp != 0)
            {
                return cmp;
            }
            return x.Id.CompareTo(y.Id);
        }
    }

    class SubFolderManagerForFileAdd
    {
        private const string PREFIX = "a";

        public FileManager ParentManager { get; }

        private SubFolderManagerForFileAdd(FileManager parentMan)
        {
            ParentManager = parentMan;
        }

        public static SubFolderManagerForFileAdd Create(IFileManager man)
        {
            var fman = SubFolderManager.GetFileManager(man);
            if (fman is null)
            {
                return null;
            }
            return new SubFolderManagerForFileAdd(fman);
        }

        #region SubFolder Util
        private static string[] GetSubFolders(string dir)
        {
            if (Directory.Exists(dir))
            {
                return Directory.GetDirectories(dir, PREFIX + "*");
            }
            else
            {
                return [];
            }
        }

        public static bool IsValidSubFolder(string subfoler)
        {
            return subfoler.StartsWith(PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasSubFolder(string dir)
        {
            return GetSubFolders(dir).Length != 0;
        }
        #endregion

        #region SubFolder
        private int _lastIndex;

        private string[] GetSubFolders()
        {
            return GetSubFolders(ParentManager.FilePath);
        }

        public static string GetTitle(string dirPath)
        {
            var list = FileInfoUtil.ImageExt.GetFileNameList(dirPath);
            if (list.Count == 0)
            {
                return null;
            }
            var style = FileNameUtil.CreateAuto(list[0]);
            return style?.Prefix;
        }

        public List<SubFolderInfo> GetSubFolderList()
        {
            var ret = new List<SubFolderInfo>();
            _lastIndex = 0;

            foreach (string dirPath in GetSubFolders())
            {
                string subfoler = Path.GetFileName(dirPath);
                if (!int.TryParse(subfoler.AsSpan(PREFIX.Length), out int index))
                {
                    continue;
                }

                ret.Add(new SubFolderInfo(subfoler, GetTitle(dirPath)));
                _lastIndex = Math.Max(_lastIndex, index);
            }

            ret.Sort();
            return ret;
        }

        public List<SubFolderInfo> AddNewSubFolder()
        {
            var ret = GetSubFolderList();
            string newFolder = PREFIX + (_lastIndex + 1).ToString("D2");
            Directory.CreateDirectory(ParentManager.Combine(newFolder));

            ret.Add(new SubFolderInfo(newFolder));
            ret.Sort();
            return ret;
        }

        public List<SubFolderInfo> DeleteSubFolder(SubFileManager fman)
        {
            FileUtil.SendToRecycleBin(fman.FilePath);
            return GetSubFolderList();
        }

        public int GetFileCount()
        {
            int ret = 0;
            foreach (string dirPath in GetSubFolders())
            {
                ret += FileInfoUtil.ImageExt.GetFileNameList(dirPath).Count;
            }
            return ret;
        }
        #endregion

        #region Manager
        public SubFileManagerForFileAdd GetSubFileManager(SubFolderInfo subfolder)
        {
            return new SubFileManagerForFileAdd(this, subfolder.Id);
        }

        private SubFileManagerForFileAdd GetSubFileManager(string subfolder)
        {
            return new SubFileManagerForFileAdd(this, subfolder);
        }

        public FileEmptyManager GetEmptyManager()
        {
            return new FileEmptyManager(ParentManager);
        }

        public RealFolderSimple GetSubFolder(SubFolderInfo subfolder)
        {
            return new RealFolderSimple(ParentManager.Combine(subfolder.Id));
        }
        #endregion

        #region Merge/Commit/Clear
        public string MergeFolder => ParentManager.Combine("Merge");

        private FileManager GetMergeManager()
        {
            return new FileManager(MergeFolder);
        }

        public bool Locked => Directory.Exists(MergeFolder);

        public void Unlock()
        {
            FileUtil.DeleteFolder(MergeFolder);
        }

        public IFileMarkerAccessor GetErrorFileMarker()
        {
            var marker = new FileMarkerNameWithoutExtension();

            // 親を保存
            ParentManager.Reload();
            marker.SetAllMark(ParentManager.GetAllItems(), true);

            // エラーをチェック
            var errMarker = new FileMarkerName();
            foreach (var subfolder in GetSubFolders())
            {
                foreach (var finfo in GetSubFileManager(subfolder).GetAllItems())
                {
                    if (marker[finfo])
                    {
                        errMarker[finfo] = true;
                    }
                    marker[finfo] = true;
                }
            }
            return errMarker;
        }

        public IFileMarkerAccessor CreateMargedFolder()
        {
            var marker = GetErrorFileMarker();
            if (marker.Count > 0)
            {
                return marker;
            }

            FileUtil.DeleteFolder(MergeFolder);
            Directory.CreateDirectory(MergeFolder);

            foreach (var subfolder in GetSubFolders())
            {
                foreach (var finfo in GetSubFileManager(subfolder).GetAllItems())
                {
                    finfo.CopyTo(Path.Combine(MergeFolder, finfo.FileName));
                }
            }
            return null;
        }

        public bool Commitable
        {
            get
            {
                var mark = new FileMarkerNameWithoutExtension();
                foreach (var subfolder in GetSubFolders())
                {
                    mark.SetAllMark(GetSubFileManager(subfolder).GetAllItems(), true);
                }

                mark.SetAllMark(GetMergeManager().GetAllItems(), false);
                return mark.Count == 0;
            }
        }

        public IFileActionResult Commit()
        {
            try
            {
                var action = new FileActionTransaction();
                foreach (var finfo in GetMergeManager().GetAllItems())
                {
                    action.Add(new FileRenameAction(finfo, ParentManager));
                }
                return action.Commit();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ClearAll()
        {
            foreach (string subfolder in GetSubFolders())
            {
                Directory.Delete(ParentManager.Combine(subfolder), true);
            }
            FileUtil.DeleteFolder(MergeFolder);
        }
        #endregion
    }
    #endregion

    #region SubEditFolderManager
    class SubFolderManagerForFileEdit
    {
        private const string FOLDER_NAME = "Edit";

        public FileManager ParentManager { get; }

        private SubFolderManagerForFileEdit(FileManager parentMan)
        {
            ParentManager = parentMan;
        }

        public static SubFolderManagerForFileEdit Create(IFileManager man)
        {
            var fman = SubFolderManager.GetFileManager(man);
            if (fman is null)
            {
                return null;
            }
            return new SubFolderManagerForFileEdit(fman);
        }

        #region SubFolder Util
        public static bool HasSubFolder(string dir)
        {
            return Directory.Exists(Path.Combine(dir, FOLDER_NAME));
        }
        #endregion

        #region Manager
        public SubFileManager GetSubFileManager()
        {
            return new SubFileManager(ParentManager, FOLDER_NAME);
        }

        public FileEmptyManager GetEmptyManager()
        {
            return new FileEmptyManager(ParentManager);
        }
        #endregion

        #region Add/Commit/Clear
        public string EditFolder => ParentManager.Combine(FOLDER_NAME);

        public bool HasFolder()
        {
            return Directory.Exists(EditFolder);
        }

        public void AddFileToEditFolder(IFileInfo finfo)
        {
            if (!Directory.Exists(EditFolder))
            {
                Directory.CreateDirectory(EditFolder);
            }
            finfo.CopyTo(Path.Combine(EditFolder, finfo.FileName));
        }

        public void AddFileToEditFolder(IEnumerable<IFileInfo> list)
        {
            if (list is null)
            {
                return;
            }
            if (!Directory.Exists(EditFolder))
            {
                Directory.CreateDirectory(EditFolder);
            }
            foreach (var finfo in list)
            {
                finfo.CopyTo(Path.Combine(EditFolder, finfo.FileName));
            }
        }

        public IFileActionResult Commit()
        {
            try
            {
                var action = new FileActionTransaction();
                foreach (var finfo in GetSubFileManager().GetAllItems())
                {
                    var a = new FileRenameAction(finfo, ParentManager)
                    {
                        OverWrite = true
                    };
                    action.Add(a);
                }
                return action.Commit();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ClearAll()
        {
            FileUtil.DeleteFolder(EditFolder);
        }
        #endregion
    }
    #endregion

    #region SubFileManager
    class SubFileManager : IFileManager, IRealFolder, IFileRealManager,
        IFileDeleteController,
        IFolderInfoController, IFolderSettingController
    {
        private List<string> _list;

        public FileManager ParentManager { get; }

        public SubFileManager(FileManager folder, string dir)
        {
            ParentManager = folder;
            FilePath = folder.Combine(dir);
            LoadDir();
        }

        private void LoadDir()
        {
            _list = FileInfoUtil.ImageExt.GetFileNameList(FilePath);
        }

        #region As IFileManager
        public string Title => FileUtil.GetMiniPath(FilePath);

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
            if (finfo?.MyManager is SubFileManager sfman)
            {
                return FileUtil.EqualsPath(sfman.FilePath, FilePath);
            }
            return false;
        }
        #endregion

        #region As IFileRealManager
        public string FilePath { get; private set; }

        public void Reload()
        {
            LoadDir();
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

        #region As IFolderInfoController
        public IFolderInfo GetFolderInfo()
        {
            return FolderInfoSimple.Create(ParentManager.FilePath);
        }
        #endregion

        #region As IFolderSettingController
        public FolderTextSetting GetFolderTextSetting()
        {
            return new FolderTextSetting(ParentManager.FilePath);
        }
        public FolderPropSetting GetFolderPropSetting()
        {
            return new FolderPropSetting(ParentManager.FilePath);
        }
        #endregion

        #region FileInfo Class
        protected virtual IFileImageInfo GetImageInfo(IFileInfo finfo)
        {
            return FileImageInfoSimple.Create(finfo.FilePath);
        }

        private class MyFileInfo(SubFileManager manager, FileComparerFactory cfac, string name) : IFileInfo, IRealFile
        {
            public IFileManager MyManager => manager;
            public IFileImageInfo GetImageInfo()
            {
                return manager.GetImageInfo(this);
            }

            public int Id { get; } = cfac.GetId(name);

            public string FileName { get; } = name;
            public string FilePath => Path.Combine(manager.FilePath, FileName);
            public string Title => Path.GetFileNameWithoutExtension(FileName);
        }
        #endregion
    }

    class SubFileManagerForFileAdd : SubFileManager
    {
        private readonly string _mergeFolder;

        public SubFileManagerForFileAdd(SubFolderManagerForFileAdd fman, string dir)
            : base(fman.ParentManager, dir)
        {
            _mergeFolder = fman.MergeFolder;
        }

        protected override IFileImageInfo GetImageInfo(IFileInfo finfo)
        {
            string target;
            if (Directory.Exists(_mergeFolder))
            {
                target = FileInfoUtil.ImageExt.GetSuportedFile(_mergeFolder, finfo.FileName);
                if (string.IsNullOrEmpty(target))
                {
                    return null;
                }
            }
            else
            {
                target = finfo.FilePath;
            }
            return FileImageInfoSimple.Create(target);
        }
    }
    #endregion
}
