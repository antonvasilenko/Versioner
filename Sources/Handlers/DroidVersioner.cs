using System;

namespace Versioner.Handlers
{
    public class DroidVersioner : IVersioner
    {
        public bool CanHandle(string filePath)
        {
            return false;
        }

        public void Init(string filePath)
        {
            throw new NotImplementedException();
        }

        public Version FetchVersion()
        {
            throw new NotImplementedException();
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