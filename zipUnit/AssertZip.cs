using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Comparers;
using ZipUnit.Lists;
using ZipUnit.Report;
using ZipUnit.Zip;

namespace ZipUnit
{
    public class AssertZip
    {
        private readonly Root actual;
        private readonly List<string> ignoreMissing = new List<string>();
        private readonly List<string> ignoreAdditional = new List<string>();
        private readonly IDictionary<string, IComparer> comparers;

        public static void AreEqual(string expectedPath, string actualPath)
        {
            new AssertZip(new ZipRootDirectory(actualPath)).MatchesZipFile(expectedPath);
        }

        internal AssertZip(IDirectory actual) : this(actual, DefaultComparers.ForExtensions) { }

        internal AssertZip(IDirectory actual, IDictionary<string, IComparer> comparers)
        {
            this.actual = new Root(actual);
            this.comparers = new Dictionary<string, IComparer>(comparers);
        }

        public AssertZip IgnoringAdditional(string pattern = "*.*")
        {
            ignoreAdditional.Add(pattern);
            return this;
        }

        public AssertZip IgnoringMissing(string pattern = "*.*")
        {
            ignoreMissing.Add(pattern);
            return this;
        }

        public AssertZip WithComparer(string extension, IComparer comparer)
        {
            comparers[extension] = comparer;
            return this;
        }

        public void MatchesZipFile(string name)
        {
            Root expected = new Root(new ZipRootDirectory(name));
            var report = Match(expected);
            if (report.Failed) throw new ZipUnitAssertException(report);
        }

        public AssertFileInZip ContainsFile(string fileName)
        {
            if (!actual.ContainsFile(fileName)) throw new ZipUnitAssertException("Assertion failed: zip " + actual + " doesn't contain " + fileName);
            IComparer comparer = GetComparer(fileName);
            return new AssertFileInZip(this, actual.OpenFile(fileName), comparer);
        }

        private MatchReport Match(Root expected)
        {
            var sortedComparer = new SortedListComparer();
            var diff = sortedComparer.Diff(expected.FilesSorted, actual.FilesSorted);
            var missing = diff.Missing.Where(file => !Pattern.MatchesAny(file, ignoreMissing)).ToList();
            var additional = diff.Additional.Where(file => !Pattern.MatchesAny(file, ignoreAdditional)).ToList();

            var fileDifferences = diff.Matching.Select(name => CompareFiles(name, expected)).Where(fd => fd != null).ToList();
            return new MatchReport(missing, additional, fileDifferences);
        }

        private IFileDifference CompareFiles(string fullName, Root expected)
        {
            IComparer comparer = GetComparer(fullName);
            return comparer.DifferenceOrNull(fullName, expected.OpenFile(fullName), actual.OpenFile(fullName));
        }

        private IComparer GetComparer(string fullName)
        {
            string extension = Pattern.Extension(fullName);
            if (comparers.ContainsKey(extension)) return comparers[extension];
            return DefaultComparers.BinaryComparer;
        }

        public class AssertFileInZip : AssertFile
        {
            private readonly AssertZip zip;

            public AssertFileInZip(AssertZip zip, Stream actual, IComparer comparer) : base(actual, comparer)
	        {
                this.zip = zip;
	        }

            public AssertZip AndTheZip()
            { 
                return zip;
            }
        }
    }
}
