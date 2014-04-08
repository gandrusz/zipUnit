using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public class LongestCommonSubstringIndexedComparer<T> : IIndexedComparer<T>
    {
        public IEnumerable<IndexDifference> IndexDiff(IList<T> expected, IList<T> actual)
        {
            int[,] lcs = CalculateLcsTable(expected, actual);

            IList<IndexDifference> results = new List<IndexDifference>();

            int i = 0;
            int j = 0;
            while(i<expected.Count && j<actual.Count)
            {
                if(expected[i].Equals(actual[j]))
                {
                    i++;
                    j++;
                }
                else if (lcs[i, j+1]>lcs[i+1, j])
                {
                    results.Add(IndexDifference.Additional(j));
                    j++;
                }
                else if (lcs[i, j + 1] < lcs[i + 1, j])
                {
                    results.Add(IndexDifference.Missing(i));
                    i++;
                }
                else
                {
                    if (lcs[i, j + 1] == lcs[i + 1, j])
                    {
                        results.Add(IndexDifference.Different(i, j));
                        i++;
                        j++;
                    }
                    else
                    {
                        results.Add(IndexDifference.Missing(i));
                        i++;
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

        /// <summary>
        /// Working with suffixes as opposed to prefixed
        /// </summary>
        private int[,] CalculateLcsTable(IList<T> expected, IList<T> actual)
        {
            int[,] lcs = new int[expected.Count + 1, actual.Count + 1];
            for (int i = 0; i <= expected.Count; ++i) lcs[i, actual.Count] = 0;
            for (int j = 0; j < actual.Count; ++j) lcs[expected.Count, j] = 0;

            for(int i=expected.Count-1; i>=0; --i)
            {
                for(int j=actual.Count-1; j>=0; --j)
                {
                    if (expected[i].Equals(actual[j])) lcs[i, j] = lcs[i + 1, j + 1] + 1;
                    else lcs[i, j] = Math.Max(lcs[i, j + 1], lcs[i + 1, j]);
                }
            }
            return lcs;
        }
    }
}
