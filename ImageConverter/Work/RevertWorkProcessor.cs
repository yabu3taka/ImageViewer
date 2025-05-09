using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageConverter.Work
{
    /// <summary>
    /// 逆変換
    /// </summary>
    class RevertWorkProcessor(string toDir, MaskConverter converter) : LocalWorkProcessor(toDir)
    {
        private readonly MaskConverter _converter = converter;

        #region SubFolderWorkData
        public override void SetupSetting(WorkData work)
        {
            work.Setting.RescanDir = true;
        }

        public override void SyncSubFolder(WorkData work)
        {
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddConvertRule(".isd", "");
            return true;
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.ToFile.ExistsExt(".png") || pair.ToFile.ExistsExt(".jpg");
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            // 入力
            using Stream rfs = pair.GetInStream();
            using MemoryStream mfs = new();
            _converter.ConvertStream(rfs, mfs);
            mfs.Flush();

            // 種類判定
            mfs.Position = 0;
            using (Image img = Image.FromStream(mfs))
            {
                string ext;
                if (img.RawFormat.Equals(ImageFormat.Png))
                {
                    ext = ".png";
                }
                else
                {
                    ext = ".jpg";
                }
                pair.ChangeExtOfToFile(ext);
            }

            // 出力
            mfs.Position = 0;
            using Stream wfs = pair.GetOutStream();
            mfs.CopyTo(wfs);
        }

        public override void CopyIndexFile(FilePair pair)
        {
        }

        public override void CopySettingFile(FilePair pair)
        {
        }
        #endregion
    }
}
