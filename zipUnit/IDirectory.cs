using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit
{
    internal interface IDirectory
    {
        string Name { get; }
        IEnumerable<IDirectory> SubDirectories { get; }
        IEnumerable<IFile> Files { get; }
    }
}
