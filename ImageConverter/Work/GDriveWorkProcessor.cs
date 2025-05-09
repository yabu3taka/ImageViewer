using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImageConverter.GDrive;

namespace ImageConverter.Work
{
    class GDriveWorkProcessor : WorkProcessor
    {
        public GDriveWorkProcessor(GoogleDrive drive, string toDir)
        {
            var file = drive.GetFolderInfo(toDir);
            ToRoot = new WorkFolderGDrive(null, drive, file, file.Name);
        }

        #region WorkData
        public override void StartProc()
        {
        }

        public override void SyncSubFolder(WorkData work)
        {
            work.DeleteSubFolderIfNoOrig(ToRoot);
        }

        public override void EndProc()
        {
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddSyncConvertRule();
            target.DeleteNoOrigMode = true;
            return true;
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.IsSameBySize();
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

        #region IWorkObject
        private abstract class WorkTargetGDrive(WorkFolderGDrive parent, GoogleDrive drive, GoogleDriveObject obj, string fileName) : IWorkObject
        {
            protected readonly WorkFolderGDrive _parent = parent;
            protected readonly GoogleDrive _drive = drive;
            protected GoogleDriveObject _obj = obj;

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
        private class WorkFileGDrive(WorkFolderGDrive parent, GoogleDrive drive, GoogleDriveFile file, string fileName) :
            WorkTargetGDrive(parent, drive, file, fileName), IWorkFile
        {
            private GoogleDriveFile _file = file;

            public long FileSize => _file.Length;

            public byte[] GetBytes()
            {
                return null;
            }

            public Stream GetOutStream()
            {
                return new GoogleDriveStream(this);
            }

            private class GoogleDriveStream(GDriveWorkProcessor.WorkFileGDrive workFile) : MemoryStream, IWorkStreamDone
            {
                public void DoneStream()
                {
                    Seek(0, SeekOrigin.Begin);
                    workFile._drive.UploadFile(workFile._parent._folder, workFile.FileName, this, "application/octet-stream");
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
                    _drive.Delete(_file);
                }
                _file = null;
                _obj = null;
            }
        }
        #endregion

        #region IWorkFolder
        private class WorkFolderGDrive(WorkFolderGDrive parent, GoogleDrive drive, GoogleDriveFolder folder, string fileName) :
            WorkTargetGDrive(parent, drive, folder, fileName), IWorkFolder
        {
            public GoogleDriveFolder _folder = folder;

            private GoogleDriveResult _result = null;
            private GoogleDriveResult Result
            {
                get
                {
                    if (_folder is not null)
                    {
                        _result ??= new GoogleDriveResult(_drive, _folder);
                    }
                    return _result;
                }
            }

            public IWorkFile GetNewSubWorkFile(string subname)
            {
                return new WorkFileGDrive(this, _drive, null, subname);
            }

            public IWorkFile GetSubWorkFile(string subname)
            {
                return new WorkFileGDrive(this, _drive, Result.FindFile(subname), subname);
            }

            public IWorkFolder GetNewSubWorkFolder(string subname)
            {
                return new WorkFolderGDrive(this, _drive, null, subname);
            }

            public IWorkFolder GetSubWorkFolder(string subname)
            {
                return new WorkFolderGDrive(this, _drive, Result.FindFolder(subname), subname);
            }

            public IList<IWorkFile> GetFiles(string ext)
            {
                return Result.FindFilesExt(ext)
                    .Select(file => new WorkFileGDrive(this, _drive, file, file.Name))
                    .Cast<IWorkFile>()
                    .ToList();
            }

            public IList<IWorkFolder> GetFolders()
            {
                return Result.Folders
                    .Select(folder => new WorkFolderGDrive(this, _drive, folder, folder.Name))
                    .Cast<IWorkFolder>()
                    .ToList();
            }

            public void CreateFolder()
            {
                _folder ??= _drive.CreateFolder(_parent._folder, FileName);
            }

            public void DeleteFolder()
            {
                if (_folder is not null)
                {
                    _drive.Delete(_folder);
                }
                _obj = null;
                _folder = null;
            }

            public void RenameFolder(string newName)
            {
                if (_folder is not null)
                {
                    _drive.Rename(_folder, newName);
                }
            }
        }
        #endregion
    }
}
