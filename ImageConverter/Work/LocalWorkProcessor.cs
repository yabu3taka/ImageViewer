using ImageConverter.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageConverter.Work
{
    abstract class LocalWorkProcessor : WorkProcessor
    {
        protected LocalWorkProcessor(string toDir)
        {
            ToRootDir = toDir;
            ToRoot = new WorkTargetLocal(toDir);
        }

        #region Localのみ
        public string ToRootDir { get; }
        public string PassFile => AppSetting.GetPassFile(ToRootDir);
        #endregion

        #region WorkData
        public override void StartProc()
        {
        }

        public override void EndProc()
        {
        }
        #endregion

        private class WorkTargetLocal(string path) : IWorkFile, IWorkFolder
        {
            private readonly string mPath = path;

            #region IWorkObject
            public string LogPath => mPath; 

            public string FileName => Path.GetFileName(mPath);

            public bool Exists()
            {
                return File.Exists(mPath) || Directory.Exists(mPath);
            }

            public IWorkFolder ParentWorkFolder => new WorkTargetLocal(Path.GetDirectoryName(mPath));

            public bool Equals(IWorkObject other)
            {
                return FileUtil.EqualsPath(mPath, ((WorkTargetLocal)other).mPath);
            }
            #endregion

            #region IWorkFile
            public long FileSize
            {
                get
                {
                    FileInfo tfi = new(mPath);
                    return tfi.Length;
                }
            }

            public byte[] GetBytes()
            {
                return File.ReadAllBytes(mPath);
            }

            public Stream GetOutStream()
            {
                return new FileStream(mPath, FileMode.Create, FileAccess.Write);
            }

            public void SetFileDate(DateTime date)
            {
                File.SetCreationTime(mPath, date);
                File.SetLastWriteTime(mPath, date);
                File.SetLastAccessTime(mPath, date);
            }

            public void DeleteFile()
            {
                File.Delete(mPath);
            }
            #endregion

            #region IWorkFolder
            public IWorkFile GetNewSubWorkFile(string subname)
            {
                return new WorkTargetLocal(Path.Combine(mPath, subname));
            }

            public IWorkFile GetSubWorkFile(string subname)
            {
                return new WorkTargetLocal(Path.Combine(mPath, subname));
            }

            public IWorkFolder GetNewSubWorkFolder(string subname)
            {
                return new WorkTargetLocal(Path.Combine(mPath, subname));
            }

            public IWorkFolder GetSubWorkFolder(string subname)
            {
                return new WorkTargetLocal(Path.Combine(mPath, subname));
            }

            public IList<IWorkFile> GetFiles(string ext)
            {
                return Directory.EnumerateFiles(mPath, "*" + ext)
                    .Select(file => new WorkTargetLocal(file))
                    .Cast<IWorkFile>()
                    .ToList();
            }

            public IList<IWorkFolder> GetFolders()
            {
                return Directory.EnumerateDirectories(mPath)
                    .Select(dir => new WorkTargetLocal(dir))
                    .Cast<IWorkFolder>()
                    .ToList();
            }

            public void CreateFolder()
            {
                Directory.CreateDirectory(mPath);
            }

            public void DeleteFolder()
            {
                Directory.Delete(mPath, true);
            }

            public void RenameFolder(string newName)
            {
                string newDir = Path.Combine(Path.GetDirectoryName(mPath), newName);
                Directory.Move(mPath, newDir);
            }
            #endregion
        }
    }
}
