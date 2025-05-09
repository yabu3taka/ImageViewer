using System;

namespace ImageConverter.Util
{
    internal class RunningSection
    {
        public bool Running { private set; get; }

        public void Run(Action a)
        {
            Running = true;
            try
            {
                a();
            }
            finally
            {
                Running = false;
            }
        }
    }
}