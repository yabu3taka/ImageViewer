using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.Data
{
    internal class ConverterFolderSetting(string dir)
    {
        private readonly string _noSmallFile = Path.Combine(dir, "no-small.dat");

        public bool NoSmall
        {
            get => File.Exists(_noSmallFile);
            set
            {
                try
                {
                    if (value)
                    {
                        File.Create(_noSmallFile);
                    }
                    else
                    {
                        File.Delete(_noSmallFile);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
