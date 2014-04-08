using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZipUnit.Comparers;
using ZipUnit.Lists;

namespace ZipUnitTests.Comparers
{
    [TestFixture]
    public class TextComparerTests
    {
        List<string> expected = new List<string> { "A", "B", "C"};
        List<string> actual = new List<string> { "A", "B", "C" };

        [Test]
        public void ReturnsNullIfNoErrorsFound()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { });
            TextComparer comparer = new TextComparer(indexedComparer);
            Assert.IsNull(comparer.TextFileDifferenceOrNull("name", expected, actual));
        }

        [Test]
        public void ReportsMissing()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Missing(2) });
            TextComparer comparer = new TextComparer(indexedComparer);
            var result = comparer.TextFileDifferenceOrNull("name", expected, actual);
            Assert.NotNull(result);
            StringAssert.Contains("missing", result.Message);
            StringAssert.Contains("2", result.Message);
            StringAssert.Contains("C", result.Message);            
        }

        [Test]
        public void ReportsAdditional()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Additional(2) });
            TextComparer comparer = new TextComparer(indexedComparer);
            var result = comparer.TextFileDifferenceOrNull("name", expected, actual);
            Assert.NotNull(result);
            StringAssert.Contains("not expected", result.Message);
            StringAssert.Contains("2", result.Message);
            StringAssert.Contains("C", result.Message);
        }

        [Test]
        public void ReportsDifference()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Different(1, 2) });
            TextComparer comparer = new TextComparer(indexedComparer);
            var result = comparer.TextFileDifferenceOrNull("name", expected, actual);
            Assert.NotNull(result);
            StringAssert.Contains("different", result.Message);
            StringAssert.Contains("1", result.Message);
            StringAssert.Contains("2", result.Message);
            StringAssert.Contains("B", result.Message);
            StringAssert.Contains("C", result.Message);
            AssertHasNumberOfLines(3, result.Message);
        }

        [Test]
        public void IgnoresIgnoredMissingIndex()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Missing(2) });
            TextComparer comparer = new TextComparer(indexedComparer, new [] {2});
            Assert.IsNull(comparer.TextFileDifferenceOrNull("name", expected, actual));
        }

        [Test]
        public void IgnoresIgnoredDifferentIndex()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Different(2, 2) });
            TextComparer comparer = new TextComparer(indexedComparer, new[] { 2 });
            Assert.IsNull(comparer.TextFileDifferenceOrNull("name", expected, actual));
        }

        [Test]
        public void DoesntIgnoreRelevantDifferentIndex()
        {
            var indexedComparer = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == new IndexDifference[] { IndexDifference.Different(2, 2) });
            TextComparer comparer = new TextComparer(indexedComparer, new[] { 1 });
            var result = comparer.TextFileDifferenceOrNull("name", expected, actual);
            StringAssert.Contains("different", result.Message);
            StringAssert.Contains("2", result.Message);
            StringAssert.Contains("C", result.Message);
        }


        public void AssertHasNumberOfLines(int n, string actual)
        {
            var lines = Regex.Split(actual, "\r\n|\r|\n").Count(s=> !String.IsNullOrWhiteSpace(s));
            Assert.AreEqual(n, lines, "Expected " + n + " lines, but was " + lines + ". Actual string: " + actual);
        }
    }
}
