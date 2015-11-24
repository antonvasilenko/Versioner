using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Versioner.Handlers
{
    public class CsharpVersioner : IVersioner
    {
        private string _fileContent;
        private string _filePath;

        public bool CanHandle(string filePath)
        {
            var fileExt = Path.GetExtension(filePath);
            return ".cs".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Init(string filePath)
        {
            _filePath = filePath;
            _fileContent = File.ReadAllText(filePath);
        }

        public Version FetchVersion()
        {
            var parseResult = Regex.Match(_fileContent, @"AssemblyVersion\s*\(\s*""([0-9\.\*]+)""\s*\)\s*\]");
            if (parseResult.Success)
                return new Version(parseResult.Groups[1].Value);
            return null;
        }

        public void UpdateVersion(Version versionMask)
        {
            var parseResult = Regex.Match(_fileContent, @"AssemblyVersion\s*\(\s*""([0-9\.\*]+)""\s*\)\s*\]");
            if (parseResult.Success)
            {
                var oldVersion = new Version(parseResult.Groups[1].Value);
                var newVersion = versionMask.ApplyTo(oldVersion);
                _fileContent = _fileContent.Replace(parseResult.Groups[1].Value, newVersion.ToString());
                Lo.Details("VersionName updated from {0} to {1}\n", oldVersion, newVersion);
                File.WriteAllText(_filePath, _fileContent);
            }
        }

        public void Dispose()
        {
        }
    }
}