using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Comparers;

namespace ZipUnit
{
    public class AssertFile
    {
        private readonly Stream actual;
        private readonly string name = "N/A";
        private IComparer comparer;

        public static void AreEqual(string expectedPath, string actualPath)
        {
            new AssertFile(actualPath).Matches(expectedPath);
        }

        public static void AreEqual(string expectedPath, string actualPath, IComparer comparer)
        {
            new AssertFile(actualPath, comparer).Matches(expectedPath);
        }

        public static void AreEqual(Stream expected, Stream actual, IComparer comparer)
        {
            new AssertFile(actual, comparer).Matches(expected);
        }

        public AssertFile(string actualFileName)
            : this(actualFileName, GetComparerForFilename(actualFileName))
        {}

        public AssertFile(string actualFileName, IComparer comparer) : this(File.Open(actualFileName, FileMode.Open), comparer)
        {}

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

        public void Matches(String expectedPath)
        {
            Matches(File.Open(expectedPath, FileMode.Open));
        }

        public void Matches(Stream expected)
        {
            var report = comparer.DifferenceOrNull(name, expected, actual);
            if (report != null) throw new ZipUnitAssertException(report.Message);
        }

        private static IComparer GetComparerForFilename(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (DefaultComparers.ForExtensions.ContainsKey(extension)) return DefaultComparers.ForExtensions[extension];
            return DefaultComparers.BinaryComparer;
        }
    }
}
