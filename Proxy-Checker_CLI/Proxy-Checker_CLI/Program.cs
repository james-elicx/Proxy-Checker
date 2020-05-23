using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;

namespace Proxy_Checker_CLI
{
	class Program
	{
		static void Main(string[] args)
		{
            bool show_help = true;
            string proxiesFile = "";
            string outputFile = $"{Directory.GetCurrentDirectory()}\\working_proxies.txt";
            int threads = 1;
            int timeout = 2000;

            var p = new OptionSet() {
                "Usage: Proxy-Checker_CLI [OPTIONS]",
                "",
                "Options:",
                {
                    "f|file=",
                    "A text file containing proxies to check e.g. '--file proxies.txt'",
                    (string v) => proxiesFile = v
                },
                {
                    "o|output=",
                    "Text file to output working proxies to. If a file already exists, working proxies will be appended to the end of the file. e.g. '--output working_proxies.txt' DEFAULT: working_proxies.txt",
                    (string v) => outputFile = v
                },
                {
                    "t|threads=",
                    "Number of threads to use e.g. '--threads 10' (DEFAULT: 1)",
                    (int v) => threads = v
                },
                {
                    "x|timeout=",
                    "Timeout when testing proxies in milliseconds e.g. '--timeout 1000' DEFAULT: 2000",
                    (int v) => timeout = v
                },
                {
                    "h|help",
                    "Show this message and exit e.g. '--help'",
                    v => show_help = v != null
                },
            };

            List<string> extra;

            try
            {
                extra = p.Parse(args);
                foreach (string item in extra)
                {
                    throw new OptionException($"Option does not exist", item);
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine($"Error: {e.Message} - '{e.OptionName}'");
                Console.WriteLine("Try using '--help' for a list of valid options.");
                return;
            }

            DisplayHeader();

            if (proxiesFile == "")
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("Missing input for the file.");
            }
            else
            {
                CheckProxies(proxiesFile, outputFile, threads, timeout);
                return;
            }

            if (show_help)
            {
                p.WriteOptionDescriptions(Console.Out);
                return;
            }
        }

        private static void DisplayHeader()
        {
            Console.Clear();
            Console.WriteLine("|--------------------------------|");
            Console.WriteLine("|                                |");
            Console.WriteLine("|    HTTP Proxy Checker (CLI)    |");
            Console.WriteLine("|       github.com/moodiest      |");
            Console.WriteLine("|          Version 1.0.0         |");
            Console.WriteLine("|                                |");
            Console.WriteLine("|--------------------------------|");
            Console.WriteLine();
            Console.WriteLine();
        }
        private static void DisplaySettings(int threads, int timeout)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"Maximum threads: {threads}");
            Console.WriteLine($"Timeout: {timeout}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        private static void DisplayStatus(int totalProxies, int checkedProxies, int workingProxies, int failedProxies)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 4);
            Console.WriteLine($"Proxies Checked: {checkedProxies}/{totalProxies}");
            Console.WriteLine($"Working: {workingProxies}");
            Console.WriteLine($"Failed: {failedProxies}");
            Console.WriteLine();
        }

        private static void CheckProxies(string proxiesFile, string outputFile, int threads, int timeout)
        {
            string[] proxiesList;
            List<string> workingProxiesList = new List<string>();

            try
            {
                proxiesList = File.ReadAllLines(proxiesFile);
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine($"Error reading proxies file - {e.Message}");
                return;
            }

            int totalProxies = proxiesList.Length;
            int checkedProxies = 0;
            int workingProxies = 0;
            int failedProxies = 0;

            DisplaySettings(threads, timeout);

            Parallel.ForEach(proxiesList, new ParallelOptions { MaxDegreeOfParallelism = threads }, p =>
            {

                if (Thread.CurrentThread.ManagedThreadId == 1)
                {
                    DisplayStatus(totalProxies, checkedProxies, workingProxies, failedProxies);
                }

                try
                {
                    WebRequest webRequest = WebRequest.Create("https://api.ipify.org");
                    WebProxy webProxy = new WebProxy(p);
                    webRequest.Proxy = webProxy;
                    webRequest.Timeout = timeout;

                    WebResponse webResponse = webRequest.GetResponse();
                    Stream stream = webResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(stream);

                    var result = streamReader.ReadToEnd();

                    if (result == p.ToString().Split(':')[0])
                    {
                        workingProxiesList.Add(p);
                        checkedProxies += 1;
                        workingProxies += 1;
                    }
                    else
                    {
                        checkedProxies += 1;
                        failedProxies += 1;
                    }

                    streamReader.Close();
                    stream.Close();
                    webResponse.Close();
                }
                catch (WebException e)
                {
                    if (e.Message.ToLower().Contains("timed out"))
                    {
                        //Console.WriteLine($"Timeout: {p}");
                    }
                    checkedProxies += 1;
                    failedProxies += 1;
                }
                catch (OperationCanceledException e)
                {
                    checkedProxies += 1;
                    failedProxies += 1;
                }
                catch (Exception e)
                {
                    checkedProxies += 1;
                    failedProxies += 1;
                    Console.WriteLine($"Error Occured: {e.Message}");
                    if (workingProxies > 0)
                    {
                        Console.WriteLine("Outputting working proxies.");
                        File.AppendAllLines(outputFile, workingProxiesList);
                        Console.WriteLine($"Working proxies outputted to {outputFile}");
                    }
                    return;
                }
            });

            DisplayStatus(totalProxies, checkedProxies, workingProxies, failedProxies);

            if (workingProxies > 0)
            {
                Console.WriteLine("Outputting working proxies.");
                File.AppendAllLines(outputFile, workingProxiesList);
                Console.WriteLine($"Working proxies outputted to {outputFile}");
            }
            else
            {
                Console.WriteLine("No working proxies.");
            }
        }
	}
}
