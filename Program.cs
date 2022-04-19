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
            string fullUrl = "https://localhost:44303/v0.1/Diagnostics/documentSummary/7ae08b42ad8cf1b1116e7c147d0af24de9efb2c0063609a9a4369ad5d7ef8a/8d90b5ee22c49ab";
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("User-Agent", "noecald");
            var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiIzNmQzNjE0Yy04N2ZhLTQ1MTctODU4ZS01MTY2MDQ2YWQ0MmQiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3L3YyLjAiLCJpYXQiOjE2MjAwODA2OTksIm5iZiI6MTYyMDA4MDY5OSwiZXhwIjoxNjIwMDg0NTk5LCJhaW8iOiJBV1FBbS84VEFBQUFxMnpxVE53cUV0SkhlbU5kSi85MEdNMVd5cjd3Q3RLaDZONERvM1hndy9wUXJOK0FHalNKc0NvQ3hGM21KbEppVTNxVFUweUsvZEpCbTJvR0U3OUJ0N3NnaHlTRGczeFVNdS9TUTF5Vi9FVVBiSkJoV0Z5cTc3cTNGWkJJVjBZayIsImF6cCI6ImZmNjAwNzhmLTkyNTYtNDY4OC1iZWI5LTgwNzJjMGQ4N2Q5OCIsImF6cGFjciI6IjAiLCJuYW1lIjoiTm9lbGxlIENhbGR3ZWxsIiwib2lkIjoiM2UyMGM1Y2EtYmFmYy00YmU0LTk4ZDYtYTRiOGQ2ZjFhZGRjIiwicHJlZmVycmVkX3VzZXJuYW1lIjoibm9lY2FsZEBtaWNyb3NvZnQuY29tIiwicmgiOiIwLkFSb0F2NGo1Y3ZHR3IwR1JxeTE4MEJIYlI0OEhZUDlXa29oR3ZybUFjc0RZZlpnYUFQcy4iLCJyb2xlcyI6WyJWc2Nsa0FkbWluIl0sInNjcCI6InJlYWRfcHVibGljIHVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6ImtNLXNJOGlNZV9LZ28wM3p4UVkxajgwOF8zdXh4d0NEbndEeUQxY05vbW8iLCJ0aWQiOiI3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJ1dGkiOiJvV2JOMnlhbUwwLXBlMmpfaWhERUFBIiwidmVyIjoiMi4wIn0.FZktbs6J8wgVbyHOQBlmbLXPgPJUluiWgEbb_GfuLlOyu3Spvyv6BQGsNtGIWA4Ii0stHdVO58HfGogBHy5vVu5ZANZ-uaTeskiCv30gd0Sj909Ud1I4xQKRZvt5YdObJRU6CFttHtwYcUfxAw973tP5PAi2Wy2XJvejDSzHOzUne3UYKwQ8eLx09OK0eVkCCZazmKoYwyDANoTzKPtnUFHvEaz1YoxG7t8fokzbp8NR6_kWTPHB0xYYQqiLaB0SlbN-KZXVwNocdAuR9KqjxTGU-DQpRzcJsYDFGdWMj9kYzUTIPL5Fs1V1XS1Tm7jcyiJN0WdWHcD6v1sed7FGgQ";
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
