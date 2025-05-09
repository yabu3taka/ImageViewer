using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    #region Real File/Folder
    interface IRealCommon
    {
        string FilePath { get; }
    }

    interface IRealFolder : IRealCommon
    {
    }

    interface IRealFile : IRealCommon
    {
        string FileName { get; }
    }

    class RealFolderSimple(string dir) : IRealFolder
    {
        public string FilePath { get; } = dir;
    }

    class RealFileSimple(string dir) : IRealFile
    {
        public string FilePath { get; } = dir;
        public string FileName { get; } = Path.GetFileName(dir);
    }
    #endregion

    #region FileInfo
    interface IFileImageInfo
    {
        Bitmap Bitmap { get; }
        Size ImageSize { get; }
        string FileSize { get; }
    }

    class FileImageInfoSimple : IFileImageInfo
    {
        private readonly string _filepath;

        private FileImageInfoSimple(Bitmap img, string filepath)
        {
            _filepath = filepath;
            Bitmap = img;
        }

        public static IFileImageInfo Create(string filepath)
        {
            var img = BitmapUtil.OpenBitmap(filepath);
            if (img is null)
            {
                return null;
            }
            return new FileImageInfoSimple(img, filepath);
        }

        public Bitmap Bitmap { get; }
        public Size ImageSize { get { return Bitmap.Size; } }
        public string FileSize { get { return FileUtil.GetSizeText(_filepath); } }
    }

    interface IFileInfo : IRealCommon
    {
        IFileManager MyManager { get; }
        IFileImageInfo GetImageInfo();

        int Id { get; }

        string FileName { get; }
        string Title { get; }
    }

    interface IFileDetail
    {
        void LoadDetailedTitle();

        string DetailTitle { get; }
    }

    interface IFileText
    {
        string PhotoTextPath { get; }
        string FileText { get; set; }
    }
    #endregion

    #region FileManager/Controller
    interface IFileManager
    {
        string Title { get; }
        int ForEach(Action<IFileInfo> a, out IFileComparer defComparer);
        bool IsMime(IFileInfo finfo);
    }

    interface IFileRealManager
    {
        string FilePath { get; }
        void Reload();
        void NotifyFolderRenamed(string newDir, IFileActionReloader reloader);
    }

    interface IFileIndexController
    {
        IFileMarker IndexMarker { get; }
        void ReloadIndex();
        void SaveIndex();
    }

    interface IFileGroupController
    {
        IFileMarkerAccessor GetGroupMarker();
    }

    interface IFileDeleteController
    {
        IFileMarker CreateDeleteMarker();
        bool IsDeletable(IFileInfo finfo);
    }

    interface IFileCutAndPasteController
    {
        bool IsPastable(IFileInfo finfo);
    }

    class FileEmptyManager(IFileManager fman = null) : IFileManager
    {
        public IFileManager InternalManager { get; } = fman;

        public string Title
        {
            get
            {
                return InternalManager?.Title ?? "";
            }
        }

        public int ForEach(Action<IFileInfo> a, out IFileComparer defComparer)
        {
            defComparer = new FileComparerSimple();
            return 0;
        }

        public bool IsMime(IFileInfo finfo)
        {
            return false;
        }
    }
    #endregion

    #region Util
    static class FileInfoUtil
    {
        public static FileUtil.FileExt ImageExt = new(".png", ".jpg", ".jpeg", ".gif");

        #region IRealFolder
        public static string Combine(this IRealFolder folder, string name)
        {
            return Path.Combine(folder.FilePath, name);
        }

        public static bool HasFile(this IRealFolder folder, string name)
        {
            return File.Exists(folder.Combine(name));
        }
        #endregion

        #region IFileInfo
        public static DateTime LastWriteTime(this IFileInfo finfo)
        {
            var fi = new FileInfo(finfo.FilePath);
            return fi.LastWriteTime;
        }

        public static string GetExt(this IFileInfo finfo)
        {
            return Path.GetExtension(finfo.FileName);
        }

        public static void CopyTo(this IFileInfo finfo, string toPath)
        {
            File.Copy(finfo.FilePath, toPath);
        }

        public static void CopyToFolder(this IFileInfo finfo, string toFolder)
        {
            File.Copy(finfo.FilePath, Path.Combine(toFolder, finfo.FileName));
        }

        public static Dictionary<string, IFileInfo> CreateDictionary(this IEnumerable<IFileInfo> list)
        {
            var ret = FileUtil.CreateDictionary<IFileInfo>();
            foreach (var finfo in list)
            {
                ret.Add(finfo.FilePath, finfo);
            }
            return ret;
        }

        public static Bitmap GetBitmap(this IFileInfo finfo)
        {
            return finfo.GetImageInfo()?.Bitmap;
        }

        public static bool GetIndexMark(this IFileInfo finfo)
        {
            if (finfo.MyManager is IFileIndexController icon)
            {
                return icon.IndexMarker[finfo];
            }
            return false;
        }

        public static string GetToolTip(this IFileImageInfo imgInfo)
        {
            return $"{imgInfo.ImageSize.ToDimString()} {imgInfo.FileSize}";
        }
        #endregion

        #region IRealCommon
        public static bool IsSame(this IRealCommon file, string path)
        {
            return FileUtil.EqualsPath(file.FilePath, path);
        }

        public static bool IsSame(this IRealCommon file1, IRealCommon file2)
        {
            return FileUtil.EqualsPath(file1.FilePath, file2?.FilePath);
        }

        public static IRealFolder GetParent(this IRealCommon finfo)
        {
            return new RealFolderSimple(Path.GetDirectoryName(finfo.FilePath));
        }
        #endregion

        #region IFileManager
        public static List<IFileInfo> GetAllItems(this IFileManager fman)
        {
            List<IFileInfo> files = [];
            void getFunc(IFileInfo finfo)
            {
                files.Add(finfo);
            }
            fman.ForEach(getFunc, out IFileComparer defComparer);
            files.Sort(defComparer);
            return files;
        }

        public static IFileInfo GetFirst(this IFileManager fman)
        {
            var files = fman.GetAllItems();
            return files.GetFirst();
        }

        public static FileFromToBoundList GetGroupBoundList(this IFileManager fman)
        {
            if (fman is IFileGroupController gcon)
            {
                return new FileFromToBoundList(fman, gcon.GetGroupMarker());
            }
            return null;
        }

        public static FileFromToBoundList GetIndexBoundList(this IFileManager fman)
        {
            if (fman is IFileIndexController icon)
            {
                return new FileFromToBoundList(fman, icon.IndexMarker);
            }
            return null;
        }
        #endregion

        #region IFilePosCursor
        public static IFileManager GetMyManager(this IFilePosCursor c)
        {
            return c.FileList.MyManager;
        }

        public static FileFromToBoundList GetGroupBoundList(this IFilePosCursor c)
        {
            return GetGroupBoundList(c.FileList.MyManager);
        }

        public static FileFromToBoundList GetIndexBoundList(this IFilePosCursor c)
        {
            return GetIndexBoundList(c.FileList.MyManager);
        }
        #endregion
    }
    #endregion
}
