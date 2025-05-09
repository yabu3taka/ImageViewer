using System;
using System.Collections.Generic;
using System.IO;

using ImageConverter.Util;

namespace ImageConverter.Work
{
    /// <summary>
    /// マスク化変換
    /// </summary>
    abstract class MaskConvertWorkProcessor : LocalWorkProcessor
    {
        protected readonly MaskConverter _converter;

        public MaskConvertWorkProcessor(string toDir, MaskConverter c) : base(toDir)
        {
            _converter = c;
        }

        public MaskConvertWorkProcessor(string toDir, MaskConvertWorkProcessor me) : base(toDir)
        {
            _converter = me._converter;
        }

        #region WorkData
        public override void SyncSubFolder(WorkData work)
        {
            work.DeleteSubFolderIfNoOrig(ToRoot);
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddCommonConvertRule();
            target.DeleteNoOrigMode = true;
            return true;
        }

        public override void CopyIndexFile(FilePair pair)
        {
            var exthash = new Dictionary<string, string>() {
                { ".jpg", ".isd" },
                { ".png", ".isd" }
            };

            pair.ConvertWriter((reader, writer) =>
            {
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine().ToLowerInvariant();
                    writer.WriteLine(Path.GetFileNameWithoutExtension(str) + exthash[Path.GetExtension(str)]);
                }
            });
        }

        public override void CopySettingFile(FilePair pair)
        {
            pair.Copy();
        }
        #endregion
    }

    /// <summary>
    /// マスク化処理
    /// </summary>
    class MaskConverter(byte mask)
    {
        private readonly byte _mask = mask;

        public void SavePassword(MaskConvertWorkProcessor worker, string pass)
        {
            using var sw = new StreamWriter(worker.PassFile, false, FilePair.FileEncode);
            sw.NewLine = "\n";
            sw.WriteLine(CryptUtil.GetMD5("pass=" + pass));
            sw.WriteLine(CryptUtil.GetMD5("s=" + pass + "=" + _mask.ToString()));
        }

        public void ConvertStream(Stream inStream, Stream outStream)
        {
            byte[] bs = new byte[1024];
            for (; ; )
            {
                int len = inStream.Read(bs, 0, bs.Length);
                if (len == 0)
                {
                    break;
                }
                for (int i = 0; i < len; ++i)
                {
                    bs[i] = (byte)(bs[i] ^ _mask);
                }
                outStream.Write(bs, 0, len);
            }
        }
    }
}
