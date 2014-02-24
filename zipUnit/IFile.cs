using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipUnit
{
    interface IFile
    {
        string Name { get; }
        Stream Read();
    }
}
