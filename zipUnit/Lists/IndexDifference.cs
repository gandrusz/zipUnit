using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public enum IndexDifferenceType
    {
        Missing,
        Additional,
        Different
    }

    public class IndexDifference
    {
        private int expectedIndex;
        private int actualIndex;

        public static IndexDifference Missing(int i)
        {
            if(i<0) throw new ArgumentException("Index needs to be non-negative");
            return new IndexDifference(i, -1);
        }

        public static IndexDifference Additional(int j)
        {
            if(j<0) throw new ArgumentException("Index needs to be non-negative");
            return new IndexDifference(-1, j);
        }

        public static IndexDifference Different(int i, int j)
        {
            if(i<0 || j<0) throw new ArgumentException("Index needs to be non-negative");
            return new IndexDifference(i, j);
        }

        private IndexDifference(int expectedIndex, int actualIndex)
        {
            this.expectedIndex = expectedIndex;
            this.actualIndex = actualIndex;
        }

        public int ExpectedIndex
        {
            get { return expectedIndex; }
        }

        public int ActualIndex
        {
            get { return actualIndex; }
        }

        public IndexDifferenceType DifferenceType
        {
            get
            {
                if (actualIndex < 0) return IndexDifferenceType.Missing;
                if (expectedIndex < 0) return IndexDifferenceType.Additional;
                return IndexDifferenceType.Different;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            IndexDifference other = obj as IndexDifference;
            if (other == null) return false;
            return other.actualIndex == actualIndex && other.expectedIndex == expectedIndex;
        }

        public override int GetHashCode()
        {
            return actualIndex.GetHashCode() ^ expectedIndex.GetHashCode();
        }
    }
}
