using System.Collections.Generic;
using Versioner.Handlers;

namespace Versioner
{
    internal class Runner
    {
        private Options _options;

        public Runner(Options options)
        {
            _options = options;
        }

        public void Run()
        {
            Lo.Details("Searching strings in project(s) folders\n");
            foreach (var handler in GetHandlers())
            {
                if (handler.CanHandle(_options.FilePath))
                {
                    handler.Init(_options.FilePath);
                    if (_options.ReadMode)
                    {
                        var version = handler.FetchVersion();
                        Lo.Data(version.ToString());
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