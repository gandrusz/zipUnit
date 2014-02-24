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
            var diff = Lists.Lists.Diff(expected.FilesSorted, actual.FilesSorted);
            var missing = diff.Missing.Where(file => !Pattern.MatchesAny(file, ignoreMissing)).ToList();
            var additional = diff.Additional.Where(file => !Pattern.MatchesAny(file, ignoreAdditional)).ToList();

            var fileDifferences = diff.Matching.Select(name => CompareFiles(name, expected)).Where(fd => fd != null).ToList();
            return new MatchReport(missing, additional, fileDifferences);
        }

        private IFileDifference CompareFiles(string fullName, Root expected)
        {
            return binaryComparer.DifferenceOrNull(fullName, expected.OpenFile(fullName), actual.OpenFile(fullName));
        }

        
    }
}
