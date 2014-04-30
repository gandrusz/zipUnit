using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipUnit
{
    internal class Root
    {
        private IDirectory rootDirectory;
        private List<string> files;

        public Root(IDirectory rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            files = FilesForDirectory(rootDirectory, "").ToList();
            files.Sort(StringComparer.InvariantCultureIgnoreCase);
        }

        private IEnumerable<string> FilesForDirectory(IDirectory rootDirectory, string pathSoFar)
        {
            Func<string, string, string> newName = (string path, string name) => path == "" ? name : path + "/" + name;
            return rootDirectory.SubDirectories.SelectMany(d => FilesForDirectory(d, newName(pathSoFar, d.Name))).Union(rootDirectory.Files.Select(f => newName(pathSoFar, f.Name)));
        }

        public IList<string> FilesSorted 
        {
            get { return files; } 
        }

        public Stream OpenFile(string fullName)
        {
            return OpenFile(rootDirectory, fullName.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries), 0);
        }

        //TODO: improve efficiency
        private Stream OpenFile(IDirectory directory, string[] path, int next)
        {
            if (next + 1 == path.Length) return directory.Files.Single(f => f.Name == path[next]).Read();
            return OpenFile(directory.SubDirectories.Single(d => d.Name == path[next]), path, next + 1);
        }
        //TODO: add directories

        internal bool ContainsFile(string fullName)
        {
            return ContainsFile(rootDirectory, fullName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries), 0);
        }

        private bool ContainsFile(IDirectory directory, string[] path, int next)
        {
            if (next + 1 == path.Length) return directory.Files.Any(f => f.Name == path[next]);
            return ContainsFile(directory.SubDirectories.Single(d => d.Name == path[next]), path, next + 1);
        }
    }
}
