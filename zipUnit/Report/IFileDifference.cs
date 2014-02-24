using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Report
{
    public interface IFileDifference
    {
        string FullName { get; }
        string Message { get; }
    }
}
