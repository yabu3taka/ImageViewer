using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ImageViewer.Util;

namespace ImageViewer.Manager
{
    #region FileNameStyle 
    interface IFileNameStyleFactory
    {
        FileNameStyle Make(string fname);
    }

    class FileNamePrefixStyleFactory(string name, Regex regex) : IFileNameStyleFactory
    {
        public override string ToString()
        {
            return name;
        }

        public FileNameStyle Make(string fname)
        {
            Match m = regex.Match(FileNameUtil.ToCleanName(fname));
            if (!m.Success)
            {
                return null;
            }
            string prefix = FileNameUtil.GetResult(m, "p");
            string serial = FileNameUtil.GetResult(m, "s");
            return FileNameStyle.ForSerial(prefix, serial, "");
        }
    }

    class FileNameStyleDatePattern(string name, Regex regex) : IFileNameStyleFactory
    {
        public override string ToString()
        {
            return name;
        }

        public FileNameStyle Make(string fname)
        {
            Match m = regex.Match(Path.GetFileNameWithoutExtension(fname));
            if (!m.Success)
            {
                return null;
            }
            string prefix = FileNameUtil.GetResult(m, "p");
            string serial = FileNameUtil.GetResult(m, "s");
            return FileNameStyle.ForDate(prefix, serial, "");
        }
    }

    partial class FileNameStyle
    {
        public string Prefix { get; }
        public string Postfix { get; }
        public IFileNameSerialFactory SerialFac { get; }
        private readonly string _startSerial;

        private FileNameStyle(string prefix, string postfix, IFileNameSerialFactory serialFac, string startSerial)
        {
            Prefix = prefix;
            Postfix = postfix;
            SerialFac = serialFac;
            _startSerial = startSerial;
        }

        [GeneratedRegex(@"\(([0-9]+|[t])\)")]
        private static partial Regex NameRegex();

        public static FileNameStyle FromName(string name)
        {
            var r = NameRegex();
            Match m = r.Match(name);
            if (!m.Success)
            {
                return null;
            }

            string prefix = name[..m.Index];
            string type = m.Groups[1].Value;
            string postfix = name[(m.Index + m.Length)..];
            if (type == "t")
            {
                return ForDate(prefix, "", postfix);
            }
            else
            {
                return new FileNameStyle(prefix, postfix,
                    new FileNameSerialIntFactory(int.Parse(type)), "");
            }
        }

        public static FileNameStyle ForDate(string prefix, string serial, string postfix)
        {
            return new FileNameStyle(prefix, postfix,
                new FileNameSerialDateFactory(), serial);
        }

        public static FileNameStyle ForSerial(string prefix, string serial, string postfix)
        {
            return new FileNameStyle(prefix, postfix,
                new FileNameSerialIntFactory(serial.Length), serial);
        }

        public override string ToString()
        {
            if (SerialFac is null)
            {
                return Prefix + Postfix;
            }
            else
            {
                return Prefix + SerialFac.PatternStr + Postfix;
            }
        }

        private bool IsContained(string name)
        {
            name = FileNameUtil.ToCleanName(name);
            if (!name.StartsWith(Prefix))
            {
                return false;
            }
            if (!name.EndsWith(Postfix))
            {
                return false;
            }
            return true;
        }

        public bool IsContained(IFileInfo file)
        {
            return IsContained(file.FileName);
        }

        public bool GetBody(string name, out string body)
        {
            body = "";

            name = FileNameUtil.ToCleanName(name);
            if (!IsContained(name))
            {
                return false;
            }

            body = name.Substring(Prefix.Length, name.Length - Prefix.Length - Postfix.Length);
            return true;
        }

        public bool GetBody(IFileInfo file, out string body)
        {
            return GetBody(file.FileName, out body);
        }

        public string MakeFileName(string body, string ext)
        {
            return Prefix + body + Postfix + ext;
        }

        public IFileNameGenerator MakeInitGenerator()
        {
            return new FileNameGenerator(this, SerialFac.CreateSerial("", 1));
        }

        public IFileNameGenerator MakeCurrentGenerator()
        {
            var serial = SerialFac.CreateSerial(_startSerial, 1);
            return serial.Valid ? new FileNameGenerator(this, serial) : null;
        }

        public IFileNameGenerator MakeGenerator()
        {
            return MakeCurrentGenerator() ?? MakeInitGenerator();
        }
    }
    #endregion

    #region FileName Serial Number
    interface IFileNameSerialFactory
    {
        string PatternStr { get; }
        IFileNameSerial CreateSerial(string value, int direction);
    }

    interface IFileNameSerial
    {
        bool Valid { get; }
        bool NextSerial();
    }

    class FileNameSerialInvalid : IFileNameSerial
    {
        public bool Valid { get { return false; } }
        public bool NextSerial()
        {
            return false;
        }
    }

    class FileNameSerialIntFactory(int keta) : IFileNameSerialFactory
    {
        public string PatternStr => $"({keta})";
        public IFileNameSerial CreateSerial(string str, int direction)
        {
            return new FileNameSerial(NumberUtil.ToInt32WithDefault(str, 1), keta, (int)Math.Pow(10, keta), direction);
        }

        class FileNameSerial(int serial, int keta, int max, int direction) : IFileNameSerial
        {
            public bool Valid => 0 < serial && serial < max;

            public bool NextSerial()
            {
                serial += direction;
                return Valid;
            }

            public override string ToString()
            {
                return serial.ToString($"D{keta}");
            }
        }
    }

    class FileNameSerialDateFactory : IFileNameSerialFactory
    {
        public string PatternStr => "(t)";

        public IFileNameSerial CreateSerial(string value, int direction)
        {
            if (direction > 0)
            {
                return new FileNameSerial(NumberUtil.ToInt32WithDefault(value, 0));
            }
            else
            {
                return new FileNameSerialInvalid();
            }
        }

        class FileNameSerial(int serial) : IFileNameSerial
        {
            public bool Valid
            {
                get
                {
                    if ((serial % 100) >= 60)
                    {
                        return false;
                    }
                    if ((serial % 10000) >= 6000)
                    {
                        return false;
                    }
                    if (serial >= 240000)
                    {
                        return false;
                    }
                    return true;
                }
            }

            public bool NextSerial()
            {
                serial++;
                if ((serial % 100) >= 60)
                {
                    serial += 40;
                }
                if ((serial % 10000) >= 6000)
                {
                    serial += 4000;
                }
                if (serial >= 240000)
                {
                    return false;
                }
                return true;
            }

            public override string ToString()
            {
                return serial.ToString("D6");
            }
        }
    }
    #endregion

    #region FileName Generator 
    interface IFileNameGenerator
    {
        bool NextSerial();
        string MakeFileName(string origName);
    }

    class FileNameGenerator(FileNameStyle style, IFileNameSerial serial) : IFileNameGenerator
    {
        public bool NextSerial()
        {
            return serial.NextSerial();
        }

        public string MakeFileName(string origName)
        {
            return style.MakeFileName(serial.ToString(), Path.GetExtension(origName));
        }
    }
    #endregion

    #region FileGroup
    class FileGroup(FileNameStyle style, IFileInfo finfo) : IFileInfo
    {
        public FileNameStyle Style { get; } = style;
        public IReadOnlyList<IFileInfo> Items => _items;

        private readonly List<IFileInfo> _items = [finfo];

        public bool IsContained(IFileInfo finfo)
        {
            return Style?.IsContained(finfo) ?? false;
        }

        public bool Add(IFileInfo finfo)
        {
            if (IsContained(finfo))
            {
                this._items.Add(finfo);
                return true;
            }
            return false;
        }

        public IFileManager MyManager => _items[0].MyManager;
        public IFileImageInfo GetImageInfo()
        {
            return _items[0].GetImageInfo();
        }

        public int Id => _items[0].Id;

        public string FileName => _items[0].FileName;
        public string FilePath => _items[0].FilePath;
        public string Title => _items[0].Title;
    }
    #endregion

    #region Util
    static partial class FileNameUtil
    {
        [GeneratedRegex(@"^(?<p>\d{4}-)(?<s>\d{6})\D?")]
        private static partial Regex DateRegex();

        [GeneratedRegex(@"^(?<p>.*\D)(?<s>\d+)$")]
        private static partial Regex NameRegex();

        private static readonly List<IFileNameStyleFactory> _list = [
            new FileNameStyleDatePattern("mmdd-hhmmss", DateRegex()),
            new FileNamePrefixStyleFactory("前方一致", NameRegex()),
        ];

        public static string GetResult(Match m, string type)
        {
            return m.Groups[type].Success ? m.Groups[type].Value : "";
        }

        public static string ToCleanName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        public static FileNameStyle CreateAuto(string fname)
        {
            fname = ToCleanName(fname);
            foreach (var fpat in _list)
            {
                var ret = fpat.Make(fname);
                if (ret is not null)
                {
                    return ret;
                }
            }
            return null;
        }

        public static FileNameStyle CreateAuto(IFileInfo file)
        {
            return CreateAuto(file.FileName);
        }

        public static FileActionSerialRename SerialConvert(this IEnumerable<IFileInfo> list, IRealFolder folder, IFileNameGenerator gene)
        {
            var ret = new FileActionSerialRename();
            foreach (var finfo in list)
            {
                ret.Add(new FileRenameAction(finfo, folder, gene.MakeFileName(finfo.FileName)));
                if (!gene.NextSerial())
                {
                    return null;
                }
            }
            return ret;
        }

        public static FileActionSerialRename RenameGroup(this IEnumerable<IFileInfo> list, FileNameStyle fromStyle, FileNameStyle toStyle)
        {
            if (fromStyle.ToString() == toStyle.ToString())
            {
                return null;
            }

            var ret = new FileActionSerialRename();
            foreach (var finfo in list)
            {
                if (fromStyle.GetBody(finfo, out string body))
                {
                    ret.Add(new FileRenameAction(finfo, finfo.GetParent(), toStyle.MakeFileName(body, finfo.GetExt())));
                }
            }
            return ret;
        }

        public static FileActionSerialRename SwapConvert(this IFileManager fman, FileNameStyle fromStyle, FileNameStyle toStyle)
        {
            if (fromStyle.ToString() == toStyle.ToString())
            {
                return null;
            }

            var list = fman.GetAllItems();
            var ret = list.RenameGroup(fromStyle, toStyle);
            ret.Merge(list.RenameGroup(toStyle, fromStyle));
            return ret;
        }

        public static string FindNonExist(this IRealFolder folder, string origName, IFileNameGenerator gene)
        {
            string newName = gene.MakeFileName(origName);
            while (folder.HasFile(newName))
            {
                if (!gene.NextSerial())
                {
                    return "";
                }
                newName = gene.MakeFileName(origName);
            }
            return newName;
        }

        public static List<FileGroup> GetFileGroupList(this IEnumerable<IFileInfo> finfos, Func<IFileInfo, FileNameStyle> f)
        {
            var ret = new List<FileGroup>();
            foreach (var finfo in finfos)
            {
                bool added = false;
                foreach (var g in ret)
                {
                    if (g.Add(finfo))
                    {
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    ret.Add(new FileGroup(f(finfo), finfo));
                }
            }
            return ret;
        }

        public static List<FileGroup> GetFileGroupList(this IEnumerable<IFileInfo> finfos)
        {
            return GetFileGroupList(finfos, FileNameUtil.CreateAuto);
        }
    }
    #endregion
}
