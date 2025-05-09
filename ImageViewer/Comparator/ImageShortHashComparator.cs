using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ImageViewer.Util;

namespace ImageViewer.Comparator
{
    class ImageShortHash : ImageHash
    {
        private readonly short[,] _gray;
        private readonly short _ave;

        // ulong
        private readonly ulong[] _row;
        const int BitmapSize = 64;

        public ImageShortHash(string file, short[,] gray, short ave) : base(file)
        {
            _gray = gray;
            _ave = ave;
            _row = new ulong[gray.GetLength(1)];

            for (int j = 0; j < _row.Length; ++j)
            {
                _row[j] = NumberUtil.GetBitValue(v => v >= ave, gray, j);
            }
        }

        public int DiffSimple(ImageShortHash v)
        {
            return _row.Zip(v._row, (u1, u2) => NumberUtil.CountBit(u1 ^ u2)).Sum();
        }

        public int LeftDiff(ImageShortHash v, short ave)
        {
            int sum = 0;
            for (int j = 0; j < _row.Length; ++j)
            {
                ulong row = NumberUtil.GetBitValue(gv => gv >= ave, v._gray, j);
                sum += NumberUtil.CountBit(_row[j] ^ row);
            }
            return sum;
        }

        public int Diff(ImageShortHash v)
        {
            int[] alist = [
                LeftDiff(v, _ave),
                v.LeftDiff(this, v._ave),
                DiffSimple(v)
            ];
            return alist.Max();
        }

        public static Bitmap GetMiniBitmap(Bitmap orig)
        {
            return BitmapUtil.ChangeSize(orig, BitmapSize, BitmapSize, false);
        }
    }

    abstract class ImageShortHashComparator(int closeness) : IImageComparator<ImageShortHash>, IImageComparationManagerFactory
    {
        public int Closeness { get; set; } = closeness;

        public ImageShortHash CreateHash(string file)
        {
            using var orig = new Bitmap(file);
            using var hashImg = ImageShortHash.GetMiniBitmap(orig);
            short[,] gray = GetGrayScaleMatrix(hashImg);
            short ave = MatrixUtil.Average(gray);
            return new ImageShortHash(file, gray, ave);
        }

        protected abstract short[,] GetGrayScaleMatrix(Bitmap b);

        public ImageCloseFunc<ImageShortHash> GetCloseFunc()
        {
            return IsClose;
        }

        private bool IsClose(ImageShortHash ih1, ImageShortHash ih2)
        {
            return ih1.Diff(ih2) <= Closeness;
        }

        public abstract IImageComparationManager ReplaceManager(IImageComparationManager man);
    }

    class ImageAveComparator(int closeness) : ImageShortHashComparator(closeness)
    {
        protected override short[,] GetGrayScaleMatrix(Bitmap from)
        {
            return BitmapUtil.Process(from, c => (short)(c.R + c.G + c.B));
        }

        public override IImageComparationManager ReplaceManager(IImageComparationManager man)
        {
            if (man is ImageComparationManager<ImageShortHash> shman)
            {
                if (shman.Comparator is ImageAveComparator avec)
                {
                    avec.Closeness = Closeness;
                    return man;
                }
            }

            return new ImageComparationManager<ImageShortHash>(this);
        }
    }

    class ImageGrayScaleComparator(int closeness) : ImageShortHashComparator(closeness)
    {
        protected override short[,] GetGrayScaleMatrix(Bitmap from)
        {
            const int v1 = 299 * 3, v2 = 587 * 3, v3 = 114 * 3;
            return BitmapUtil.Process(from, c => (short)((v1 * c.R + v2 * c.G + v3 * c.B) / 1000));
        }

        public override IImageComparationManager ReplaceManager(IImageComparationManager man)
        {
            if (man is ImageComparationManager<ImageShortHash> shman)
            {
                if (shman.Comparator is ImageGrayScaleComparator avec)
                {
                    avec.Closeness = Closeness;
                    return man;
                }
            }

            return new ImageComparationManager<ImageShortHash>(this);
        }
    }
}
