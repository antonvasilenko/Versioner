using System;
using NUnit.Framework;
using Versioner.Handlers;

namespace Versioner.Tests
{
    [TestFixture]
    public class CSharpVersionerTests
    {
        private string res1Path = "TestResources\\CSharpRes.cs";

        [Test]
        public void Ctor_no_crash()
        {
            var versioner = new CsharpVersioner();
        }

        [Test]
        public void Init_can_handle_cs_files_only()
        {
            var versioner = new CsharpVersioner();
            Assert.True(versioner.CanHandle(res1Path));
            Assert.False(versioner.CanHandle("test.xml"));
            Assert.False(versioner.CanHandle("test.json"));
            Assert.False(versioner.CanHandle("cs.xml"));
        }

        [Test]
        public void Init_reads_existing_file_fine()
        {
            var versioner = new CsharpVersioner();
            versioner.Init(res1Path);
        }

        [Test]
        [ExpectedException]
        public void Init_throws_if_file_absent()
        {
            var versioner = new CsharpVersioner();
            versioner.Init("unavainable.dummy");
        }

        [Test]
        public void FetchVersion_reads_assembly_version()
        {
            var versioner = new CsharpVersioner();
            versioner.Init(res1Path);
            var version = versioner.FetchVersion();
            Assert.NotNull(version);
            Assert.AreEqual("1.2.3.4", version.ToString());
        }

        [Test]
        public void UpdateVersion_overrides_version()
        {
            var versioner = new CsharpVersioner();
            versioner.Init(res1Path);
            versioner.UpdateVersion(new Version("2.2.2.2"));

            var versioner2 = new CsharpVersioner();
            versioner2.Init(res1Path);
            var versionToCheck = versioner2.FetchVersion();
            Assert.AreEqual("2.2.2.2", versionToCheck.ToString());
        }

        [Test]
        public void UpdateVersion_applies_mask()
        {
            var versioner = new CsharpVersioner();
            versioner.Init(res1Path);
            var v1 = versioner.FetchVersion();
            var mask = new Version(null, null, null, v1.D + 1);
            var vExpected = mask.ApplyTo(v1);
            versioner.UpdateVersion(mask);

            Console.WriteLine(v1);
            Console.WriteLine(vExpected);

            var versioner2 = new CsharpVersioner();
            versioner2.Init(res1Path);
            var versionToCheck = versioner2.FetchVersion();
            Assert.AreEqual(vExpected.ToString(), versionToCheck.ToString());
        }
    }
}