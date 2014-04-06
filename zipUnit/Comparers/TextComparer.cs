using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Lists;
using ZipUnit.Report;

namespace ZipUnit.Comparers
{
    public class TextComparer : IComparer
    {
        IIndexedComparer<string> indexedComparer;
        HashSet<int> ignoreLines = new HashSet<int>();

        public TextComparer(IIndexedComparer<string> indexedComparer)
        {
            this.indexedComparer = indexedComparer;
        }

        public TextComparer(IIndexedComparer<string> indexedComparer, IEnumerable<int> ignoreLines)
        {
            this.indexedComparer = indexedComparer;
            this.ignoreLines.UnionWith(ignoreLines);
        }

        public IFileDifference DifferenceOrNull(string name, Stream expected, Stream actual)
        {
            return TextFileDifferenceOrNull(name, ReadLines(expected), ReadLines(actual));
        }

        public TextFileDifference TextFileDifferenceOrNull(string name, IList<string> expected, IList<string> actual)
        {
            var differences = indexedComparer.IndexDiff(expected, actual);
            bool wasDifference = false;
            StringBuilder sb = new StringBuilder();
            foreach(var difference in differences)
            {
                if (ignoreLines.Contains(difference.ExpectedIndex)) continue;
                wasDifference = true;
                switch(difference.DifferenceType)
                {
                    case IndexDifferenceType.Missing:
                        sb.AppendFormat("Line " + difference.ExpectedIndex + " was missing: " + expected[difference.ExpectedIndex]);
                        break;
                    case IndexDifferenceType.Additional:
                        sb.AppendFormat("Line " + difference.ActualIndex + " was not expected: " + actual[difference.ActualIndex]);
                        break;
                    case IndexDifferenceType.Different:
                        sb.AppendFormat("Line " + difference.ExpectedIndex + " was different from the line " + difference.ActualIndex + " in actual:");
                        sb.AppendFormat("  Expected: " + expected[difference.ExpectedIndex]);
                        sb.AppendFormat("  Actual:   " + actual[difference.ActualIndex]);
                        break;
                    default:
                        throw new NotSupportedException("IndexDifferenceType " + difference.DifferenceType + " is not expected.");
                }
            }
            if(wasDifference) return new TextFileDifference(name, sb.ToString());
            return null;
        }

        private IList<string> ReadLines(Stream stream)
        {
            List<string> result = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }
}
