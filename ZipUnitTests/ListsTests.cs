using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipUnit.Lists;

namespace ZipUnitTests
{
    [TestFixture]
    public class ListsTests
    {
        [Test]
        public void DiffWhenEqual()
        {
            var expected = new[] { "A", "B", "C", "D", "E" };
            var actual = new[] { "A", "B", "C", "D", "E" };
            var diff = Lists.Diff(expected, actual);

            CollectionAssert.IsEmpty(diff.Missing);
            CollectionAssert.IsEmpty(diff.Additional);
            CollectionAssert.AreEqual(expected, diff.Matching);
        }

        [Test]
        public void DiffWhenExpectedLonger()
        {
            var expected = new[] { "A", "B", "C", "D", "E" };
            var actual = new[] { "A1", "B", "C", "D" };
            var diff = Lists.Diff(expected, actual);

            CollectionAssert.AreEqual(new[] { "A", "E" }, diff.Missing);
            CollectionAssert.AreEqual(new[] { "A1" }, diff.Additional);
            CollectionAssert.AreEqual(new[] { "B", "C", "D" }, diff.Matching);
        }

        [Test]
        public void DiffWhenActualLonger()
        {
            var expected = new[] { "A", "B", "C", "D"};
            var actual = new[] { "A", "C", "D", "E" };
            var diff = Lists.Diff(expected, actual);

            CollectionAssert.AreEqual(new[] { "B" }, diff.Missing);
            CollectionAssert.AreEqual(new[] { "E" }, diff.Additional);
            CollectionAssert.AreEqual(new[] { "A", "C", "D" }, diff.Matching);
        }
    }
}
