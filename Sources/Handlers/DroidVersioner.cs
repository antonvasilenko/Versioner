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
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var fileExt = Path.GetExtension(filePath);
            if (fileName == null || fileExt == null) return false;

            return fileName.EndsWith("manifest", StringComparison.InvariantCultureIgnoreCase) &&
                   ".xml".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Init(string filePath)
        {
            _filePath = filePath;
            _xDoc = XDocument.Load(_filePath);
        }

        public Version FetchVersion()
        {
            var attr = GetRootAttr("versionName");
            if (attr == null) return null;

            return new Version(attr.Value);
        }

        public void UpdateVersion(Version versionMask)
        {
            var attr = GetRootAttr("versionName");
            if (attr != null)
            {
                var newVersion = versionMask.ApplyTo(attr.Value);
                attr.SetValue(newVersion);

                var attrCode = GetRootAttr("versionCode");
                if (attrCode != null)
                {
                    // additionally put 'c' part from string versionName to versionCode
                    // 'c' usually stands for revision; droid versionCode should be unique
                    // so the mathc each other perfectly
                    attrCode.SetValue(newVersion.C.ToString());
                }
            }
            _xDoc.Save(_filePath, SaveOptions.None);
        }

        private XAttribute GetRootAttr(string name)
        {
            if (_xDoc.Root == null) return null;
            return _xDoc.Root.Attribute(_androidNamespace + name);

        }

        public void Dispose()
        {
        }
    }
}