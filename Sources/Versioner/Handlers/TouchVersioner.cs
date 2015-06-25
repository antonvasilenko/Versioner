using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Versioner.Extensions;

namespace Versioner.Handlers
{
    /// <summary>
    /// Works with presumption that:
    /// CFBundleShortVersionString - should contain release version visible in AppStore (so a.b.c would work)
    /// CFBundleVersion - should contain build version ('d' part which is build number)
    /// More here: http://stackoverflow.com/questions/6876923/difference-between-xcode-version-cfbundleshortversionstring-and-build-cfbundl
    /// </summary>
    public class TouchVersioner : IVersioner
    {
        private const string BundleShortVersionKey = "CFBundleShortVersionString";
        private const string BundleVersionKey = "CFBundleVersion";
        private string _filePath;
        private XDocument _xDoc;

        public bool CanHandle(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return "info.plist".Equals(fileName, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Init(string filePath)
        {
            if (!CanHandle(filePath))
                throw new InvalidOperationException(string.Format("{0}: Cannot handle file {1}", GetType().Name, filePath));

            _filePath = filePath;
            _xDoc = XDocument.Load(_filePath);
        }

        public Version FetchVersion()
        {
            var shortVersionString = GetStringValueElement(BundleShortVersionKey);
            if (shortVersionString == null || String.IsNullOrWhiteSpace(shortVersionString.Value))
                throw new InvalidOperationException(string.Format("{0} element should exist in {1}", BundleShortVersionKey, _filePath));

            var bundleVersion = GetStringValueElement(BundleVersionKey);
            if (bundleVersion == null || String.IsNullOrWhiteSpace(bundleVersion.Value))
                throw new InvalidOperationException(string.Format("{0} element should exist in {1}", BundleVersionKey, _filePath));

            uint bundleVersionNumber;
            if (!uint.TryParse(bundleVersion.Value.Trim('\'', '.', ',', ' '), out bundleVersionNumber))
            {
                throw new InvalidOperationException(string.Format("{0} in {1} expected to be single number with no extra characters ", BundleVersionKey, _filePath));
            }

            var finalStringVersion = string.Join(".", shortVersionString.Value.Replace(',', '.').Trim('.', ' ', ','), bundleVersionNumber);

            return new Version(finalStringVersion);
        }

        private XElement GetStringValueElement(string keyElementName)
        {
            if (_xDoc.Root == null) return null;
            return _xDoc.XPathSelectElement(string.Format("plist/dict/key[text()='{0}']/following-sibling::string", keyElementName));
        }

        public void UpdateVersion(Version versionMask)
        {
            var shortVersionElement = GetStringValueElement(BundleShortVersionKey);
            var bundleVersionElement = GetStringValueElement(BundleVersionKey);

            if (shortVersionElement != null && !string.IsNullOrEmpty(shortVersionElement.Value) &&
                bundleVersionElement != null && !string.IsNullOrEmpty(bundleVersionElement.Value))
            {
                var shortVersionText = shortVersionElement.Value.Replace(',', '.').Trim('.', ' ', ',');
                var bundleVersionText = bundleVersionElement.Value.Replace(',', '.').Trim('.', ' ', ',');
                var initialVersion = string.Join(".", shortVersionText, bundleVersionText);

                var newVersion = versionMask.ApplyTo(initialVersion);

                if (newVersion.ToString() != initialVersion)
                {
                    Lo.Details("{0} updated from {1} to {2}\n", BundleShortVersionKey, shortVersionElement.Value, newVersion.ToTouchShortVersion());
                    Lo.Details("{0} updated from {1} to {2}\n", BundleVersionKey, bundleVersionElement.Value, newVersion.ToTouchBundleVersion());
                    shortVersionElement.SetValue(newVersion.ToTouchShortVersion());
                    bundleVersionElement.SetValue(newVersion.ToTouchBundleVersion());

                }
                _xDoc.Save(_filePath, SaveOptions.None);
            }
        }

        public void Dispose()
        {
        }
    }
}