using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipUnit.Zip
{
    internal class ZipRootDirectory : ZipDirectory
    {
        private Ionic.Zip.ZipFile zipFile;

        public ZipRootDirectory(string name) : base(name, null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Provided an empty name for a ZipFile");
            zipFile = new Ionic.Zip.ZipFile(name);
            root = this;
            foreach(var part in zipFile)
            {
                var nameParts = part.FileName.Split('/').Where(s => !string.IsNullOrWhiteSpace(s));
                if(part.IsDirectory)
                {
                    AddDirectory(nameParts);
                }
                else
                {
                    AddFile(nameParts, part.FileName);
                }                
            }
            SortEntries();
        }

        internal Stream Read(string fullName)
        {
            return zipFile[fullName].OpenReader();
        }
    }
}
