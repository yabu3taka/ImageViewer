using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageViewer.Util;

namespace ImageViewer.UI
{
    abstract class BrowserDragDropFile(string filename)
    {
        public string FileName { get; } = filename;

        public abstract void Save(string filepath);
    }

    partial class BrowserDragDrop(IDataObject data)
    {
        private const string BROWSER_DATA = "HTML Format";

        [GeneratedRegex(@"<img[^/]src=""([^""]*)""")]
        private static partial Regex ImgTagRegex();

        public bool Valid
        {
            get { return data.GetDataPresent(BROWSER_DATA); }
        }

        private Uri GetImageUri()
        {
            string html = data.GetData(BROWSER_DATA).ToString();
            var match = ImgTagRegex().Match(html);
            if (match.Success)
            {
                return new Uri(match.Groups[1].Value);
            }
            else
            {
                return null;
            }
        }

        public BrowserDragDropFile GetImageFileFromUri(FileUtil.FileExt supportedExts)
        {
            Uri uri = GetImageUri();
            if (uri is null)
            {
                return null;
            }

            string filename = Path.GetFileName(uri.AbsolutePath);
            if (!supportedExts.IsSupported(filename))
            {
                return null;
            }
            return new BrowserDragDropFileDl(filename, uri);
        }

        public BrowserDragDropFile GetImageFileDirect(FileUtil.FileExt supportedExts)
        {
            //var dataObject = new OutlookDataObject(mData);
            Uri uri = GetImageUri();

            string[] filenames = DataObjectUtil.GetFileGroupDescriptorW(data);

            for (int i = 0; i < filenames.Length; i++)
            {
                string filename = filenames[i];
                if (supportedExts.IsSupported(filename))
                {
                    return GetDirectAction(filename, i);
                }
                if (WicBitmapUtil.ImageExt.IsSupported(filename))
                {
                    return GetWicDirectAction(filename, i);
                }
            }

            if (uri != null)
            {
                if (filenames.Length == 1)
                {
                    string filename = Path.GetFileName(uri.AbsolutePath);
                    if (supportedExts.IsSupported(filename))
                    {
                        return GetDirectAction(filename, 0);
                    }
                    if (WicBitmapUtil.ImageExt.IsSupported(filename))
                    {
                        return GetWicDirectAction(filename, 0);
                    }
                }
            }

            return null;
        }

        private BrowserDragDropFileStream GetDirectAction(string filename, int pos)
        {
            MemoryStream filestream = DataObjectUtil.GetFileContents(data, pos);
            if (filestream.Length == 0)
            {
                return null;
            }

            try
            {
                var bmp = new Bitmap(filestream);
                return new BrowserDragDropFileStream(filename, filestream);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private BrowserDragDropFileStream GetWicDirectAction(string filename, int pos)
        {
            MemoryStream filestream = DataObjectUtil.GetFileContents(data, pos);
            if (filestream.Length == 0)
            {
                return null;
            }

            try
            {
                var newstream = WicBitmapUtil.CreateJpegStream(filestream);
                return new BrowserDragDropFileStream(FileUtil.ChangeExt(filename, ".jpg"), newstream);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private class BrowserDragDropFileDl(string filename, Uri uri) : BrowserDragDropFile(filename)
        {
            public override void Save(string filepath)
            {
                using var webClient = new HttpClient();
                Task t = DownloadFileTaskAsync(webClient, uri, filepath);
                t.Wait();
            }

            public static async Task DownloadFileTaskAsync(HttpClient client, Uri uri, string filepath)
            {
                using var s = await client.GetStreamAsync(uri);
                using var fs = new FileStream(filepath, FileMode.CreateNew);
                await s.CopyToAsync(fs);
            }
        }

        private class BrowserDragDropFileStream(string filename, MemoryStream stream) : BrowserDragDropFile(filename)
        {
            public override void Save(string filepath)
            {
                using var outputStream = File.Create(filepath);
                stream.WriteTo(outputStream);
            }
        }
    }
}
