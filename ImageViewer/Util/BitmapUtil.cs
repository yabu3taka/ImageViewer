using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageViewer.Util
{
    static class BitmapUtil
    {
        #region 画像情報
        public static Bitmap OpenBitmap(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                return new Bitmap(fs);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsSameSize(Bitmap from, Bitmap to)
        {
            return from.Width == to.Width && from.Height == to.Height && from.PixelFormat == to.PixelFormat;
        }

        public static int GetDepth(Bitmap from)
        {
            int pixelSize = Bitmap.GetPixelFormatSize(from.PixelFormat);
            if (pixelSize != 24 && pixelSize != 32)
            {
                return -1;
            }
            return pixelSize / 8;
        }
        #endregion

        #region サイズ変更
        public static Bitmap ChangeSize(Image from, int width, int height, InterpolationMode mode, CompositingQuality quality)
        {
            var dest = new Bitmap(width, height, from.PixelFormat);
            using (var g = Graphics.FromImage(dest))
            {
                g.InterpolationMode = mode;
                g.CompositingQuality = quality;
                g.DrawImage(from, 0, 0, width, height);
            }
            return dest;
        }

        public static Bitmap ChangeSize(Image from, int width, int height, bool smooth)
        {
            InterpolationMode mode;
            CompositingQuality quality;
            if (smooth)
            {
                mode = InterpolationMode.HighQualityBicubic;
                quality = CompositingQuality.HighQuality;
            }
            else
            {
                mode = InterpolationMode.NearestNeighbor;
                quality = CompositingQuality.HighSpeed;
            }
            return ChangeSize(from, width, height, mode, quality);
        }
        public static Bitmap ChangeSize(Image from, Size s, bool smooth)
        {
            return ChangeSize(from, s.Width, s.Height, smooth);
        }

        public static Bitmap ChangeSize(Image from, int width, int height)
        {
            return ChangeSize(from, width, height, true);
        }
        public static Bitmap ChangeSize(Image from, Size s)
        {
            return ChangeSize(from, s.Width, s.Height, true);
        }

        public static Bitmap Zoom(Image from, float ratio, bool smooth)
        {
            return ChangeSize(from, (int)(from.Width * ratio), (int)(from.Height * ratio), smooth);
        }
        public static Bitmap Zoom(Image from, float ratio)
        {
            return Zoom(from, ratio, true);
        }

        public static Bitmap AdjustSize(Image from, Size maxSize)
        {
            double ratioWidth = (double)maxSize.Width / from.Width;
            double ratioHeight = (double)maxSize.Height / from.Height;
            int width, height;
            if (ratioWidth > ratioHeight)
            {
                width = (int)(from.Width * ratioHeight);
                height = maxSize.Height;
            }
            else
            {
                width = maxSize.Width;
                height = (int)(from.Height * ratioWidth);
            }
            return ChangeSize(from, width, height, true);
        }
        #endregion

        #region 2ファイルの違いを取得(透過パッチ)
        public static Bitmap GetPatch(Bitmap from, Bitmap to)
        {
            if (!IsSameSize(from, to))
            {
                return null;
            }

            int depth = GetDepth(from);
            if (depth < 0)
            {
                return null;
            }

            int width = from.Width;
            int height = from.Height;
            var area = new Rectangle(0, 0, width, height);
            var ret = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            BitmapData fromData = from.LockBits(area, ImageLockMode.ReadOnly, from.PixelFormat);
            BitmapData toData = to.LockBits(area, ImageLockMode.ReadOnly, to.PixelFormat);
            BitmapData retData = ret.LockBits(area, ImageLockMode.WriteOnly, ret.PixelFormat);

            try
            {
                unsafe
                {
                    byte* fromAddr = (byte*)fromData.Scan0;
                    byte* toAddr = (byte*)toData.Scan0;
                    byte* retAddr = (byte*)retData.Scan0;
                    for (int y = 0; y < height; ++y)
                    {
                        int fromStride = fromData.Stride * y;
                        int retStride = retData.Stride * y;
                        for (int x = 0; x < width; ++x)
                        {
                            int fromPos = x * depth + fromStride;
                            int retPos = x * 4 + retStride;

                            bool same = true;
                            for (int i = 0; i < depth; ++i)
                            {
                                if (fromAddr[fromPos + i] != toAddr[fromPos + i])
                                {
                                    same = false;
                                    break;
                                }
                            }

                            if (same)
                            {
                                // ARGB
                                retAddr[retPos + 3] = 0;
                                retAddr[retPos + 2] = 0xff;
                                retAddr[retPos + 1] = 0xff;
                                retAddr[retPos] = 0xff;
                            }
                            else
                            {
                                retAddr[retPos + 3] = 0xff;
                                retAddr[retPos + 2] = toAddr[fromPos + 2];
                                retAddr[retPos + 1] = toAddr[fromPos + 1];
                                retAddr[retPos] = toAddr[fromPos];
                            }
                        }
                    }
                }
            }
            finally
            {
                ret.UnlockBits(retData);
                to.UnlockBits(toData);
                from.UnlockBits(fromData);
            }
            return ret;
        }
        #endregion

        #region 画像変換
        public static T[,] ProcessDiff<T>(Bitmap from, Bitmap to, Func<Color, Color, T> proc)
        {
            if (!IsSameSize(from, to))
            {
                return null;
            }

            int depth = GetDepth(from);
            if (depth < 0)
            {
                return null;
            }

            int width = from.Width;
            int height = from.Height;
            var area = new Rectangle(0, 0, width, height);

            BitmapData fromData = from.LockBits(area, ImageLockMode.ReadOnly, from.PixelFormat);
            BitmapData toData = to.LockBits(area, ImageLockMode.ReadOnly, to.PixelFormat);
            T[,] ret = new T[width, height];

            try
            {
                unsafe
                {
                    byte* fromAddr = (byte*)fromData.Scan0;
                    byte* toAddr = (byte*)toData.Scan0;
                    for (int y = 0; y < height; ++y)
                    {
                        int fromStride = fromData.Stride * y;
                        for (int x = 0; x < width; ++x)
                        {
                            int fromPos = x * depth + fromStride;
                            ret[x, y] = proc(
                                Color.FromArgb(fromAddr[fromPos + 2], fromAddr[fromPos + 1], fromAddr[fromPos]),
                                Color.FromArgb(toAddr[fromPos + 2], toAddr[fromPos + 1], toAddr[fromPos]));
                        }
                    }
                }
            }
            finally
            {
                to.UnlockBits(toData);
                from.UnlockBits(fromData);
            }

            return ret;
        }

        public static T[,] Process<T>(Bitmap from, Func<Color, T> proc)
        {
            int depth = BitmapUtil.GetDepth(from);
            if (depth < 0)
            {
                return null;
            }

            int width = from.Width;
            int height = from.Height;
            var area = new Rectangle(0, 0, width, height);
            T[,] ret = new T[width, height];

            BitmapData fromData = from.LockBits(area, ImageLockMode.ReadOnly, from.PixelFormat);
            try
            {
                unsafe
                {
                    byte* fromAddr = (byte*)fromData.Scan0;
                    for (int y = 0; y < height; ++y)
                    {
                        int fromStride = fromData.Stride * y;
                        for (int x = 0; x < width; ++x)
                        {
                            int fromPos = x * depth + fromStride;
                            ret[x, y] = proc(Color.FromArgb(fromAddr[fromPos + 2], fromAddr[fromPos + 1], fromAddr[fromPos]));
                        }
                    }
                }
            }
            finally
            {
                from.UnlockBits(fromData);
            }

            return ret;
        }
        #endregion

        #region グレースケール計算
        private static Bitmap CreateGrayscaleInternal(Bitmap from, float v1, float v2, float v3)
        {
            var ret = new Bitmap(from.Width, from.Height);
            using (var g = Graphics.FromImage(ret))
            {
                var cm = new ColorMatrix(
                    new float[][]{
                        new float[]{v1, v1, v1, 0 ,0},
                        new float[]{v2, v2, v2, 0, 0},
                        new float[]{v3, v3, v3, 0, 0},
                        new float[]{0, 0, 0, 1, 0},
                        new float[]{0, 0, 0, 0, 1}
                    });

                var attr = new ImageAttributes();
                attr.SetColorMatrix(cm);
                var rect = new Rectangle(0, 0, from.Width, from.Height);
                g.DrawImage(from, rect, 0, 0, from.Width, from.Height, GraphicsUnit.Pixel, attr);
            }
            return ret;
        }

        public static Bitmap CreateGrayscale(Bitmap from)
        {
            return CreateGrayscaleInternal(from, 0.299F, 0.587F, 0.114F);
        }

        public static Bitmap CreateAverageGrayscale(Bitmap from)
        {
            float v = 1F / 3F;
            return CreateGrayscaleInternal(from, v, v, v);
        }
        #endregion

        #region その他
        public static Bitmap CreateBoolBitmap(ulong[] rows, int cols, int dsize)
        {
            var ret = new Bitmap(cols * dsize, rows.Length * dsize);
            using (var g = Graphics.FromImage(ret))
            {
                g.Clear(Color.White);
                for (int y = 0; y < rows.Length; ++y)
                {
                    ulong target = rows[y];
                    for (int x = 0; x < cols; ++x)
                    {
                        ulong res = target & (1UL << x);
                        if (res > 0)
                        {
                            g.FillRectangle(Brushes.Black, x * dsize, y * dsize, dsize, dsize);
                        }
                    }
                }
            }
            return ret;
        }
        #endregion
    }
}
