using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Lists;

namespace ZipUnitTests.Lists
{
    [TestFixture]
    public class NaiveIndexedComparerTests
    {
        private NaiveIndexedComparer<string> comparer = new NaiveIndexedComparer<string>();

        [Test]
        public void DiffWhenEqual()
        {
            var expected = new[] { "A", "B", "C", "D", "E" };
            var actual = new[] { "A", "B", "C", "D", "E" };
            var diff = comparer.IndexDiff(expected, actual);

            CollectionAssert.IsEmpty(diff);
        }

        [Test]
        public void DiffWhenExpectedLonger()
        {
            var expected = new[] { "A", "B", "C", "D", "E" };
            var actual = new[] { "A1", "B", "C", "D" };
            var diff = comparer.IndexDiff(expected, actual);

            CollectionAssert.AreEqual(new[] { IndexDifference.Different(0, 0), IndexDifference.Missing(4) }, diff);
        }

        [Test]
        public void DiffWhenActualLonger()
        {
            var expected = new[] { "A", "B", "C", "D" };
            var actual = new[] { "A", "C", "D", "E" };
            var diff = comparer.IndexDiff(expected, actual);

            CollectionAssert.AreEqual(new[] { IndexDifference.Missing(1), IndexDifference.Additional(3) }, diff);
        }
    }
}
