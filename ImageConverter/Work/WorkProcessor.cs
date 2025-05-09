using System;
using System.Collections.Generic;
using System.IO;

namespace ImageConverter.Work
{
    interface IWorkStreamDone
    {
        void DoneStream();
    }

    interface IWorkObject : IEquatable<IWorkObject>
    {
        string LogPath { get; }
        string FileName { get; }
        IWorkFolder ParentWorkFolder { get; }
        bool Exists();
    }

    interface IWorkFile : IWorkObject
    {
        long FileSize { get; }

        byte[] GetBytes();
        Stream GetOutStream();
        void SetFileDate(DateTime date);

        void DeleteFile();

        public bool ExistsExt(string ext)
        {
            string name = Path.GetFileNameWithoutExtension(FileName) + ext;
            var target = ParentWorkFolder.GetSubWorkFile(name);
            return target.Exists();
        }
    }

    interface IWorkFolder: IWorkObject
    {
        IWorkFile GetNewSubWorkFile(string subname);
        IWorkFile GetSubWorkFile(string subname);

        IWorkFolder GetNewSubWorkFolder(string subname);
        IWorkFolder GetSubWorkFolder(string subname);

        IList<IWorkFile> GetFiles(string ext);
        IList<IWorkFolder> GetFolders();

        void CreateFolder();
        void DeleteFolder();
        void RenameFolder(string newName);
    }

    abstract class WorkProcessor
    {
        public IWorkFolder ToRoot { get; protected set; }

        #region WorkData
        public virtual void SetupSetting(WorkData work)
        {
        }

        public abstract void StartProc();

        public abstract void SyncSubFolder(WorkData work);

        public abstract void EndProc();
        #endregion

        #region SubFolderWorkData
        public abstract bool ScanSubFolder(SubFolderWorkData target);

        public virtual bool AfterDelete(SubFolderWorkData target)
        {
            return false;
        }

        public abstract bool IsSameImageOrTextFile(FilePair pair);
        public abstract void CopyImageOrTextFile(FilePair pair);

        public abstract void CopyIndexFile(FilePair pair);
        public abstract void CopySettingFile(FilePair pair);

        public void RenameDirectory(string oldName, string newName)
        {
            var oldDir = ToRoot.GetSubWorkFolder(oldName);
            if (oldDir.Exists())
            {
                oldDir.RenameFolder(newName);
            }
        }

        public void DeleteDirectory(string oldName)
        {
            var oldDir = ToRoot.GetSubWorkFolder(oldName);
            if (oldDir.Exists())
            {
                oldDir.DeleteFolder();
            }
        }
        #endregion
    }
}
