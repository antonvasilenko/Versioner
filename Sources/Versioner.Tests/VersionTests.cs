﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

            var versioner2 = new CsharpVersioner();
            versioner2.Init(res1Path);
            var versionToCheck = versioner2.FetchVersion();
            Assert.AreEqual(vExpected.ToString(), versionToCheck.ToString());
        }
    }

    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void Ctor_int()
        {
            var version = new Version(1, 2, 3, 4);
            Assert.False(version.IsMask);
            Assert.AreEqual(1, version.A);
            Assert.AreEqual(2, version.B);
            Assert.AreEqual(3, version.C);
            Assert.AreEqual(4, version.D);
        }

        [Test]
        public void Ctor_int_mask()
        {
            var version = new Version(1, null, 3, null);
            Assert.True(version.IsMask);
            Assert.AreEqual(1, version.A);
            Assert.AreEqual(null, version.B);
            Assert.AreEqual(3, version.C);
            Assert.AreEqual(null, version.D);
        }

        [Test]
        public void Ctor_array()
        {
            var version = new Version(new uint?[] { 2, 3, 4, 5 });
            Assert.False(version.IsMask);
            Assert.AreEqual(2, version.A);
            Assert.AreEqual(3, version.B);
            Assert.AreEqual(4, version.C);
            Assert.AreEqual(5, version.D);
        }

        [Test]
        public void Ctor_String()
        {
            var textVersion = "1.2.3.4";
            var version = new Version(textVersion);
            Assert.False(version.IsMask);
            Assert.AreEqual(1, version.A);
            Assert.AreEqual(2, version.B);
            Assert.AreEqual(3, version.C);
            Assert.AreEqual(4, version.D);
        }

        [Test]
        public void Ctor_String_Mask()
        {
            var textVersion = "#.#.3.4";
            var version = new Version(textVersion);
            Assert.True(version.IsMask);
            Assert.AreEqual(null, version.A);
            Assert.AreEqual(null, version.B);
            Assert.AreEqual(3, version.C);
            Assert.AreEqual(4, version.D);
        }

        [TestCase("1.2..3.4", ExpectedException = typeof(ArgumentException))]
        [TestCase("1.2.3.4.5", ExpectedException = typeof(ArgumentException))]
        [TestCase("a.2.3.4", ExpectedException = typeof(ArgumentException))]
        [TestCase("1.2", ExpectedException = typeof(ArgumentException))]
        [TestCase("-1.2.3.4", ExpectedException = typeof(ArgumentException))]
        [TestCase("1,2,3,4", ExpectedException = typeof(ArgumentException))]
        [TestCase("1.2.3.##", ExpectedException = typeof(ArgumentException))]
        public void Ctor_String_Format_error(string textVersion)
        {
            var ver = new Version(textVersion);
        }

        [Test, TestCaseSource("ToStringFormatCases")]
        public void Version_toString(Version ver, string expected)
        {
            Assert.AreEqual(expected, ver.ToString());
        }

        private static readonly object[] ToStringFormatCases =
        {
            new object[]{new Version(1,2,3,4), "1.2.3.4"},
            new object[]{new Version(1,null,3,null), "1.#.3.#"},
            new object[]{new Version(new uint?[]{1,1,0,0}), "1.1.0.0"},
            new object[]{new Version(new uint?[]{1,1,null,null}), "1.1.#.#"},
        };

        public void ApplyTo_Mask_string()
        {
            var ver = new Version("#.#.10.20");

            var newVer = ver.ApplyTo("1.2.3.4");

            Assert.False(newVer.IsMask);
            Assert.AreNotSame(ver, newVer);
            Assert.AreEqual("1.2.10.20", newVer.ToString());
        }

        [TestCase("9.9.10.20", "1.2.3.4", false, "9.9.10.20")]
        [TestCase("9.9.10.20", "1.2.#.#", false, "9.9.10.20")]
        [TestCase("9.9.10.#", "1.2.3.4", false, "9.9.10.4")]
        [TestCase("9.9.10.#", "1.2.#.4", false, "9.9.10.4")]
        [TestCase("9.9.10.#", "1.2.3.4", false, "9.9.10.4")]
        [TestCase("9.9.10.#", "1.2.3.#", true, "9.9.10.#")]
        public void ApplyTo_Version_string(string verA, string verB, bool isMask, string verResult)
        {
            var ver = new Version(verA);

            var newVer = ver.ApplyTo(verB);

            Assert.AreEqual(isMask, newVer.IsMask);
            Assert.AreNotSame(ver, newVer);
            Assert.AreEqual(verResult, newVer.ToString());
        }


    }
}
