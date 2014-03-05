using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipUnit;

namespace ZipUnitTests
{
    [TestFixture]
    public class AssertThatTests
    {
        [Test]
        public void PassesForSameFiles()
        {
            AssertThat.ZipFile("TestZip1.zip").MatchesZipFile("TestZip1.zip");
            AssertThat.ZipFile("TestZip2.zip").MatchesZipFile("TestZip2.zip");
            AssertThat.ZipFile("TestZip3.zip").MatchesZipFile("TestZip3.zip");
        }

        [Test]
        public void FailsWhenContentsDifferent()
        {
            try
            {
                AssertThat.ZipFile("TestZip1.zip").MatchesZipFile("TestZip2.zip");
                Assert.Fail("Expected exception");
            }
            catch(ZipUnitAssertException exception)
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
                AssertThat.ZipFile("TestZip1.zip").MatchesZipFile("TestZip3.zip");
                Assert.Fail("Expected exception");
            }
            catch (ZipUnitAssertException exception)
            {
                CollectionAssert.AreEqual(new[] {"C.txt"}, exception.Report.Missing);
                CollectionAssert.AreEqual(new[] { "A.txt" }, exception.Report.Additional);
                Assert.AreEqual(0, exception.Report.FileDifferences.Count);
            }
        }

        [Test]
        public void FailsWhenDifferentFilesIgnoringMissing()
        {
            try
            {
                AssertThat.ZipFile("TestZip1.zip").IgnoringMissing().MatchesZipFile("TestZip3.zip");
                Assert.Fail("Expected exception");
            }
            catch (ZipUnitAssertException exception)
            {
                CollectionAssert.AreEqual(new string [] { }, exception.Report.Missing);
                CollectionAssert.AreEqual(new[] { "A.txt" }, exception.Report.Additional);
                Assert.AreEqual(0, exception.Report.FileDifferences.Count);
            }
        }

        [Test]
        public void FailsWhenDifferentFilesIgnoringAdditional()
        {
            try
            {
                AssertThat.ZipFile("TestZip1.zip").IgnoringAdditional().MatchesZipFile("TestZip3.zip");
                Assert.Fail("Expected exception");
            }
            catch (ZipUnitAssertException exception)
            {
                CollectionAssert.AreEqual(new string[] { "C.txt" }, exception.Report.Missing);
                CollectionAssert.AreEqual(new string[] { }, exception.Report.Additional);
                Assert.AreEqual(0, exception.Report.FileDifferences.Count);
            }
        }

        [Test]
        public void PassesIgnoringAdditionalAndMissing()
        {
            AssertThat.ZipFile("TestZip1.zip").IgnoringAdditional()
                                              .IgnoringMissing()
                                              .MatchesZipFile("TestZip3.zip");
        }
    }
}
