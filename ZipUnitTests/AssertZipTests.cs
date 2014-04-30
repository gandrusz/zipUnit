using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit;

namespace ZipUnitTests
{
    [TestFixture]
    public class AssertZipTests
    {
        [Test]
        public void PassesForSameFiles()
        {
            AssertZip.AreEqual("TestZip1.zip", "TestZip1.zip");
            AssertZip.AreEqual("TestZip2.zip", "TestZip2.zip");
            AssertZip.AreEqual("TestZip3.zip", "TestZip3.zip");
        }

        [Test]
        public void FailsWhenContentsDifferent()
        {
            try
            {
                AssertZip.AreEqual("TestZip2.zip", "TestZip1.zip");
                Assert.Fail("Expected exception");
            }
            catch (ZipUnitAssertException exception)
            {
                Assert.AreEqual(0, exception.Report.Missing.Count);
                Assert.AreEqual(0, exception.Report.Additional.Count);
                Assert.AreEqual(1, exception.Report.FileDifferences.Count);
                Assert.AreEqual("A.txt", exception.Report.FileDifferences[0].FullName);
            }
        }

        [Test]
        public void FailsWhenDifferentFiles()
        {
            try
            {
                AssertZip.AreEqual("TestZip3.zip", "TestZip1.zip");
                Assert.Fail("Expected exception");
            }
            catch (ZipUnitAssertException exception)
            {
                CollectionAssert.AreEqual(new[] { "C.txt" }, exception.Report.Missing);
                CollectionAssert.AreEqual(new[] { "A.txt" }, exception.Report.Additional);
                Assert.AreEqual(0, exception.Report.FileDifferences.Count);
            }
        }

       
    }
}
