using System;
using System.Collections.Generic;
using System.Data.HashFunction.CityHash;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace TesterConsoleApp
{
    class Program
    {
        private static readonly ICityHash CityHash = CityHashFactory.Instance.Create(new CityHashConfig()
        {
            HashSizeInBits = 128,
        });

        static void Main(string[] args)
        {
            // var hash = GetCityHashInHex("CloudKernel#M:CloudKernel.Contracts.ICloudWorkspaceManager.GetSnapshotByCommitIdAsync(System.String,System.Threading.CancellationToken)#cancellationToken");
            // Console.WriteLine(hash);

            // testing deserialization of object returned from AzDO drop
            // var stopwatch = Stopwatch.StartNew();
            // var items = LoadJsonFile("C:/Users/noecald/Downloads/manifest (9).json");
            // Console.WriteLine(stopwatch.ElapsedMilliseconds);
            // Newtonsoft.Json: 5348, 5065, 5162
            // System.Text.Json: 4133, 4556, 3973

            var test1 = RemoveSubstring("fffffoxoxoxooxxxxx", "fox");
            Console.WriteLine(test1 == "ffooxxxxx");
            var test2 = RemoveSubstring("barbarbarbarbrbrbbaarr", "ar");
            Console.WriteLine(test2 == "bbbbbrbrbb");
        }

        static string RemoveSubstring(string original, string toRemove)
        {
            StringBuilder letters = new StringBuilder();
            foreach (char letter in original)
            {
                letters.Append(letter);
                // if there are at least the right number of letters and the current letter matches the last letter of the substring 
                if (letters.Length >= toRemove.Length && toRemove[toRemove.Length-1] == letter) {
                    // see if all the previous letters are a full match
                    int subI = toRemove.Length - 1;
                    int mainI = letters.Length - 1;
                    while (subI >= 0 && toRemove[subI] == letters[mainI])
                    {
                        subI--;
                        mainI--;
                    }
                    
                    // if all the letters matched, get rid of em
                    if (subI == -1)
                    {
                        letters.Remove(++mainI, toRemove.Length);
                    }
                }
            }

            return letters.ToString();
        }

        static void DownloadFile()
        {
            string fullUrl = "todo: construct uri";
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("User-Agent", "noecald");
            var token = "mytoken"; // replace with token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var webClient = new WebClient();
            webClient.DownloadFile(fullUrl, "downloadhere");
        }

        static string GetCityHashInHex(string sourceString)
        {
            StringBuilder hash = new StringBuilder();
            byte[] result = CityHash.ComputeHash(Encoding.Unicode.GetBytes(sourceString)).Hash;
            foreach (byte b in result)
            {
                hash.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return hash.ToString();
        }

        static List<DropItem> LoadJsonFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                var items = System.Text.Json.JsonSerializer.Deserialize<List<DropItem>>(json);
                return items;
            }
        }
    }
}
