using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public class NaiveIndexedComparer<T> : IIndexedComparer<T>
    {
        private const int LOOK_AHEAD = 10;

        public IEnumerable<IndexDifference> IndexDiff(IList<T> expected, IList<T> actual)
        {
            IList<IndexDifference> results = new List<IndexDifference>();

            int i = 0;
            int j = 0;
            while (i < expected.Count && j < actual.Count)
            {
                if (expected[i].Equals(actual[j]))
                {
                    i++;
                    j++;
                }
                else 
                {
                    int aheadExpected = MatchAhead(actual[j], expected, i);
                    int aheadActual = MatchAhead(expected[i], actual, j);
                    if(aheadExpected>aheadActual)
                    {
                        results.Add(IndexDifference.Additional(j));
                        j++;
                    }
                    else if (aheadExpected < aheadActual)
                    {
                        results.Add(IndexDifference.Missing(i));
                        i++;
                    }
                    else
                    {
                        results.Add(IndexDifference.Different(i, j));
                        i++;
                        j++;
                    }
                }
            }
            while (i < expected.Count)
            {
                results.Add(IndexDifference.Missing(i));
                i++;
            }
            while (j < actual.Count)
            {
                results.Add(IndexDifference.Additional(j));
                j++;
            }

            return results;
        }

        private int MatchAhead(T item, IList<T> table, int start)
        {
            for(int k=1; k<=LOOK_AHEAD && k+start<table.Count; ++k)
            {
                if (table[start + k].Equals(item)) return k;
            }
            return int.MaxValue;
        }
    }
}
