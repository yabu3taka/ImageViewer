using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageConverter.Util
{
    class CryptUtil
    {
        public static byte GetMask(string dir)
        {
            using var sr = new StreamReader(Path.Combine(dir, "mask.txt"), Encoding.UTF8);
            string line = sr.ReadLine();
            int v = Convert.ToInt32(line, 16);
            if (v == 0)
            {
                throw new IOException();
            }
            return (byte)(0xFF & v);
        }

        public static string GetMD5(string p)
        {
            byte[] data = Encoding.UTF8.GetBytes(p);
            byte[] bs = MD5.HashData(data);

            var result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }
    }
}
