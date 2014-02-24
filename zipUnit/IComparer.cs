using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Report;

namespace ZipUnit
{
    public interface IComparer
    {
        IFileDifference DifferenceOrNull(string name, Stream expected, Stream actual);
    }
}
