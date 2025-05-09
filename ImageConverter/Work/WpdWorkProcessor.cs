using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImageConverter.PortableDevices;
using ImageConverter.Util;

namespace ImageConverter.Work
{
    abstract class WpdWorkProcessorBase(PortableDevice device, string toDir) : WorkProcessor
    {
        private readonly PortableDevice _device = device;
        private readonly string _toDir = toDir;
        protected PortableDeviceConnector _connector;

        public bool HasPassFile()
        {
            using var c = _device.Connect();
            var devFile = c.GetContent(Path.Combine(_toDir, "pass.dat"));
            return devFile is not null;
        }

        #region WorkData
        public override void StartProc()
        {
            _connector = _device.Connect();
            var devFolder = _connector.GetFolder(_toDir) ?? throw new MyException($"{_toDir}が見つかりません");
            ToRoot = new WorkFolderWpd(null, _connector, devFolder, devFolder.Name);
        }

        public override void EndProc()
        {
            _connector.Disconnect();
            _connector = null;
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddSyncConvertRule();
            target.DeleteNoOrigMode = true;
            target.DeleteUpdatedFileMode = true;
            return true;
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.IsSameBySize();
        }

        #endregion

        #region IWorkObject
        private abstract class WorkObjectWpd(WorkFolderWpd parent, PortableDeviceConnector con, PortableDeviceObject obj, string fileName) : IWorkObject
        {
            protected readonly WorkFolderWpd _parent = parent;
            protected readonly PortableDeviceConnector _connector = con;
            protected PortableDeviceObject _obj = obj;

            public string LogPath => FilePair.GetLogPath(this);

            public string FileName { get; } = fileName;

            public IWorkFolder ParentWorkFolder => _parent;

            public bool Exists()
            {
                return _obj is not null;
            }

            public bool Equals(IWorkObject otherP)
            {
                return FilePair.EqualWorkObject(this, otherP);
            }
        }
        #endregion

        #region IWorkFile
        private class WorkFileWpd(WorkFolderWpd parent, PortableDeviceConnector con, PortableDeviceFile file, string fileName) :
            WorkObjectWpd(parent, con, file, fileName), IWorkFile
        {
            private PortableDeviceFile _file = file;

            public long FileSize => ((PortableDeviceFile)_file).Length;

            public byte[] GetBytes()
            {
                return _connector.DownloadFileToByte(_file);
            }

            public Stream GetOutStream()
            {
                return new WpdStream(this);
            }

            private class WpdStream(WorkFileWpd file) : MemoryStream, IWorkStreamDone
            {
                private readonly WorkFileWpd _workFile = file;

                public void DoneStream()
                {
                    Seek(0, SeekOrigin.Begin);
                    if (_workFile.DeleteFileBeforeCopy())
                    {
                        _workFile._connector.CreateFileFromMemory(_workFile._parent._folder, _workFile.FileName, this);
                    }
                }
            }

            public void SetFileDate(DateTime date)
            {
                //throw new NotImplementedException();
            }

            public void DeleteFile()
            {
                if (_file is not null)
                {
                    _connector.DeleteBatch(_file);
                }
                _file = null;
                _obj = null;
            }

            public bool DeleteFileBeforeCopy()
            {
                bool ret = true;
                if (_file is not null)
                {
                    ret = false;
                    //_connector.DeleteFile(_file);
                }
                _file = null;
                _obj = null;
                return ret;
            }
        }
        #endregion

        #region IWorkFolder
        private class WorkFolderWpd(WorkFolderWpd parent, PortableDeviceConnector con, PortableDeviceFolder folder, string fileName) :
            WorkObjectWpd(parent, con, folder, fileName), IWorkFolder
        {
            public PortableDeviceFolder _folder = folder;

            private PortableDeviceResult _result = null;
            private PortableDeviceResult Result
            {
                get
                {
                    if (_folder is not null)
                    {
                        _result ??= _connector.GetContents(_folder);
                    }
                    return _result;
                }
            }

            public IWorkFile GetNewSubWorkFile(string subname)
            {
                return new WorkFileWpd(this, _connector, null, subname);
            }

            public IWorkFile GetSubWorkFile(string subname)
            {
                return new WorkFileWpd(this, _connector, Result.FindFile(subname), subname);
            }

            public IWorkFolder GetNewSubWorkFolder(string subname)
            {
                return new WorkFolderWpd(this, _connector, null, subname);
            }

            public IWorkFolder GetSubWorkFolder(string subname)
            {
                return new WorkFolderWpd(this, _connector, Result.FindFolder(subname), subname);
            }

            public IList<IWorkFile> GetFiles(string ext)
            {
                return Result.FindFilesExt(ext)
                    .Select(file => new WorkFileWpd(this, _connector, file, file.Name))
                    .Cast<IWorkFile>()
                    .ToList();
            }

            public IList<IWorkFolder> GetFolders()
            {
                return Result.Folders
                    .Select(folder => new WorkFolderWpd(this, _connector, folder, folder.Name))
                    .Cast<IWorkFolder>()
                    .ToList();
            }

            public void CreateFolder()
            {
                _folder ??= _connector.CreateFolder(_parent._folder, FileName);
            }

            public void DeleteFolder()
            {
                if (_folder is not null)
                {
                    _connector.DeleteFolder(_folder);
                }
                _obj = null;
                _folder = null;
            }

            public void RenameFolder(string newName)
            {
                if (_folder is not null)
                {
                    _connector.Rename(_folder, newName);
                }
            }
        }
        #endregion
    }

    class WpdWorkProcessorCheck(PortableDevice device, string toDir) : WpdWorkProcessorBase(device, toDir)
    {
        public static WpdWorkProcessorCheck Create(PortableDevice device, string toDir)
        {
            return new(device, toDir);
        }

        #region WorkData
        public override void SyncSubFolder(WorkData work)
        {
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            base.ScanSubFolder(target);
            target.SetupModifiedTypeOfSetting();
            return false;
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
        }

        public override void CopyIndexFile(FilePair pair)
        {
        }

        public override void CopySettingFile(FilePair pair)
        {
        }
        #endregion
    }

    class WpdWorkProcessorDelete(PortableDevice device, string toDir) : WpdWorkProcessorBase(device, toDir)
    {
        public static WpdWorkProcessorDelete Create(PortableDevice device, string toDir)
        {
            return new(device, toDir);
        }

        #region WorkData
        public override void SyncSubFolder(WorkData work)
        {
            work.DeleteSubFolderIfNoOrig(ToRoot);
        }
        #endregion

        #region SubFolderWorkData
        public override bool AfterDelete(SubFolderWorkData target)
        {
            SimpleLogUtil.I(GetType(), @"AfterDelete");
            target.ToIndex.DeleteFile();
            target.ToSetting.DeleteFile();
            _connector.DoneDelete();
            return true;
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
        }

        public override void CopyIndexFile(FilePair pair)
        {
        }

        public override void CopySettingFile(FilePair pair)
        {
        }
        #endregion
    }

    class WpdWorkProcessorCopy(PortableDevice device, string toDir) : WpdWorkProcessorBase(device, toDir)
    {
        public static WpdWorkProcessorCopy Create(PortableDevice device, string toDir)
        {
            return new(device, toDir);
        }

        #region WorkData
        public override void SyncSubFolder(WorkData work)
        {
        }
        #endregion

        #region SubFolderWorkData
        public override bool AfterDelete(SubFolderWorkData target)
        {
            return false;
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            pair.Copy();
        }

        public override void CopyIndexFile(FilePair pair)
        {
            pair.Copy();
        }

        public override void CopySettingFile(FilePair pair)
        {
            pair.Copy();
        }
        #endregion
    }
}
