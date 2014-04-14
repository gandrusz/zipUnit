using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit;

namespace ZipUnitTests
{
    [TestFixture]
    public class AssertXmlTests
    {
        [Test]
        public void WorksForSameXml()
        {
            AssertXml.AreEqual("<a/>", "<a/>");
        }

        [Test]
        public void ThrowsForDifferentXml()
        {
            Assert.Throws<ZipUnitAssertException>(() => AssertXml.AreEqual("<a/>", "<b/>"));
        }
    }
}
