using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using ImageConverter.Data;
using ImageConverter.Util;
using static System.Net.Mime.MediaTypeNames;

namespace ImageConverter.Work
{
    /// <summary>
    /// 軽量化変換
    /// </summary>
    class LightConvertWorkProcessor(string toDir, NormalConvertWorkProcessor proc, int quality, Size size) :
        MaskConvertWorkProcessor(toDir, proc)
    {
        private readonly int _quality = quality;
        private readonly NormalConvertWorkProcessor _proc = proc;
        private readonly Size _size = size;

        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            var setting = new ConverterFolderSetting(target.FromDir);
            if (setting.NoSmall)
            {
                target.ToDir.DeleteFolder();
                return false;
            }
            return base.ScanSubFolder(target);
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            var newPair = pair.CreateFilePairFor(_proc);
            return newPair.IsSameBySize();
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            string file = pair.FromFile;
            if (FileUtil.ContainExt(file, ".txt"))
            {
                pair.Convert(_converter.ConvertStream);
            }
            else
            {
                using Bitmap orig = pair.GetInBitmap();
                using Stream wfs = pair.GetOutStream();
                if (_size.Width >= orig.Width && _size.Height >= orig.Height)
                {
                    if (FileUtil.ContainExt(file, ".png"))
                    {
                        WriteJpeg(orig, wfs);
                    }
                    else
                    {
                        using Stream rfs = pair.GetInStream();
                        _converter.ConvertStream(rfs, wfs);
                    }
                }
                else
                {
                    using Bitmap target = ChangeSize(orig, _size);
                    WriteJpeg(target, wfs);
                }
            }
        }

        private void WriteJpeg(Bitmap bmp, Stream wfs)
        {
            using var mfs = new MemoryStream();
            var eps = new EncoderParameters(1);
            var ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)_quality);
            eps.Param[0] = ep;

            var ici = GetEncoderInfo(ImageFormat.Jpeg);

            bmp.Save(mfs, ici, eps);

            mfs.Flush();
            mfs.Position = 0;

            _converter.ConvertStream(mfs, wfs);
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat f)
        {
            foreach (ImageCodecInfo enc in ImageCodecInfo.GetImageEncoders())
            {
                if (enc.FormatID == f.Guid)
                {
                    return enc;
                }
            }
            return null;
        }

        private static Bitmap ChangeSize(Bitmap from, Size s)
        {
            double ratioWidth = (double)s.Width / from.Width;
            double ratioHeight = (double)s.Height / from.Height;
            int width, height;
            if (ratioWidth > ratioHeight)
            {
                width = (int)(from.Width * ratioHeight);
                height = s.Height;
            }
            else
            {
                width = s.Width;
                height = (int)(from.Height * ratioWidth);
            }

            var dest = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(dest))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(from, 0, 0, width, height);
            }
            return dest;
        }
    }
}
