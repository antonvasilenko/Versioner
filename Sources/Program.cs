using System.IO;
using System.Reflection;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Versioner.Handlers;

namespace Versioner
{
    public class UserInput
    {
        public string FilePath { get; set; }

        public string VersionMask { get; set; }

        public string Mode { get; set; }

        public string Verbosity { get; set; }

        public bool ShowHelp { get; set; }
    }

    public class Options
    {
        public string FilePath { get; set; }
        public bool ReadMode { get; set; }
        public Version VersionMask { get; set; }
    }

    public enum Verbosity { Data = 0, Normal = 1, Detailed  = 2 }

    class Program
    {
        private static string[] _verbosities = { "data", "normal", "detailed" };
        private static string[] _modes = { "read", "update" };
        private static UserInput _userInput;

        private static Verbosity _verbosity;
        private static Options _options;


        static void Main(string[] args)
        {
            Lo.Verbosity = Verbosity.Normal;

            OptionSet options;
            _userInput = ParseStartupOptions(args, out options);
            var analyzeRes = AnalyzeStartupOptions(_userInput, out _options);

            if (analyzeRes)
            {
                Lo.Verbosity = _verbosity;
            }
            Lo.Log("Versioner\n");
            Lo.Log("Copyright antonv 2015\n");

            if (!analyzeRes || _userInput.ShowHelp)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: Versioner --file:\"C:\\work\\AssemblyInfo.cs\"  --read --quiet");
                Console.WriteLine("Usage: Versioner --file:\"C:\\work\\info.plist\"  --update --normal");
                Console.WriteLine("Usage: Versioner --file:\"C:\\work\\android.manifest\"  --update --normal");
                Console.WriteLine();
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            new Runner(_options).Run();
        }

        private static T GetAttribute<T>() where T : Attribute
        {
            return typeof(Program).Assembly.GetCustomAttribute<T>();
        }

        private static OptionSet PrepareOptionsParser(UserInput inputToFill)
        {
            var opts = inputToFill;
            var lParser = new OptionSet
			{
                    {"f|file=", "path to file that contains version (.cs .manifest.xml info.plist)", filePath => opts.FilePath = filePath},
                    {"ver|version=", "version mask to update file with, i.e. 1.0.3.4, *.*.*.1342", v => opts.VersionMask = v},
                    {"m|mode=", string.Format("set the work mode for versioner ({0})", string.Join(" | ", _modes)), m => opts.Mode = m},
					{"v|verbosity=", string.Format("set output verbosity level ({0})", string.Join(" | ", _verbosities)), v => opts.Verbosity = v}, 
                    {"h|?|help", "shows this help message and exit", h=> opts.ShowHelp = h != null}
                };
            return lParser;
        }

        private static UserInput ParseStartupOptions(IEnumerable<string> args, out OptionSet options)
        {
            var userInput = new UserInput
            {
                Mode = "read",
                Verbosity = "normal",
                ShowHelp = false,
            };

            options = PrepareOptionsParser(userInput);
            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                userInput.ShowHelp = true;
            }
            return userInput;
        }

        private static bool AnalyzeStartupOptions(UserInput input, out Options options)
        {
            options = new Options();
            if (input.ShowHelp)
                return true;

            if (String.IsNullOrEmpty(input.FilePath))
            {
                Console.WriteLine("path to file with version not provided, use '-f' or '--file' option");
                return false;
            }
            if (!File.Exists(input.FilePath))
            {
                Console.WriteLine("File '{0}' doesn't exists. Check path in '-f' or '--files", input.FilePath);
                return false;
            }
            options.FilePath = input.FilePath;

            if (String.IsNullOrWhiteSpace(input.Verbosity))
            {
                Console.WriteLine("Output verbosity level not set.");
                return false;
            }

            var pos = Array.IndexOf(_verbosities, input.Verbosity.ToLowerInvariant());
            if (pos < 0)
            {
                Console.WriteLine("Unexpected output verbosity value. Allowed values are '{0}')", string.Join(", ", _verbosities));
                return false;
            }
            _verbosity = (Verbosity)pos;
            if (_verbosity != Verbosity.Data)
            {
                Lo.Log("For using versioner in pipelines consider use verbosity level '{0}' to return result only", Verbosity.Data);
            }

            if (String.IsNullOrWhiteSpace(input.Mode))
            {
                Console.WriteLine("Execution mode not set.");
                return false;
            }

            pos = Array.IndexOf(_modes, input.Mode.ToLowerInvariant());
            if (pos < 0)
            {
                Console.WriteLine("Unexpected execution mode. Allowed values are '{0}')", string.Join(", ", _modes));
                return false;
            }
            options.ReadMode = _modes[pos] == "read";

            if (!options.ReadMode)
            {
                try
                {
                    options.VersionMask = new Version(input.VersionMask);
                }
                catch (ArgumentException aex)
                {
                    Console.WriteLine(aex.Message);
                    return false;
                }
            }
            return true;
        }
    }

    internal class Runner
    {
        private Options _options;

        public Runner(Options options)
        {
            _options = options;
        }

        public void Run()
        {
            Lo.Details("Searching strings in project(s) folders");
            foreach (var handler in GetHandlers())
            {
                if (handler.CanHandle(_options.FilePath))
                {
                    handler.Init(_options.FilePath);
                    if (_options.ReadMode)
                    {
                        handler.FetchVersion();
                    }
                    else
                    {
                        handler.UpdateVersion(_options.VersionMask);
                    }
                }
            }
        }

        private static IEnumerable<IVersioner> GetHandlers()
        {
            yield return new CsharpVersioner();
            yield return new DroidVersioner();
            yield return new TouchVersioner();
        }
    }
}
