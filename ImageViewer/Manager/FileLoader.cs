using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageViewer.Util;

namespace ImageViewer.Manager
{
    interface IFileManagerCache
    {
        FileManager GetFromCache(string dir);
    }

    interface IFileLoader
    {
        string TargetDir { get; }
        FilePosCursor CreateCursor(FileManager fman, IFileActionReloader reloader);
    }

    class FileDirLoader(string dir) : IFileLoader
    {
        public string TargetDir { get; } = dir;

        public FilePosCursor CreateCursor(FileManager fman, IFileActionReloader reloader)
        {
            return FilePosCursor.CreateFirst(fman);
        }
    }

    class FileOneFileLoader(string file) : IFileLoader
    {
        public string TargetDir { get; } = Path.GetDirectoryName(file);

        public FilePosCursor CreateCursor(FileManager fman, IFileActionReloader reloader)
        {
            FilePosCursor c = FilePosCursor.CreateFromFile(fman, file);
            if (c is null)
            {
                reloader.Reload(fman);
                c = FilePosCursor.CreateFromFile(fman, file);
                c ??= FilePosCursor.CreateFirst(fman);
            }
            return c;
        }
    }

    class FileManyFileLoader(List<string> files) : IFileLoader
    {
        public string TargetDir { get; } = Path.GetDirectoryName(files[0]);

        public FilePosCursor CreateCursor(FileManager fman, IFileActionReloader reloader)
        {
            var fileList = new FileInfoList(fman);
            var selections = fileList.GetFileInfo(files);
            if (selections.Count != files.Count)
            {
                reloader.Reload(fman);
                selections = fileList.GetFileInfo(files);
            }
            return FilePosCursor.Create(fileList, selections);
        }
    }

    static class FileLoaderUtil
    {
        public static FileManager CreateManager(this IFileLoader loader, IFileManagerCache cache)
        {
            string dir = loader.TargetDir;
            return cache.GetFromCache(dir) ?? new FileManager(dir);
        }

        public static IFileLoader Create(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }
            if (Directory.Exists(file))
            {
                return new FileDirLoader(file);
            }
            if (File.Exists(file))
            {
                return new FileOneFileLoader(file);
            }
            return null;
        }

        public static IFileLoader Create(List<string> files)
        {
            if (files is null || files.Count == 0)
            {
                return null;
            }
            if (files.Count == 1)
            {
                return Create(files[0]);
            }

            var fileList = new List<string>();
            string targetDir = null;
            foreach (string f in files)
            {
                if (File.Exists(f))
                {
                    fileList.Add(f);
                }
                else if (Directory.Exists(f))
                {
                    targetDir ??= f;
                }
            }
            if (fileList.Count > 0)
            {
                string dir = Path.GetDirectoryName(fileList[0]);
                fileList.RemoveAll(f => !f.StartsWith(dir));
                return new FileManyFileLoader(fileList);
            }
            else if (targetDir is not null)
            {
                return new FileDirLoader(targetDir);
            }
            return null;
        }

        public static IFileLoader CreateNext(string dir, int move)
        {
            var dirs = FileUtil.GetSortedDirectories(Path.GetDirectoryName(dir));
            if (dirs.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < dirs.Count; ++i)
            {
                if (FileUtil.EqualsPath(dir, dirs[i]))
                {
                    string newDir = ListUtil.SlideItemAt(dirs, i, move);
                    return new FileDirLoader(newDir);
                }
            }
            return new FileDirLoader(dirs[0]);
        }
    }
}
