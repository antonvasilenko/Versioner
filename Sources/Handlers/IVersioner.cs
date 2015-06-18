using System;

namespace Versioner.Handlers
{
	public interface IVersioner : IDisposable
	{
		bool CanHandle(string filePath);
		void Init(string filePath);
		Version FetchVersion();
		void UpdateVersion(Version version);
	}
}