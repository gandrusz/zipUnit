using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit;

namespace ZipUnitTests
{
    [TestFixture]
    public class AssertFileTests
    {
        [Test]
        public void WorksForIdenticalFiles()
        {
            AssertFile.AreEqual("TestFile1.bin", "TestFile1Copy.bin");
            AssertFile.AreEqual("TestFile1.txt", "TestFile1Copy.txt");
            AssertFile.AreEqual("TestFile1.xml", "TestFile1Copy.xml");
        }

        [Test]
        public void ThrowsForBinaryDifference()
        {
            var exception = Assert.Throws<ZipUnitAssertException>(() => AssertFile.AreEqual("TestFile1.bin", "TestFile2.bin"));
            Assert.That(exception.Message, Is.StringContaining("Binary"));
        }

        [Test]
        public void ThrowsForTextDifference()
        {
            var exception = Assert.Throws<ZipUnitAssertException>(() => AssertFile.AreEqual("TestFile1.txt", "TestFile2.txt"));
            Assert.That(exception.Message, Is.StringContaining("Text"));
        }

        [Test]
        public void ThrowsForXmlDifference()
        {
            var exception = Assert.Throws<ZipUnitAssertException>(() => AssertFile.AreEqual("TestFile1.xml", "TestFile2.xml"));
            Assert.That(exception.Message, Is.StringContaining("Xml"));
        }
    }
}
