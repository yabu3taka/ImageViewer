using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ImageViewer
{
    class ProcessCommuniation(string pipeName)
    {
        public void StartServer(Action<List<string>> a)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var list = new List<string>();

                    using (var pipeServer = new NamedPipeServerStream(pipeName))
                    {
                        pipeServer.WaitForConnection();

                        using var reader = new StreamReader(pipeServer);
                        while (!reader.EndOfStream)
                        {
                            string file = reader.ReadLine();
                            list.Add(file);
                        }
                    }

                    a(list);
                }
            });
        }

        public async Task StartClient(List<string> files)
        {
            try
            {
                using var pipeClient = new NamedPipeClientStream(pipeName);
                await pipeClient.ConnectAsync(5000);

                using var writer = new StreamWriter(pipeClient);
                foreach (string f in files)
                {
                    await writer.WriteLineAsync(f);
                }
            }
            catch (TimeoutException)
            {
                // タイムアウト時の処理
            }
            catch (Exception)
            {
                // その他のエラー
            }
        }
    }
}
