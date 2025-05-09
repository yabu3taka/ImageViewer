using System;
using System.Collections.Generic;
using System.IO;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFileAction
    {
        bool Commitable { get; }
        FileActionResult Commit();
    }

    #region Dir Action
    class FileNewDirAction(string targetPath) : IFileAction
    {
        public bool Commitable => !Directory.Exists(targetPath);

        public FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                Directory.CreateDirectory(targetPath);
                result.AddNewDirAction(targetPath);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    class FileRenameDirAction(IRealFolder fromInfo, string toPath) : IFileAction
    {
        public string FromFilePath { get; } = fromInfo.FilePath;
        public string ToFilePath { get; } = toPath;

        public bool Same => FileUtil.EqualsPath(FromFilePath, ToFilePath);

        public bool Commitable => !Directory.Exists(ToFilePath);

        public FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                if (!Same)
                {
                    Directory.Move(FromFilePath, ToFilePath);
                    result.AddRenameDirAction(FromFilePath, ToFilePath);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #endregion

    #region Rename Action
    abstract class FileRenameActionBase(string fromPath, string toPath) : IFileAction
    {
        public FileActionInfo FromFile { get; } = new(fromPath);
        public FileActionInfo ToFile { get; } = new(toPath);
        public FileActionInfo TmpFile => ToFile.CreateTmp();

        public bool Same => FromFile.IsSame(ToFile);

        public abstract bool Commitable { get; }

        public abstract FileActionResult Commit();
    }

    class FileCopyAction : FileRenameActionBase
    {
        public FileCopyAction(IFileInfo fromInfo, IRealFolder folder, string toName) :
            base(fromInfo.FilePath, folder.Combine(toName))
        {
        }

        public FileCopyAction(IFileInfo fromInfo, IRealFolder folder) :
            this(fromInfo, folder, fromInfo.FileName)
        {
        }

        public FileCopyAction(IRealFile fromInfo, IRealFolder folder, string toName) :
            base(fromInfo.FilePath, folder.Combine(toName))
        {
        }

        public FileCopyAction(IRealFile fromInfo, IRealFolder folder) :
            this(fromInfo, folder, fromInfo.FileName)
        {
        }

        public bool OverWrite { get; set; } = false;

        public override bool Commitable
        {
            get
            {
                if (OverWrite)
                {
                    return true;
                }
                return !ToFile.Exists();
            }
        }

        public override FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                if (!Same)
                {
                    if (OverWrite)
                    {
                        ToFile.SendToRecycleBin();
                    }
                    FromFile.Copy(ToFile);
                    result.AddNewFileAction(ToFile);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    class FileRenameAction : FileRenameActionBase
    {
        public FileRenameAction(IFileInfo fromInfo, IRealFolder folder, string toName) :
            base(fromInfo.FilePath, folder.Combine(toName))
        {
        }

        public FileRenameAction(IFileInfo fromInfo, IRealFolder folder) :
            this(fromInfo, folder, fromInfo.FileName)
        {
        }

        public FileRenameAction(IRealFile fromInfo, IRealFolder folder, string toName) :
            base(fromInfo.FilePath, folder.Combine(toName))
        {
        }

        public FileRenameAction(IRealFile fromInfo, IRealFolder folder) :
            this(fromInfo, folder, fromInfo.FileName)
        {
        }

        public bool OverWrite { get; set; } = false;

        public override bool Commitable
        {
            get
            {
                if (OverWrite)
                {
                    return true;
                }
                return !ToFile.Exists();
            }
        }

        public override FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                if (!Same)
                {
                    if (OverWrite)
                    {
                        ToFile.SendToRecycleBin();
                    }
                    FromFile.Move(ToFile);
                    result.AddRenameAction(FromFile, ToFile);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    class FileSwapAction(IFileInfo fromInfo, IFileInfo toInfo) :
        FileRenameActionBase(fromInfo.FilePath, toInfo.FilePath)
    {
        public override bool Commitable => !Same;

        public override FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                if (!Same)
                {
                    FromFile.Move(TmpFile);
                    ToFile.Move(FromFile);
                    TmpFile.Move(ToFile);
                    result.AddRenameAction(FromFile, ToFile);
                    result.AddRenameAction(ToFile, FromFile);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    class FileActionSerialRename : IFileAction
    {
        private readonly List<FileRenameAction> _allActionList = [];
        public IEnumerable<FileRenameAction> RenameItems => _allActionList;

        public void Add(FileRenameAction a)
        {
            if (!a.Same)
            {
                _actionList.Add(a);
            }
            _allActionList.Add(a);
        }

        public void Merge(FileActionSerialRename trans)
        {
            foreach (var act in trans._allActionList)
            {
                Add(act);
            }
        }

        private readonly List<FileRenameAction> _actionList = [];

        public bool Commitable
        {
            get
            {
                var okhash = FileUtil.CreateHashSet();
                foreach (var action in _actionList)
                {
                    okhash.Add(action.FromFile.ImageFilePath);
                }
                foreach (var action in _actionList)
                {
                    if (!okhash.Contains(action.ToFile.ImageFilePath))
                    {
                        if (File.Exists(action.ToFile.ImageFilePath))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                foreach (var action in _actionList)
                {
                    action.FromFile.Move(action.TmpFile);
                }
                foreach (var action in _actionList)
                {
                    action.TmpFile.Move(action.ToFile);
                    result.AddRenameAction(action.FromFile, action.ToFile);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #endregion

    #region Delete Action
    abstract class FileDeletionAction(IFileInfo finfo) : IFileAction
    {
        private readonly FileActionInfo _targetFile = new(finfo.FilePath);

        public bool Commitable => true;

        public FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                if (!DeleteFile(_targetFile))
                {
                    return null;
                }
                result.AddDeleteAction(_targetFile);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected abstract bool DeleteFile(FileActionInfo path);
    }

    class FileDeletionActionRecycle(IFileInfo finfo) : FileDeletionAction(finfo)
    {
        protected override bool DeleteFile(FileActionInfo actionFile)
        {
            return actionFile.SendToRecycleBin();
        }
    }
    #endregion

    #region Transaction
    class FileActionTransaction : IFileAction
    {
        private readonly List<IFileAction> _actions = [];

        public void Add(IFileAction a)
        {
            _actions.Add(a);
        }

        public bool Commitable
        {
            get
            {
                return _actions.TrueForAll(a => a.Commitable);
            }
        }

        public FileActionResult Commit()
        {
            try
            {
                var result = new FileActionResult();
                foreach (var action in _actions)
                {
                    result.Merge(action.Commit());
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    #endregion

    #region Action Result
    interface IFileActionReloader
    {
        void Reload(IFileRealManager fman);
        void ForceReload(IFileRealManager fman);
    }

    interface IFileActionResult
    {
        IFileActionUpdater CreateUpdater();
    }

    interface IFileActionUpdater
    {
        void Update(FilePosCursor c);
    }

    class FileActionReloader : IFileActionReloader, IFileActionResult, IFileActionUpdater
    {
        private readonly HashSet<string> _doneFolder = FileUtil.CreateHashSet();

        public void Reload(IFileRealManager fman)
        {
            if (!_doneFolder.Contains(fman.FilePath))
            {
                ForceReload(fman);
            }
        }

        public void ForceReload(IFileRealManager fman)
        {
            _doneFolder.Add(fman.FilePath);
            fman.Reload();
        }

        #region IFileActionResult/IFileActionUpdater
        public IFileActionUpdater CreateUpdater()
        {
            return this;
        }

        public void Update(FilePosCursor c)
        {
            if (UpdateManager(c.GetMyManager()))
            {
                UpdateCursor(c);
            }
        }

        private bool UpdateManager(IFileManager man)
        {
            if (man is IFileRealManager rlman)
            {
                if (_doneFolder.Contains(rlman.FilePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static void UpdateCursor(FilePosCursor c)
        {
            c.Reload();
            c.GoToNearPos();
        }
        #endregion
    }

    class FileActionResult : IFileActionResult, IFileActionUpdater
    {
        #region File Add Result
        private readonly HashSet<string> _ruleAdd = FileUtil.CreateHashSet();

        public void AddNewFileAction(FileActionInfo fromFile)
        {
            _ruleAdd.Add(fromFile.ImageFilePath);
        }
        #endregion

        #region File Result
        private readonly Dictionary<string, string> _rule = FileUtil.CreateDictionary<string>();

        public void AddRenameAction(FileActionInfo fromFile, FileActionInfo toFile)
        {
            _rule.Add(fromFile.ImageFilePath, toFile.ImageFilePath);
        }

        public void AddDeleteAction(FileActionInfo file)
        {
            _rule.Add(file.ImageFilePath, null);
        }
        #endregion

        #region Folder Result
        private readonly Dictionary<string, string> _dirRule = FileUtil.CreateDictionary<string>();

        public void AddRenameDirAction(string fromFile, string toFile)
        {
            _dirRule.Add(fromFile, toFile);
        }

        public void AddNewDirAction(string targetFile)
        {
            _dirRule.Add(targetFile, null);
        }
        #endregion

        #region Action Result
        public bool HasSuccess
        {
            get
            {
                if (_rule.Count > 0)
                {
                    return true;
                }
                if (_dirRule.Count > 0)
                {
                    return true;
                }
                if (_ruleAdd.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public void Merge(FileActionResult result)
        {
            if (result is null)
            {
                return;
            }

            foreach (var pair in result._rule)
            {
                _rule[pair.Key] = pair.Value;
            }

            foreach (var pair in result._dirRule)
            {
                _dirRule[pair.Key] = pair.Value;
            }

            foreach (string str in result._ruleAdd)
            {
                _ruleAdd.Add(str);
            }
        }

        private HashSet<string> GetUpdatedDirs()
        {
            var mark = FileUtil.CreateHashSet();

            foreach (var pair in _rule)
            {
                if (pair.Key is not null)
                {
                    mark.Add(Path.GetDirectoryName(pair.Key));
                }
                if (pair.Value is not null)
                {
                    mark.Add(Path.GetDirectoryName(pair.Value));
                }
            }

            foreach (var pair in _dirRule)
            {
                if (pair.Key is not null)
                {
                    mark.Add(Path.GetDirectoryName(pair.Key));
                }
                if (pair.Value is not null)
                {
                    mark.Add(Path.GetDirectoryName(pair.Value));
                }
            }

            foreach (string str in _ruleAdd)
            {
                mark.Add(Path.GetDirectoryName(str));
            }

            return mark;
        }

        public void DoCommit(IFileAction action)
        {
            Merge(action.Commit());
        }
        #endregion

        #region IFileActionResult/IFileActionUpdater
        private FileActionReloader _reloader;
        private HashSet<string> _updatedDir;

        public IFileActionUpdater CreateUpdater()
        {
            if (!HasSuccess)
            {
                return null;
            }
            _reloader = new FileActionReloader();
            _updatedDir = GetUpdatedDirs();
            return this;
        }

        public void Update(FilePosCursor c)
        {
            if (UpdateManager(c.GetMyManager()))
            {
                UpdateCursor(c);
            }
        }

        private bool UpdateManager(IFileManager man)
        {
            if (man is IFileRealManager rlman)
            {
                if (_dirRule.TryGetValue(rlman.FilePath, out string value))
                {
                    rlman.NotifyFolderRenamed(value, _reloader);
                    return true;
                }
                if (_updatedDir.Contains(rlman.FilePath))
                {
                    _reloader.Reload(rlman);
                    return true;
                }
            }

            return false;
        }

        private void UpdateCursor(FilePosCursor c)
        {
            c.Reload();

            var finfo = c.Current;
            if (finfo is null)
            {
                c.GoToNearPos();
            }
            else if (finfo is IRealFolder folder)
            {
                if (_dirRule.TryGetValue(finfo.FilePath, out string topath))
                {
                    if (!c.MoveTo(topath))
                    {
                        c.GoToNearPos();
                    }
                    c.NotifyBitmapChange();
                }
                else
                {
                    if (_updatedDir.Contains(folder.FilePath))
                    {
                        c.NotifyBitmapChange();
                    }
                }
            }
            else
            {
                if (_rule.TryGetValue(finfo.FilePath, out string topath))
                {
                    if (string.IsNullOrEmpty(topath))
                    {
                        c.GoToNearPos();
                    }
                    else
                    {
                        if (!c.MoveTo(topath))
                        {
                            c.GoToNearPos();
                        }
                    }
                }
            }
        }
        #endregion
    }
    #endregion

    #region Mark Update
    enum FileActionMark { Delete, Index }

    class FileActionMarkReloader : IFileActionResult, IFileActionUpdater
    {
        private readonly HashSet<string> _doneFolder = FileUtil.CreateHashSet();

        public void Reload(IFileRealManager fman)
        {
            if (!_doneFolder.Contains(fman.FilePath))
            {
                ForcedReload(fman);
            }
        }

        public void ForcedReload(IFileRealManager fman)
        {
            _doneFolder.Add(fman.FilePath);
            fman.Reload();
        }

        public FileActionMark MarkType { get; private set; }

        public static IFileActionResult Create(IFileManager fman, FileActionMark markType)
        {
            var ret = new FileActionMarkReloader();
            if (fman is IFileRealManager rlman)
            {
                ret.Reload(rlman);
            }
            ret.MarkType = markType;
            return ret;
        }

        public IFileActionUpdater CreateUpdater()
        {
            return this;
        }

        public void Update(FilePosCursor c)
        {
            if (UpdateManager(c.GetMyManager()))
            {
                UpdateCursor(c);
            }
        }

        private bool UpdateManager(IFileManager man)
        {
            if (man is IFileRealManager rlman)
            {
                if (_doneFolder.Contains(rlman.FilePath))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateCursor(FilePosCursor c)
        {
            FileFilterType filter = MarkType switch
            {
                FileActionMark.Index => FileFilterType.Index,
                FileActionMark.Delete => FileFilterType.Delete,
                _ => throw new ArgumentOutOfRangeException("Invalid ", nameof(MarkType))
            };
            if (c.FileList.FilterType == filter)
            {
                c.Reload();
                c.GoToNearPos();
            }
        }
    }
    #endregion

    #region Internal
    class FileActionInfo
    {
        private const string TMP_EXT = "-xx";

        public readonly string ImageFilePath;
        private readonly string _parentFolder;
        private readonly string _fileName;
        private readonly bool _tmpFlag;

        public FileActionInfo(string filepath) : this(filepath, false)
        {
        }

        private FileActionInfo(string filepath, bool tmp)
        {
            _parentFolder = Path.GetDirectoryName(filepath);
            _fileName = Path.GetFileNameWithoutExtension(filepath);
            _tmpFlag = tmp;

            ImageFilePath = filepath + (tmp ? TMP_EXT : "");
        }

        public FileActionInfo CreateTmp()
        {
            return new FileActionInfo(ImageFilePath, true);
        }

        private string GetFilePath(string ext)
        {
            return Path.Combine(_parentFolder, _fileName + ext + (_tmpFlag ? TMP_EXT : ""));
        }

        private string TextFilePath => GetFilePath(".txt");

        public void Move(FileActionInfo toFile)
        {
            File.Move(ImageFilePath, toFile.ImageFilePath);

            if (File.Exists(TextFilePath))
            {
                File.Move(TextFilePath, toFile.TextFilePath);
            }
        }

        public void Copy(FileActionInfo toFile)
        {
            File.Copy(ImageFilePath, toFile.ImageFilePath);

            if (File.Exists(TextFilePath))
            {
                File.Copy(TextFilePath, toFile.TextFilePath);
            }
        }

        public bool SendToRecycleBin()
        {
            if (File.Exists(TextFilePath))
            {
                FileUtil.SendToRecycleBin(TextFilePath);
            }

            return FileUtil.SendToRecycleBin(ImageFilePath);
        }

        public bool Exists()
        {
            return File.Exists(ImageFilePath);
        }

        public bool IsSame(FileActionInfo toFile)
        {
            return FileUtil.EqualsPath(ImageFilePath, toFile.ImageFilePath);
        }
    }
    #endregion
}
