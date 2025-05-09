using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ImageConverter.Util
{
    class SimpleLogUtil
    {
        public static bool LogFlag { get; set; }

        private static string LogFile
        {
            get
            {
                Assembly myAssembly = Assembly.GetEntryAssembly();
                string path = Path.GetDirectoryName(myAssembly.Location);
                return Path.Combine(path, "log.txt");
            }
        }

        private static void Print(Type t, string format, params object[] args)
        {
            string body = string.Format(format, args);
            string line = string.Format("[{0,-20}] {1}", t.Name, body);
            File.AppendAllText(LogFile, line + Environment.NewLine);
        }

        public static void D(Type t, string format, params object[] args)
        {
            if (LogFlag)
            {
                Print(t, format, args);
            }
        }

        public static void I(Type t, string format, params object[] args)
        {
            Print(t, format, args);
        }

        public static void Ex(Type t, string method, Exception ex)
        {
            Print(t, "{0} Exception: {1} {2}", method, ex.Message, ex.StackTrace);
        }

        public static void Clear()
        {
            File.WriteAllText(LogFile, "");
        }
    }
}
