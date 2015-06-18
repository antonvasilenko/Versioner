using System;
using System.IO;
using System.Xml.Linq;

namespace Versioner.Handlers
{
    public class TouchVersioner : IVersioner
    {
        private string _filePath;
        private XDocument _xDoc;


        public bool CanHandle(string filePath)
        {
            return false;
        }

        public void Init(string filePath)
        {
            _filePath = filePath;
            _xDoc = XDocument.Load(_filePath);
        }

        public string FetchVersionText()
        {
            var textValue = _xDoc.Root.Attribute("android:versionName").Value;

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