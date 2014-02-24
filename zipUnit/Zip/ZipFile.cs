using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipUnit.Zip
{
    internal class ZipFile : IFile
    {
        private readonly string name;
        private readonly string fullName;
        private readonly ZipRootDirectory root;

        public ZipFile(string name, string fullName, ZipRootDirectory root)
        {
            this.name = name;
            this.fullName = fullName;
            this.root = root;
        }

        public string Name
        {
            get { return name; }
        }

        public Stream Read()
        {
            return root.Read(fullName);
        }
    }
}
