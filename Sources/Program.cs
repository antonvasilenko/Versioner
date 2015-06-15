using System.IO;
using JetBrains.Annotations;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Versioner
{
	public class UserInput
	{
		public enum WorkMode { Undefined, Read, Update }

		public string FilePath { get; set; }

		public string VersionMask { get; set; }

		public WorkMode Mode { get; set; }

		public Verbosity OutputLevel { get; set; }

		public bool ShowHelp { get; set; }
	}

	public enum Verbosity { Quiet = 1, Normal = 2, Detailed  = 3 }

	class Program
	{
		private static UserInput _userInput;
		public static Verbosity LogLevel;

		static void Main(string[] args)
		{
			LogLevel = Verbosity.Normal;

			

			OptionSet options;
			_userInput = ParseStartupOptions(args, out options);
			var analyzeRes = AnalyzeStartupOptions(_userInput);

			if (analyzeRes)
			{
				LogLevel = _userInput.OutputLevel;
			}
			Console.WriteLine("Versioner");
			Console.WriteLine("Copyright antonv 2015");

			if (!analyzeRes || _userInput.ShowHelp)
			{
				Console.WriteLine();
				Console.WriteLine("Usage: Versioner --file:\"C:\\work\\AssemblyInfo.cs\"  --read --quiet");
				Console.WriteLine("Usage: Versioner --file:\"C:\\work\\info.plist\"  --update --normal");
				Console.WriteLine();
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			Console.WriteLine("Searching strings in project(s) folders");
		}

		private static OptionSet PrepareOptionsParser(UserInput inputToFill)
		{
			var opts = inputToFill;
			var lParser = new OptionSet
			{
                    {"f|file=", "path to file that contains version (.cs .manifest.xml info.plist)", filePath => opts.FilePath = filePath},
                    {"v|version=", "version mask to update file with, i.e. 1.0.3.4, *.*.*.1342", v => opts.VersionMask = v},
                    {"r|read", "use to read out version from file", r => {
						                                                     if (r != null)
																			     opts.Mode = UserInput.WorkMode.Read;
																		 }},
					{"u|update", "use to update version in file", r => {
						                                                     if (r != null)
																			     opts.Mode = UserInput.WorkMode.Update;
																		 }},
					{"q|quiet", "set output level to return no log messages", flag => 
					                                                     {
						                                                     if (flag != null)
																			     opts.OutputLevel = Verbosity.Quiet;
																		 }},
		            {"n|normal", "set output level to return useful log messages", flag => 
					                                                     {
						                                                     if (flag != null)
																				 opts.OutputLevel = Verbosity.Normal;
																		 }},
			        {"d|detailed", "set output level to return all log messages", flag => 
					                                                     {
						                                                     if (flag != null)
																				 opts.OutputLevel = Verbosity.Detailed;
																		 }},
                    {"h|?|help", "shows this help message and exit", h=> opts.ShowHelp = h != null}
                };
			return lParser;
		}

		private static UserInput ParseStartupOptions(IEnumerable<string> args, out OptionSet options)
		{
			var userInput = new UserInput
			{
				OutputLevel = Verbosity.Normal,
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

		private static bool AnalyzeStartupOptions(UserInput input)
		{
			if (input.ShowHelp)
				return true;

			if (input.OutputLevel != Verbosity.Quiet)
			{
				Console.WriteLine("Output level set to {0}. Consider using quiet in pipelines to return result only", input.OutputLevel);
			}

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

			if (input.Mode == UserInput.WorkMode.Undefined)
			{
				Console.WriteLine("Mode not provided, use '-r|-read' to read or '-u|-update' to update version");
				return false;
			}

			if (input.Mode == UserInput.WorkMode.Update)
			{
				if (String.IsNullOrEmpty(input.VersionMask))
				{
					Console.WriteLine("Version mask should be provided for 'update' mode, use '-v|-version' to set it");
					return false;
				}
				var parts = input.VersionMask.Split('.').ToList();
				if (parts.Count != 4)
				{
					Console.WriteLine("Version mask should in format 'a.b.c.d' where each letter stands for positive number or #");
				}
				var errors = new StringBuilder();
				foreach (var part in parts)
				{
					if (part == "#")
						continue;
					uint number;
					if (!uint.TryParse(part, out number))
					{
						errors.AppendFormat("Version part '{0}' cannot be parsed neither as positive number nor as #", part);
						errors.AppendLine();
					}
				}
				if (errors.Length > 0)
					Console.WriteLine(errors.ToString());

			}
			return true;
		}
	}

	public static class Logger
	{
		[StringFormatMethod("template")]
		private static void Data(string template, params object[] args)
		{
			Console.Write(template, args);
		}

		private static void Log(string template, params object[] args)
		{
			if ()
			Console.Write(template, args);
		}
	}

	public class Version
	{
		public string A { get; set; }
		public string B { get; set; }
		public string C { get; set; }
		public string D { get; set; }
	}
}
