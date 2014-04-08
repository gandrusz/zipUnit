using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public class CompositeIndexedComparer<T> : IIndexedComparer<T>
    {
        private IIndexedComparer<T> finegrainComparer;
        private IIndexedComparer<T> fastComparer;
        private int threshold;

        public CompositeIndexedComparer(IIndexedComparer<T> finegrainComparer, IIndexedComparer<T> fastComparer, int threshold = 1000000)
        {
            this.finegrainComparer = finegrainComparer;
            this.fastComparer = fastComparer;
            this.threshold = threshold;
        }

        public CompositeIndexedComparer() : this(new LongestCommonSubstringIndexedComparer<T>(), new NaiveIndexedComparer<T>())
        {
        }

        public IEnumerable<IndexDifference> IndexDiff(IList<T> expected, IList<T> actual)
        {
            if(expected.Count*actual.Count>threshold)
            {
                return fastComparer.IndexDiff(expected, actual);
            }
            return finegrainComparer.IndexDiff(expected, actual);
        }
    }
}
