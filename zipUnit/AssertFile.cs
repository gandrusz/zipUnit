using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Comparers;

namespace ZipUnit
{
    public class AssertFile : IDisposable
    {
        private bool needsDisposing = false;
        private readonly Stream actual;
        private readonly string name = "N/A";
        private IComparer comparer;

        public static void AreEqual(string expectedPath, string actualPath)
        {
            using (var assert = new AssertFile(actualPath))
            {
                assert.Matches(expectedPath);
            }
        }

        public static void AreEqual(string expectedPath, string actualPath, IComparer comparer)
        {
            using (var assert = new AssertFile(actualPath, comparer))
            {
                assert.Matches(expectedPath);
            }
        }

        public static void AreEqual(Stream expected, Stream actual, IComparer comparer)
        {
            using (var assert = new AssertFile(actual, comparer))
            {
                assert.Matches(expected);
            }
        }

        public AssertFile(string actualFileName)
            : this(actualFileName, GetComparerForFilename(actualFileName))
        {}

        public AssertFile(string actualFileName, IComparer comparer) : this(File.Open(actualFileName, FileMode.Open), comparer)
        {
            needsDisposing = true;
        }

        public AssertFile(Stream actual, IComparer comparer)
        {
            this.actual = actual;
            this.comparer = comparer;
        }

        public AssertFile UsingComparer(IComparer comparer)
        {
            this.comparer = comparer;
            return this;
        }

        public AssertFile Matches(String expectedPath)
        {
            using (var stream = File.Open(expectedPath, FileMode.Open))
            {
                return Matches(stream);
            }
        }

        public AssertFile Matches(Stream expected)
        {
            var report = comparer.DifferenceOrNull(name, expected, actual);
            if (report != null) throw new ZipUnitAssertException(report.ToString());
            return this;
        }

        private static IComparer GetComparerForFilename(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (DefaultComparers.ForExtensions.ContainsKey(extension)) return DefaultComparers.ForExtensions[extension];
            return DefaultComparers.BinaryComparer;
        }

        public void Dispose()
        {
            if(needsDisposing) actual.Dispose();
        }
    }
}
