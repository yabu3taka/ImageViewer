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

namespace ImageViewer
{
    internal class AppSetting
    {
        public string ConvertCommand { get; set; }
        public string ConvertParam { get; set; }
        public string ConvertParamWidth { get; set; }
        public string ConvertOutput { get; set; }
        public bool? ConvertWait { get; set; }

        public string UrlCommand { get; set; }

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

                _setting.ConvertCommand ??= "";
                _setting.ConvertParam ??= "";
                _setting.ConvertParamWidth ??= "";
                _setting.ConvertOutput ??= "";
                _setting.ConvertWait ??= true;

                _setting.UrlCommand ??= "";

                return _setting;
            }
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, _option);
            File.WriteAllText(DefaultFile, json);
        }
        #endregion
    }
}
