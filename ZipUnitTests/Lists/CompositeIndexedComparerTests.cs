using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Lists;

namespace ZipUnitTests.Lists
{
    [TestFixture]
    public class CompositeIndexedComparerTests
    {
        [Test]
        public void CallsFastForBigData()
        {
            IList<string> expected = Mock.Of<IList<string>>(l => l.Count == 10000);
            IList<string> actual = Mock.Of<IList<string>>(l => l.Count == 10000);

            IndexDifference[] diff = new IndexDifference[1];
            IIndexedComparer<string> fast = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == diff);
            IIndexedComparer<string> slow = Mock.Of<IIndexedComparer<string>>();

            CompositeIndexedComparer<string> comparer = new CompositeIndexedComparer<string>(slow, fast);
            Assert.AreEqual(diff, comparer.IndexDiff(expected, actual));
        }

        [Test]
        public void CallsFinegrainForSmallData()
        {
            IList<string> expected = Mock.Of<IList<string>>(l => l.Count == 10);
            IList<string> actual = Mock.Of<IList<string>>(l => l.Count == 10);

            IndexDifference[] diff = new IndexDifference[1];
            IIndexedComparer<string> fast = Mock.Of<IIndexedComparer<string>>();
            IIndexedComparer<string> slow = Mock.Of<IIndexedComparer<string>>(c => c.IndexDiff(expected, actual) == diff);

            CompositeIndexedComparer<string> comparer = new CompositeIndexedComparer<string>(slow, fast);
            Assert.AreEqual(diff, comparer.IndexDiff(expected, actual));
        }
    }
}
