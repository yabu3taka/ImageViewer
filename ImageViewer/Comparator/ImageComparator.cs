using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using ImageViewer.Manager;
using ImageViewer.Util;

namespace ImageViewer.Comparator
{
    abstract class ImageHash(string file)
    {
        public string FilePath { get; } = file;
    }

    delegate bool ImageCloseFunc<T>(T h1, T h2) where T : ImageHash;

    interface IImageComparator<T> where T : ImageHash
    {
        T CreateHash(string file);
        ImageCloseFunc<T> GetCloseFunc();
    }

    #region Target List
    class ImageComparationTarget
    {
        #region File List
        private readonly List<string> _list = [];
        public IReadOnlyList<string> FileList => _list;

        public void AddFile(string f)
        {
            _list.Add(f.ToLowerInvariant());
        }

        public void AddFile(IEnumerable<string> files)
        {
            foreach (string f in files)
            {
                AddFile(f);
            }
        }

        public void AddFolder(string dir)
        {
            AddFile(FileInfoUtil.ImageExt.EnumerateFiles(dir));
        }

        public void AddFileInfo(IFileInfo finfo)
        {
            if (finfo is IRealFolder folder)
            {
                AddFolder(folder.FilePath);
            }
            else if (finfo is IRealFile file)
            {
                AddFile(file.FilePath);
            }
        }

        public void AddCursor(FilePosCursor c)
        {
            var targets = c.SelectedItems;
            if (targets.Count <= 1)
            {
                if (c.GetMyManager() is FileDirManager)
                {
                }
                else
                {
                    targets = c.FileList.Items;
                }
            }
            foreach (var finfo in targets)
            {
                AddFileInfo(finfo);
            }
        }
        #endregion

        #region Compare Key
        private List<string> _keyFiles = null;
        public IReadOnlyList<string> KeyFileList => _keyFiles;

        public void SetKeyFiles(IEnumerable<string> files)
        {
            _keyFiles = files?.ToList();
            if (files is not null)
            {
                var hash = FileUtil.CreateHashSet(files);
                _list.RemoveAll(hash.Contains);
            }
        }

        public void SetKeyFiles(IEnumerable<IFileInfo> files)
        {
            SetKeyFiles(files?.Select(f => f.FilePath));
        }
        #endregion
    }
    #endregion

    #region ImageComparationManager
    interface IImageComparationManagerFactory
    {
        IImageComparationManager ReplaceManager(IImageComparationManager man);
    }

    interface IImageComparationManager
    {
        IImageComparationResult Process(BackgroundWorker worker, ImageComparationTarget target);
    }

    class ImageComparationManager<T>(IImageComparator<T> comparator) : IImageComparationManager
        where T : ImageHash
    {
        public IImageComparator<T> Comparator { get; } = comparator;

        private readonly Dictionary<string, T> _cache = FileUtil.CreateDictionary<T>();
        private T MakeHash(string file)
        {
            if (!_cache.TryGetValue(file, out T value))
            {
                value = Comparator.CreateHash(file);
                _cache[file] = value;
            }
            return value;
        }

        private List<T> GetHashList(BackgroundWorker worker, string name, IReadOnlyList<string> list)
        {
            var progress = new ImageComparationProgress(name, list.Count);
            var hashList = new List<T>(list.Count);
            foreach (string file in list)
            {
                hashList.Add(MakeHash(file));
                worker?.ReportProgress(0, progress.CreateNext());
            }
            return hashList;
        }

        public IImageComparationResult Process(BackgroundWorker worker, ImageComparationTarget target)
        {
            var fileList = target.FileList;
            var keyList = target.KeyFileList;
            ImageComparationProgress progress;

            var fileHashList = GetHashList(worker, "準備", fileList);

            var closeFunc = Comparator.GetCloseFunc();

            var ret = new ImageComparationResult<T>();
            if (keyList is null)
            {
                progress = new ImageComparationProgress("比較", fileList.Count);
                for (int i = 1; i < fileHashList.Count; ++i)
                {
                    var ics = new ImageCloseSet<T>(fileHashList[i - 1]);
                    for (int j = i; j < fileHashList.Count; ++j)
                    {
                        ics.AddIfClose(closeFunc, fileHashList[j]);
                    }
                    ret.AddIfHasPair(ics);
                    worker?.ReportProgress(0, progress.CreateNext());
                }
            }
            else
            {
                var keyHashList = GetHashList(worker, "Key準備", keyList);

                progress = new ImageComparationProgress("比較", keyList.Count);
                foreach (var keyHash in keyHashList)
                {
                    var ics = new ImageCloseSet<T>(keyHash);
                    foreach (var imageHash in fileHashList)
                    {
                        ics.AddIfClose(closeFunc, imageHash);
                    }
                    ret.AddIfHasPair(ics);
                    worker?.ReportProgress(0, progress.CreateNext());
                }

                ret.NotDeleteFirst = true;
            }
            return ret;
        }
    }
    #endregion

    #region Worker
    class ImageComparationWorkerArg
    {
        public IImageComparationManager ComparationManager;
        public ImageComparationTarget TargetList;

        public IImageComparationResult Process(System.ComponentModel.BackgroundWorker worker)
        {
            return ComparationManager.Process(worker, TargetList);
        }
    }

    class ImageComparationProgress
    {
        public string Name { get; }
        public int Num { get; }
        public int Pos { get; private set; }

        public ImageComparationProgress(string nm, int n)
        {
            Name = nm;
            Num = n;
            Pos = 0;
        }

        private ImageComparationProgress(string nm, int n, int p)
        {
            Name = nm;
            Num = n;
            Pos = p;
        }

        public ImageComparationProgress CreateNext()
        {
            Pos++;
            return new ImageComparationProgress(Name, Num, Pos);
        }
    }
    #endregion
}
