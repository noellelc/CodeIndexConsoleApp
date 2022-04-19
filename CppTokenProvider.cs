using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace TesterConsoleApp
{
    public class CppTokenProvider
    {
        public string InputPath { get; set; }
        public string IndexerPath { get; set; } = "cppindexer.exe";

        public ICollection<IndexFormatRange> RelevantRanges = new List<IndexFormatRange>();

        public IEnumerable<Range> GetRanges()
        {
            var tokenJsonString = RunCppIndexer();
            var ranges = JsonSerializer.Deserialize<IEnumerable<Range>>(tokenJsonString);
            return ranges;
        }

        private string RunCppIndexer()
        {
            try
            {
                var sb = new StringBuilder();
                var mre = new ManualResetEvent(false);

                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo
                    {
                        FileName = IndexerPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = InputPath,
                    };

                    p.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            mre.Set();
                            return;
                        }

                        try
                        {
                            var range = JsonSerializer.Deserialize<IndexFormatRange>(e.Data);
                            RelevantRanges.Add(range);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    };

                    p.Start();
                    p.BeginOutputReadLine();
                    mre.WaitOne();

                    return sb.ToString();
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return string.Empty;
            }
        }
    }
}
