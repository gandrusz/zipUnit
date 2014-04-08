﻿using System;
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
        private static readonly IDictionary<string, IComparer> defaultComparers = DefaultComparers(); //Static because of the specific way unit tests are usually written
        private readonly IDictionary<string, IComparer> comparers;


        public static void RegisterComparer(string extension, IComparer comparer)
        {
            defaultComparers[extension] = comparer;
        }

        private AssertThat(IDirectory actual, IDictionary<string, IComparer> comparers)
        {
            this.actual = new Root(actual);
            this.comparers = new Dictionary<string, IComparer>(comparers);
        }

        public static AssertThat ZipFile(string name)
        {
            return new AssertThat(new ZipRootDirectory(name), defaultComparers);
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

        public AssertThat WithComparer(string extension, IComparer comparer)
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
            if (defaultComparers.ContainsKey(extension)) return defaultComparers[extension];
            return binaryComparer;
        }

        
        private static IDictionary<string, IComparer> DefaultComparers()
        {
            var textComparer = new TextComparer(new CompositeIndexedComparer<string>());

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
