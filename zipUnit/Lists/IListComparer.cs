using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public interface IListComparer
    {
        ListDiff<string> Diff(IList<string> expected, IList<string> actual);       
    }


}
