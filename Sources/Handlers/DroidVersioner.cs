using System;
using System.IO;
using System.Xml.Linq;

namespace Versioner.Handlers
{
    public class DroidVersioner : IVersioner
    {
        private readonly XNamespace _androidNamespace = "http://schemas.android.com/apk/res/android";
        private string _filePath;
        private XDocument _xDoc;


        public bool CanHandle(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (fileName == null) return false;

            return "AndroidManifest.xml".Equals(fileName, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Init(string filePath)
        {
            _filePath = filePath;
            _xDoc = XDocument.Load(_filePath);
        }

        public Version FetchVersion()
        {
            var attr = _xDoc.Root.Attribute(_androidNamespace + "versionName");
            if (attr == null) return null;
            
            return new Version(attr.Value);

        }

        public void UpdateVersion(Version version)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}