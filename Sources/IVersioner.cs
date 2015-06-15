using System;

namespace Versioner
{
	public interface IVersioner : IDisposable
	{
		bool CanHandle(string filePath);
		void Init(string filePath);
		Version FetchVersion();
		void UpdateVersion(Version version);
	}

	public class CsharpVersioner : IVersioner
	{
		public bool CanHandle(string filePath)
		{
			throw new NotImplementedException();
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