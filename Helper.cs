using System;
using System.Diagnostics;

namespace TesterConsoleApp
{
    public class Helper
    {
        public void ParseCPP(string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cppindexer.exe";
            startInfo.Arguments = filename;
            Process process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            process.OutputDataReceived += this.OnOutputDataReceived;

            process.Start();
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            Console.WriteLine("************************************************");
            if (args.Data != null)
            {
                Console.WriteLine(args.Data);
            }
        }
    }
}