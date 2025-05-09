using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace ImageViewer.Util
{
    static class FileUtil
    {
        private static readonly string[] UNITS = ["B", "kB", "MB", "GB", "TB"];

        public static string GetSizeText(string file)
        {
            var fi = new FileInfo(file);
            if (!fi.Exists)
            {
                return "";
            }

            long size = fi.Length;
            int unitPos = 0;
            while (size >= 1000)
            {
                size /= 1000;
                unitPos++;
            }
            return size.ToString() + UNITS[unitPos];
        }

        #region Read/Write
        public static string ReadFile(string path, Encoding enc)
        {
            if (!File.Exists(path))
            {
                return "";
            }
            return File.ReadAllText(path, enc);
        }

        public static string ReadFile(string path)
        {
            return ReadFile(path, Encoding.UTF8);
        }

        public static void WriteOrDeleteFile(string path, string str, Encoding enc)
        {
            if (!string.IsNullOrEmpty(str))
            {
                File.WriteAllText(path, str, enc);
            }
            else
            {
                File.Delete(path);
            }
        }

        public static void WriteOrDeleteFile(string path, string str)
        {
            WriteOrDeleteFile(path, str, new UTF8Encoding(false));
        }
        #endregion

        #region Delete
        public static void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        public static bool SendToRecycleBin(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else if (Directory.Exists(file))
                {
                    FileSystem.DeleteDirectory(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
        #endregion

        #region Comparer
        public static bool EqualsPath(string file1, string file2)
        {
            return string.Equals(file1, file2, StringComparison.OrdinalIgnoreCase);
        }

        public static void Sort(List<string> list)
        {
            list.Sort(StringComparer.OrdinalIgnoreCase);
        }

        public static HashSet<string> CreateHashSet()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public static HashSet<string> CreateHashSet(IEnumerable<string> files)
        {
            return new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, T> CreateDictionary<T>()
        {
            return new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region FileExt
        public static string AppendExt(string file, string ext)
        {
            return Path.HasExtension(file) ? file : file + ext;
        }

        public static string ChangeExt(string file, string ext)
        {
            return Path.GetFileNameWithoutExtension(file) + ext;
        }

        public static bool ContainExt(string file, string ext)
        {
            return file.EndsWith(ext, StringComparison.OrdinalIgnoreCase);
        }

        public static FileExt GetExtList(params string[] extList)
        {
            return new FileExt(extList);
        }

        public class FileExt(params string[] extList)
        {
            public IEnumerable<string> EnumerateFiles(string dir)
            {
                if (!Directory.Exists(dir))
                {
                    return [];
                }
                return Directory.EnumerateFiles(dir).Where(IsSupported);
            }

            public List<string> GetFileNameList(string dir)
            {
                return EnumerateFiles(dir).Select(Path.GetFileName).ToList();
            }

            public List<string> GetFilePathList(string dir)
            {
                return EnumerateFiles(dir).ToList();
            }

            public bool IsSupported(string filename)
            {
                return extList.Any(ext => filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
            }

            public bool IsAnySupported(IEnumerable<string> filenames)
            {
                return filenames.Any(IsSupported);
            }

            public List<string> FilterSupportedFile(IEnumerable<string> filenames)
            {
                return filenames.Where(IsSupported).ToList();
            }

            public string GetSuportedFile(string dir, string filename)
            {
                string nameOnly = Path.GetFileNameWithoutExtension(filename);
                foreach (string ext in extList)
                {
                    string filePath = Path.Combine(dir, nameOnly + ext);
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }
                }
                return "";
            }

            public void OverwriteMove(string targetFile, string toDir)
            {
                string toFile = GetSuportedFile(toDir, targetFile);
                if (!string.IsNullOrEmpty(toFile))
                {
                    File.Delete(toFile);
                }
                File.Move(targetFile, Path.Combine(toDir, Path.GetFileName(targetFile)));
            }
        }
        #endregion

        #region Misc
        public static string GetNextFolder(string folder)
        {
            if (folder is null)
            {
                return null;
            }

            string baseDir = Path.GetDirectoryName(folder);
            string target = folder;
            return Directory.EnumerateDirectories(baseDir)
                .Where(f => string.Compare(f, target, StringComparison.OrdinalIgnoreCase) > 0)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        public static List<string> GetSortedDirectories(string dir)
        {
            return Directory.EnumerateDirectories(dir)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static string GetMiniPath(string path)
        {
            string dir = Path.GetFileName(Path.GetDirectoryName(path));
            string name = Path.GetFileName(path);
            return Path.Combine(dir, name);
        }
        #endregion
    }
}
