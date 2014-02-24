using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public class ListDiff<T>
    {
        public IList<T> Missing { get; set; }
        public IList<T> Additional { get; set; }
        public IList<T> Matching { get; set; }
    }
}
