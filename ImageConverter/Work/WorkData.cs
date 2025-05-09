using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using ImageConverter.Util;

namespace ImageConverter.Work
{
    public enum WorkModifiedType
    {
        NONE,
        COPY,
        UPDATE,
        DELETE
    }

    public class WorkModification
    {
        private readonly Dictionary<string, WorkModifiedType> _hash = FileUtil.CreateDictionary<WorkModifiedType>();
        private readonly HashSet<string> _cbhsah = FileUtil.CreateHashSet();

        public void Add(string file, WorkModifiedType type)
        {
            _hash.Add(Path.GetFileName(file), type);
        }

        public bool this[string name]
        {
            get => _cbhsah.Contains(name);
            set
            {
                if (value)
                {
                    _cbhsah.Add(name);
                }
                else
                {
                    _cbhsah.Remove(name);
                }
            }
        }

        public class InfoData(string dir, string name, WorkModifiedType type, WorkModification parent)
        {
            public string Dir => dir;
            public string Name => name;
            public WorkModifiedType Type => type;

            public bool Flag
            {
                get => parent[Name];
                set => parent[Name] = value;
            }

            public override string ToString()
            {
                return $"#{Dir}\\#{Name} #{Type}";
            }
        }

        public List<InfoData> GetInfos(string dir)
        {
            return _hash.Select(pair => new InfoData(dir, pair.Key, pair.Value, this)).ToList();
        }

        public WorkModifiedType GetModifiedType()
        {
            if (_hash.Count > 0)
            {
                return (WorkModifiedType)_hash.Values.Select(x => (int)x).Max();
            }
            else
            {
                return WorkModifiedType.NONE;
            }
        }
    }

    public class WorkSetting
    {
        public bool UpdateModifedDate { get; set; } = false;
        public bool RescanDir { get; set; } = false;

        private readonly Dictionary<string, WorkModification> _hash = FileUtil.CreateDictionary<WorkModification>();

        public bool HasModifiedType => _hash.Count > 0;

        public void SetModification(string subdir, WorkModification t)
        {
            _hash[subdir] = t;
        }

        public WorkModifiedType GetModifiedType(string subdir)
        {
            if (!_hash.TryGetValue(subdir, out WorkModification value))
            {
                return WorkModifiedType.NONE;
            }
            return value.GetModifiedType();
        }

        public WorkModification GetModification(string subdir)
        {
            if (!_hash.TryGetValue(subdir, out WorkModification value))
            {
                return null;
            }
            return value;
        }

        public void Merge(WorkSetting setting)
        {
            foreach (var pair in setting._hash)
            {
                SetModification(pair.Key, pair.Value);
            }
        }
    }

    class WorkData(string rootDir)
    {
        public string FromRootDir { get; } = rootDir;
        public LocalWorkProcessor FromProc { get; }

        private List<string> _dirs;
        public IReadOnlyList<string> SubDirs => _dirs;

        public WorkSetting Setting { get; } = new WorkSetting();

        public WorkData(LocalWorkProcessor proc) :
            this(proc.ToRootDir)
        {
            FromProc = proc;
        }

        #region 処理フォルダー
        public WorkData SetTargetDirs(List<string> dirs)
        {
            _dirs = dirs;
            return this;
        }

        public WorkData SetTargetDirs(string dir)
        {
            _dirs = [dir];
            return this;
        }

        public WorkData SetAllDirToTarget()
        {
            _dirs = new List<string>(Directory.EnumerateDirectories(FromRootDir));
            return this;
        }
        #endregion

        #region 処理
        private readonly List<WorkProcessor> _procs = [];
        public IEnumerable<WorkProcessor> ProcList => _procs;

        public WorkData AddProc(params WorkProcessor[] procs)
        {
            foreach (var proc in procs)
            {
                if (proc is not null)
                {
                    _procs.Add(proc);
                    proc.SetupSetting(this);
                }
            }
            return this;
        }
        #endregion

        #region インターフェイス
        public void StartSync()
        {
            foreach (var proc in ProcList)
            {
                proc.StartProc();
                proc.SyncSubFolder(this);
            }
        }

        public void EndSync()
        {
            foreach (var proc in ProcList)
            {
                proc.EndProc();
            }
        }

        public void DeleteSubFolderIfNoOrig(IWorkFolder toRootDir)
        {
            SimpleLogUtil.D(GetType(), @"DeleteNoOrigSubFolder {0}", toRootDir.LogPath);
            foreach (var dir in toRootDir.GetFolders())
            {
                SimpleLogUtil.D(GetType(), @" To: {0}", dir.FileName);
                if (!Directory.Exists(Path.Combine(FromRootDir, dir.FileName)))
                {
                    SimpleLogUtil.I(GetType(), @"Delete Folder {0}", dir.FileName);
                    dir.DeleteFolder();
                }
            }
        }
        #endregion
    }

    class SubFolderWorkData
    {
        public string FromDir { get; }
        public IWorkFolder ToDir { get; }

        private readonly WorkProcessor mProc;
        private readonly string mSubname;

        public SubFolderWorkData(WorkData data, WorkProcessor proc, string subname)
        {
            subname = Path.GetFileName(subname);
            mProc = proc;
            FromDir = Path.Combine(data.FromRootDir, subname);
            ToDir = proc.ToRoot.GetSubWorkFolder(subname);
            Setting = data.Setting;
            mSubname = subname;
        }

        public bool StartSync()
        {
            if (!Directory.Exists(FromDir))
            {
                return false;
            }

            ToDir.CreateFolder();
            return mProc.ScanSubFolder(this);
        }

        #region ファイル選択
        private readonly Dictionary<string, IWorkFile> _fromToDic = FileUtil.CreateDictionary<IWorkFile>();
        private readonly Dictionary<string, Dictionary<string, IWorkFile>> _toFiles = FileUtil.CreateDictionary<Dictionary<string, IWorkFile>>();

        public void AddConvertRule(string fromExt, string toExt)
        {
            SimpleLogUtil.D(GetType(), @"AddConvertRule {0}\{1} => {2}\{3}", FromDir, fromExt, ToDir.LogPath, toExt);

            if (!_toFiles.TryGetValue(toExt, out Dictionary<string, IWorkFile> toDic))
            {
                toDic = FileUtil.CreateDictionary<IWorkFile>();
                if (!string.IsNullOrEmpty(toExt))
                {
                    foreach (var file in ToDir.GetFiles(toExt))
                    {
                        toDic.Add(file.FileName, file);
                        SimpleLogUtil.D(GetType(), @" Add {0}", file.FileName);
                    }
                }
                _toFiles[toExt] = toDic;
            }

            foreach (string file in Directory.GetFiles(FromDir, "*" + fromExt))
            {
                string toFileName = Path.GetFileNameWithoutExtension(file) + toExt;
                if (toDic.TryGetValue(toFileName, out IWorkFile value))
                {
                    _fromToDic[file] = value;
                    toDic.Remove(toFileName);
                    SimpleLogUtil.D(GetType(), @" Exist {0} => {1}", file, _fromToDic[file].LogPath);
                }
                else
                {
                    _fromToDic[file] = ToDir.GetNewSubWorkFile(toFileName);
                    SimpleLogUtil.D(GetType(), @" New {0} => {1}", file, _fromToDic[file].LogPath);
                }
            }
        }

        public void AddConvertRule(string ext)
        {
            AddConvertRule(ext, ext);
        }

        public void AddCommonConvertRule()
        {
            // 画像
            AddConvertRule(".png", ".isd");
            AddConvertRule(".jpg", ".isd");

            // テキスト
            AddConvertRule(".txt", ".tsd");
        }

        public void AddSyncConvertRule()
        {
            AddConvertRule(".isd");
            AddConvertRule(".tsd");
        }

        public List<FilePair> GetPairList()
        {
            return _fromToDic.Select(pair => new FilePair(pair)).ToList();
        }
        #endregion

        #region ファイル削除
        public bool DeleteNoOrigMode { get; set; } = false;
        public bool DeleteUpdatedFileMode { get; set; } = false;

        public List<IWorkFile> GetDeleteFileList()
        {
            List<IWorkFile> ret = [];
            if (DeleteNoOrigMode)
            {
                foreach (var toDic in _toFiles.Values)
                {
                    ret.AddRange(toDic.Values);
                }
            }
            if (DeleteUpdatedFileMode)
            {
                foreach (var pair in GetPairList())
                {
                    if (pair.ToFile.Exists() && !mProc.IsSameImageOrTextFile(pair))
                    {
                        ret.Add(pair.ToFile);
                    }
                }
            }
            return ret;
        }
        #endregion

        #region 設定
        public WorkSetting Setting { get; }

        public void SetupModifiedTypeOfSetting()
        {
            var wm = new WorkModification();
            foreach (var toDic in _toFiles.Values)
            {
                foreach (var file in toDic.Values)
                {
                    wm.Add(file.FileName, WorkModifiedType.DELETE);
                }
            }

            foreach (var pair in GetPairList())
            {
                var type = pair.GetModifiedType(mProc);
                if (type == WorkModifiedType.UPDATE)
                {
                    wm.Add(pair.ToFile.FileName, type);
                }
                if (type == WorkModifiedType.COPY)
                {
                    wm.Add(Path.GetFileName(pair.FromFile), type);
                }
            }

            foreach (var pair in new FilePair[] { IndexFilePair /*, SettingFilePair*/ })
            {
                var type = pair.GetModifiedType(mProc);
                if (type == WorkModifiedType.UPDATE)
                {
                    wm.Add(pair.ToFile.FileName, type);
                }
                if (type == WorkModifiedType.COPY)
                {
                    wm.Add(Path.GetFileName(pair.FromFile), type);
                }
            }

            Setting.SetModification(mSubname, wm);
        }
        #endregion

        #region 画像ファイル
        public void DeleteFile(IWorkFile toFile)
        {
            try
            {
                SimpleLogUtil.I(GetType(), @"DeleteFile {0}", toFile.LogPath);
                toFile.DeleteFile();
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(GetType(), @"DeleteFile", ex);
            }
        }

        public bool AfterDelete()
        {
            try
            {
                return mProc.AfterDelete(this);
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(GetType(), @"AfterDelete", ex);
                return false;
            }
        }

        public void CopyFile(FilePair pair)
        {
            try
            {
                if (File.Exists(pair.FromFile))
                {
                    SimpleLogUtil.D(GetType(), @"CopyFile {0} => {1}", pair.FromFile, pair.ToFile.LogPath);
                    if (pair.GetModifiedType(mProc) == WorkModifiedType.NONE)
                    {
                        return;
                    }
                    SimpleLogUtil.D(GetType(), @"  Need Copy");
                    mProc.CopyImageOrTextFile(pair);
                }
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(GetType(), @"CopyFile", ex);
            }
        }
        #endregion

        #region その他ファイル
        public const string INDEX_FILE = "index.dat";
        public const string SETTING_FILE = "setting.dat";

        public string FromIndex => Path.Combine(FromDir, INDEX_FILE);
        public IWorkFile ToIndex => ToDir.GetSubWorkFile(INDEX_FILE);
        public FilePair IndexFilePair => new(FromIndex, ToIndex);

        public string FromSetting => Path.Combine(FromDir, SETTING_FILE);
        public IWorkFile ToSetting => ToDir.GetSubWorkFile(SETTING_FILE);
        public FilePair SettingFilePair => new(FromSetting, ToSetting);

        public void CopyMiscFile()
        {
            try
            {
                if (File.Exists(FromIndex))
                {
                    mProc.CopyIndexFile(IndexFilePair);
                }
                else
                {
                    ToIndex.DeleteFile();
                }

                if (File.Exists(FromSetting))
                {
                    mProc.CopySettingFile(SettingFilePair);
                }
                else
                {
                    ToSetting.DeleteFile();
                }
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(GetType(), @"CopyMiscFile", ex);
            }
        }
        #endregion
    }

    class FilePair(string fromFile, IWorkFile toFile)
    {
        static public Encoding FileEncode = new UTF8Encoding(false);

        public string Key => Path.GetFileNameWithoutExtension(FromFile);

        public string FromFile { get; } = fromFile;
        public IWorkFile ToFile { get; private set; } = toFile;

        public FilePair(KeyValuePair<string, IWorkFile> pair) :
            this(pair.Key, pair.Value)
        {
        }

        #region File Copy
        public Bitmap GetInBitmap()
        {
            return new Bitmap(FromFile);
        }

        public Stream GetInStream()
        {
            return new FileStream(FromFile, FileMode.Open, FileAccess.Read);
        }

        public Stream GetOutStream()
        {
            return ToFile.GetOutStream();
        }

        private static void DoneStream(Stream outStream)
        {
            if (outStream is IWorkStreamDone done)
            {
                done.DoneStream();
            }
        }

        public void Convert(Action<Stream, Stream> conv)
        {
            using var inStream = GetInStream();
            using var outStream = GetOutStream();
            conv(inStream, outStream);
            DoneStream(outStream);
        }

        public void ConvertWriter(Action<StreamReader, StreamWriter> conv)
        {
            using var reader = new StreamReader(FromFile, FileEncode);
            using var outStream = GetOutStream();
            using var writer = new StreamWriter(outStream, FileEncode);
            conv(reader, writer);
            writer.Flush();
            DoneStream(outStream);
        }

        public void Copy()
        {
            using var inStream = GetInStream();
            using var outStream = GetOutStream();
            inStream.CopyTo(outStream);
            DoneStream(outStream);
        }
        #endregion

        #region Check File
        public WorkModifiedType GetModifiedType(WorkProcessor proc)
        {
            return GetModifiedType(proc.IsSameImageOrTextFile);
        }

        public WorkModifiedType GetModifiedType(Func<FilePair, bool> proc)
        {
            if (!ToFile.Exists())
            {
                return WorkModifiedType.COPY;
            }
            return proc(this) ? WorkModifiedType.NONE : WorkModifiedType.UPDATE;
        }

        public WorkModifiedType GetModifiedTypeWithContent()
        {
            var ret = WorkModifiedType.NONE;
            if (File.Exists(FromFile))
            {
                if (ToFile.Exists())
                {
                    if (!IsSameByContent())
                    {
                        return WorkModifiedType.UPDATE;
                    }
                }
                else
                {
                    ret = WorkModifiedType.COPY;
                }
            }
            else
            {
                if (ToFile.Exists())
                {
                    return WorkModifiedType.DELETE;
                }
            }
            return ret;
        }

        public bool IsSameBySize()
        {
            FileInfo ffi = new(FromFile);
            bool ret = ffi.Length == ToFile.FileSize;
            SimpleLogUtil.D(GetType(), @"IsSameBySize {0}: {1} == {2} => {3}", Path.GetFileName(FromFile), ffi.Length, ToFile.FileSize, ret);
            return ret;
        }

        public bool IsSameByContent()
        {
            byte[] b1 = File.ReadAllBytes(FromFile);
            byte[] b2 = ToFile.GetBytes();
            return b1 == b2;
        }
        #endregion

        #region Util
        public FilePair CreateFilePairFor(WorkProcessor work)
        {
            string file = ToFile.FileName;
            string subdir = ToFile.ParentWorkFolder.FileName;
            var subWorkFolder = work.ToRoot.GetSubWorkFolder(subdir);
            return new FilePair(FromFile, subWorkFolder.GetSubWorkFile(file));
        }

        public void ChangeExtOfToFile(string ext)
        {
            string name = Path.GetFileNameWithoutExtension(ToFile.FileName) + ext;
            ToFile = ToFile.ParentWorkFolder.GetSubWorkFile(name);
        }

        public static string GetLogPath(IWorkObject p)
        {
            List<string> tmp = [];
            do
            {
                tmp.Insert(0, p.FileName);
                p = p.ParentWorkFolder;
            } while (p is not null);
            return string.Join(@"\", tmp);
        }

        public static bool EqualWorkObject(IWorkObject p1, IWorkObject p2)
        {
            if (!FileUtil.EqualsPath(p1.FileName, p2.FileName))
            {
                return false;
            }

            if (p1.ParentWorkFolder is null)
            {
                return p2.ParentWorkFolder is null;
            }
            if (p2.ParentWorkFolder is null)
            {
                return p1.ParentWorkFolder is null;
            }

            if (p1.GetType() != p2.GetType())
            {
                return false;
            }

            if (!p1.ParentWorkFolder.Equals(p2.ParentWorkFolder))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
