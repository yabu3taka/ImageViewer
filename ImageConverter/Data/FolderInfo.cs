using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageConverter.Manager;
using ImageConverter.Work;

namespace ImageConverter.Data
{
    public class FolderInfo(string dir, string resultBaseDir) : IEquatable<FolderInfo>
    {
        public string Dir { get; } = dir;
        public DateTime LastWirteDate { get; private set; } = DateTime.MaxValue;
        public string FolderTitle { get; private set; } = "";
        public WorkModifiedType DiffType { get; set; } = WorkModifiedType.NONE;
        public WorkModification Modification { get; set; } = null;
        public bool NoSmall { get; private set; } = false;
        public int FileCount { get; private set; } = 0;

        public void UpdateHeavyInfo()
        {
            this.FileCount = 0;

            string resultDir = Path.Combine(resultBaseDir, Path.GetFileName(Dir));
            if (Directory.Exists(resultDir))
            {
                var result = DateTime.MinValue;
                void addFile(string target)
                {
                    DateTime d = File.GetLastWriteTime(target);
                    if (result < d)
                    {
                        result = d;
                    }
                    this.FileCount++;
                }

                foreach (string target in Directory.EnumerateFiles(resultDir, "*.isd"))
                {
                    addFile(target);
                }
                var files = new string[] { "index.dat", "setting.dat" };
                foreach (string file in files)
                {
                    string path = Path.Combine(Dir, file);
                    if (File.Exists(path))
                    {
                        addFile(path);
                    }
                }

                LastWirteDate = result;
            }

            var setting = new FolderTextSetting(Dir);
            FolderTitle = setting.FolderTitle;

            UpdateNoSmall();
        }

        public void UpdateNoSmall()
        {
            var setting = new ConverterFolderSetting(Dir);
            this.NoSmall = setting.NoSmall;
        }

        public override string ToString()
        {
            string kome = DiffType switch
            {
                WorkModifiedType.NONE => "",
                WorkModifiedType.COPY => "C",
                WorkModifiedType.UPDATE => "*",
                WorkModifiedType.DELETE => "*",
                _ => throw new NotImplementedException()
            };
            string small = this.NoSmall ? "-" : "";
            return $"{small}{Path.GetFileName(Dir)} ({LastWirteDate:yyyy/MM/dd} C{FileCount}) [{FolderTitle}]{kome}";
        }

        public override bool Equals(object obj)
        {
            if (obj is FolderInfo v)
            {
                return this.Dir == v.Dir;
            }
            return false;
        }

        public bool Equals(FolderInfo other)
        {
            return this.Dir == other.Dir;
        }

        public override int GetHashCode()
        {
            return Dir.GetHashCode();
        }

        public bool NotMatch(string str)
        {
            if (FolderTitle.Contains(str, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (Path.GetFileName(Dir).Contains(str, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static int CompareModified(FolderInfo v1, FolderInfo v2)
        {
            int ret = -v1.DiffType.CompareTo(v2.DiffType);
            if (ret == 0)
            {
                ret = CompareName(v1, v2);
            }
            return ret;
        }

        public static int CompareLastWirteDate(FolderInfo v1, FolderInfo v2)
        {
            return -v1.LastWirteDate.CompareTo(v2.LastWirteDate);
        }

        public static int CompareName(FolderInfo v1, FolderInfo v2)
        {
            return v1.Dir.CompareTo(v2.Dir);
        }
    }
}
