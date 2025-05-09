using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

using ImageConverter.Data;
using ImageConverter.GDrive;
using ImageConverter.Util;
using ImageConverter.Work;

namespace ImageConverter
{
    internal class AppSetting
    {
        public string FromDir { get; set; }
        public string ToDir { get; set; }
        public string SyncDir { get; set; }

        public string LightWeightToDir { get; set; }
        public int? LightWeightQuality { get; set; }
        public int? LightWeightWidth { get; set; }
        public int? LightWeightHeight { get; set; }

        public string GDriveNormalToDir { get; set; } = "";
        public string GDriveLightToDir { get; set; } = "";
        public string TabletToDir { get; set; } = "";

        public string ShowProg { get; set; } = "";
        public bool? ShowPreview { get; set; }

        public string UrlCommand { get; set; }

        public string Validate()
        {
            if (!Directory.Exists(FromDir))
            {
                return "変更元フォルダーを選択して下さい";
            }

            if (!Directory.Exists(ToDir))
            {
                return "変更先フォルダーを選択して下さい";
            }
            if (FileUtil.EqualsPath(FromDir, ToDir))
            {
                return "変更元と変更先が同じです";
            }

            if (!string.IsNullOrEmpty(LightWeightToDir))
            {
                if (!Directory.Exists(LightWeightToDir))
                {
                    return "軽量フォルダーを選択して下さい";
                }
                if (FileUtil.EqualsPath(FromDir, LightWeightToDir))
                {
                    return "変更元と軽量フォルダーが同じです";
                }
                if (FileUtil.EqualsPath(ToDir, LightWeightToDir))
                {
                    return "変更先と軽量フォルダーが同じです";
                }
            }

            if (!string.IsNullOrEmpty(ShowProg))
            {
                if (!File.Exists(ShowProg))
                {
                    return "プログラムを選択して下さい";
                }
            }

            byte mask;
            try
            {
                mask = CryptUtil.GetMask(FromDir);
            }
            catch (Exception)
            {
                return "マスクファイルが必要です";
            }

            return null;
        }

        public string ValidateSync()
        {
            if (!Directory.Exists(SyncDir))
            {
                return "同期先フォルダーを選択して下さい";
            }
            if (FileUtil.EqualsPath(SyncDir, ToDir))
            {
                return "変更先と同期先が同じです";
            }
            var file = GetPassFile(ToDir);
            if (!File.Exists(file))
            {
                return "パスワードファイルがありません";
            }
            return null;
        }

        public static string GetPassFile(string dir)
        {
            return Path.Combine(dir, "pass.dat");
        }

        #region 処理
        public List<FolderInfo> GetFolderList()
        {
            return Directory.EnumerateDirectories(FromDir)
                .Select(d => new FolderInfo(d, ToDir))
                .ToList();
        }

        public MaskConverter CreateMaskConverter()
        {
            try
            {
                byte mask = CryptUtil.GetMask(FromDir);
                return new MaskConverter(mask);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public NormalConvertWorkProcessor CreateNormalConvertWorkProcessor(MaskConverter maskp)
        {
            return new NormalConvertWorkProcessor(ToDir, maskp);
        }

        public LightConvertWorkProcessor CreateLightConvertWorkProcessor(NormalConvertWorkProcessor normalp)
        {
            if (string.IsNullOrEmpty(LightWeightToDir))
            {
                return null;
            }
            return new LightConvertWorkProcessor(LightWeightToDir,
                normalp,
                LightWeightQuality.Value,
                new Size(LightWeightWidth.Value, LightWeightHeight.Value));
        }

        public RevertWorkProcessor CreateRevertWorkProcessor(MaskConverter maskp)
        {
            return new RevertWorkProcessor(FromDir, maskp);
        }

        public RevertTextWorkProcessor CreateRevertTextWorkProcessor(MaskConverter maskp)
        {
            return new RevertTextWorkProcessor(FromDir, maskp);
        }

        private static GDriveWorkProcessor CreateGDrive(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return null;
            }
            var drive = GoogleDriveManager.GetDrive();
            return new GDriveWorkProcessor(drive, dir);
        }

        public GDriveWorkProcessor CreateNormalGDrive()
        {
            return CreateGDrive(GDriveNormalToDir);
        }

        public GDriveWorkProcessor CreateLightGDrive()
        {
            return CreateGDrive(GDriveLightToDir);
        }

        public SyncWorkProcessor CreateSyncWorkProcessor()
        {
            return new SyncWorkProcessor(SyncDir);
        }
        #endregion

        #region 設定取得
        private static AppSetting _setting = null;
        private static readonly JsonSerializerOptions _option = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private static string DefaultFile
        {
            get
            {
                Assembly myAssembly = Assembly.GetEntryAssembly();
                string path = Path.GetDirectoryName(myAssembly.Location);
                return Path.Combine(path, "setting.json");
            }
        }

        public static AppSetting Default
        {
            get
            {
                if (_setting is not null)
                {
                    return _setting;
                }

                if (File.Exists(DefaultFile))
                {
                    string json = File.ReadAllText(DefaultFile);
                    _setting = JsonSerializer.Deserialize<AppSetting>(json, _option);
                }
                else
                {
                    _setting = new AppSetting();
                }

                _setting.FromDir ??= "";
                _setting.ToDir ??= "";
                _setting.SyncDir ??= "";

                _setting.LightWeightToDir ??= "";
                _setting.LightWeightQuality ??= 90;
                _setting.LightWeightWidth ??= 1240;
                _setting.LightWeightHeight ??= 940;

                _setting.GDriveNormalToDir ??= "";
                _setting.GDriveLightToDir ??= "";
                _setting.TabletToDir ??= "";

                _setting.ShowProg ??= "";
                _setting.ShowPreview ??= false;

                _setting.UrlCommand ??= "";

                return _setting;
            }
        }

        public static AppSetting CreateEditData()
        {
            AppSetting ret = (AppSetting)Default.MemberwiseClone();
            return ret;
        }

        public static void CommitAndSave(AppSetting setting)
        {
            _setting = setting;
            _setting.Save();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, _option);
            File.WriteAllText(DefaultFile, json);
        }
        #endregion
    }
}
