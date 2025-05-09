using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageViewer.Manager;

namespace ImageViewer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var files = new List<string>(Environment.GetCommandLineArgs());
            files.RemoveAt(0);

            var mutex = new Mutex(false, Application.ProductName);
            var commu = new ProcessCommuniation(Application.ProductName);
            if (mutex.WaitOne(0, false))
            {
                Directory.SetCurrentDirectory(@"C:\");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var f = new SingleForm(files);
                void ff(List<string> list)
                {
                    var loader = FileLoaderUtil.Create(list);
                    f.TabManager.OpenFileLoader(loader);
                    f.Activate();
                }
                void a(List<string> list)
                {
                    if (f.InvokeRequired)
                    {
                        f.Invoke((MethodInvoker)(() => ff(list)));
                    }
                    else
                    {
                        ff(list);
                    }
                }
                commu.StartServer(a);
                Application.Run(f);
            }
            else
            {
                Task.WaitAll(commu.StartClient(files));
            }
            mutex.Close();
        }

        #region SerialFileNames
        public static StringCollection SerialFileNames
        {
            get
            {
                var setting = Properties.Settings.Default;
                if (setting.SerialFileNames is null)
                {
                    setting.SerialFileNames = new StringCollection();
                }
                return setting.SerialFileNames;
            }
        }

        public static void AddSerialFileName(string name)
        {
            var col = SerialFileNames;
            col.Remove(name);
            col.Insert(0, name);

            const int MAX = 20;
            while (col.Count > MAX)
            {
                col.RemoveAt(MAX);
            }
            Properties.Settings.Default.Save();
        }

        public static void SetupSerialFileNames(ComboBox cb)
        {
            cb.Items.Clear();
            foreach (string name in SerialFileNames)
            {
                cb.Items.Add(name);
            }
        }
        #endregion

        #region FilterFileNames
        public static StringCollection FilterFileNames
        {
            get
            {
                var setting = Properties.Settings.Default;
                setting.FilterFileNames ??= new StringCollection();
                return setting.FilterFileNames;
            }
        }

        public static void AddFilterFileName(string name)
        {
            var col = FilterFileNames;
            col.Remove(name);
            col.Insert(0, name);

            const int MAX = 20;
            while (col.Count > MAX)
            {
                col.RemoveAt(MAX);
            }
            Properties.Settings.Default.Save();
        }

        public static void SetupFilterFileNames(ComboBox cb)
        {
            cb.Items.Clear();
            foreach (string name in SerialFileNames)
            {
                cb.Items.Add(name);
            }
        }

        public static void SetupFilterFileNames(TextBox tb)
        {
            var source = new AutoCompleteStringCollection();
            foreach (string name in FilterFileNames)
            {
                source.Add(name);
            }
            tb.AutoCompleteCustomSource = source;
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        #endregion
    }
}
