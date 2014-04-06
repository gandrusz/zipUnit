using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public interface IIndexedComparer<T>
    {
        IEnumerable<IndexDifference> IndexDiff(IList<T> expected, IList<T> actual);
    }
}
