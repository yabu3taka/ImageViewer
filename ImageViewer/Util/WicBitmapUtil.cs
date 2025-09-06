using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using WIC;

namespace ImageViewer.Util
{
    internal class WicBitmapUtil
    {
        public static FileUtil.FileExt ImageExt = new(".avif", ".webp");

        private static MemoryStream CreateConvertedStream(Stream inStream, Guid format)
        {
            var wic = WICImagingFactory.Create();
            var decoder = wic.CreateDecoderFromStream(inStream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand);
            var frame = decoder.GetFrame(0);

            var memoryStream = new MemoryStream();

            var encoder = wic.CreateEncoder(format);
            encoder.Initialize(memoryStream.AsCOMStream(), WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

            var frameEncoder = encoder.CreateNewFrame();
            frameEncoder.Initialize(null);
            frameEncoder.SetSize(frame.GetSize());
            frameEncoder.SetResolution(frame.GetResolution());
            frameEncoder.SetPixelFormat(frame.GetPixelFormat());
            frameEncoder.WriteSource(frame);

            frameEncoder.Commit();
            encoder.Commit();

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public static MemoryStream CreateJpegStream(Stream inStream)
        {
            return CreateConvertedStream(inStream, ContainerFormat.Jpeg);
        }

        public static Bitmap CreateBitmap(Stream inStream)
        {
            using var stream = CreateConvertedStream(inStream, ContainerFormat.Png);
            return new Bitmap(stream);
        }
    }
}
