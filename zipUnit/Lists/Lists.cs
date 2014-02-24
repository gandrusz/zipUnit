using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public class Lists
    {
        public static ListDiff<string> Diff(IEnumerable<string> expected, IEnumerable<string> actual)
        {
            List<string> additional = new List<string>();
            List<string> missing = new List<string>();
            List<string> matching = new List<string>();

            var expectedIter = expected.GetEnumerator();
            var actualIter = actual.GetEnumerator();
            bool expectedMoved = expectedIter.MoveNext();
            bool actualMoved = actualIter.MoveNext();
            while (expectedMoved && actualMoved)
            {
                int i = string.Compare(expectedIter.Current, actualIter.Current);
                if (i < 0)
                {
                    missing.Add(expectedIter.Current);
                    expectedMoved = expectedIter.MoveNext();
                }
                else if (i > 0)
                {
                    additional.Add(actualIter.Current);
                    actualMoved = actualIter.MoveNext();
                }
                else
                {
                    matching.Add(actualIter.Current);
                    expectedMoved = expectedIter.MoveNext();
                    actualMoved = actualIter.MoveNext();
                }
            }
            if (expectedMoved)
            {
                do
                {
                    missing.Add(expectedIter.Current);
                }
                while (expectedIter.MoveNext());
            }
            if (actualMoved)
            {
                do
                {
                    additional.Add(actualIter.Current);
                }
                while (actualIter.MoveNext());
            }
            return new ListDiff<string> { Additional = additional, Matching = matching, Missing = missing};
        }
    }
}
