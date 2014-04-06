using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Report;
using ZipUnit.Zip;
using ZipUnit.Lists;
using ZipUnit.Comparers;

namespace ZipUnit
{
    public class AssertThat
    {
        private readonly Root actual;
        private readonly List<string> ignoreMissing = new List<string>();
        private readonly List<string> ignoreAdditional = new List<string>();

        private readonly IComparer binaryComparer = new BinaryComparer();
        private static readonly IDictionary<string, IComparer> comparerForExtension = DefaultComparers(); //Static because of the specific way unit tests are usually written

        

        private AssertThat(IDirectory actual)
        {
            this.actual = new Root(actual);
        }

        public static AssertThat ZipFile(string name)
        {
            return new AssertThat(new ZipRootDirectory(name));
        }

        public AssertThat IgnoringAdditional(string pattern = "*.*")
        {
            ignoreAdditional.Add(pattern);
            return this;
        }

        public AssertThat IgnoringMissing(string pattern = "*.*")
        {
            ignoreMissing.Add(pattern);
            return this;
        }

        public void MatchesZipFile(string name)
        {
            Root expected = new Root(new ZipRootDirectory(name));
            var report = Match(expected);
            if (report.Failed) throw new ZipUnitAssertException(report);
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
            if (comparerForExtension.ContainsKey(extension)) return comparerForExtension[extension];
            return binaryComparer;
        }

        
        private static IDictionary<string, IComparer> DefaultComparers()
        {
            var textComparer = new TextComparer(new LongestCommonSubstringListComparer<string>());

            return new Dictionary<string, IComparer>(StringComparer.InvariantCultureIgnoreCase) {
                {"txt", textComparer},
                {"htm", textComparer},
                {"html", textComparer},
                {"cs", textComparer},
                {"cpp", textComparer},
                {"java", textComparer},
                {"h", textComparer},
                {"tex", textComparer},
                {"xml", textComparer} //temporarily
            };
        } 
    }
}
