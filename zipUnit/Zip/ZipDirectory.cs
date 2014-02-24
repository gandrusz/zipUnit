using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Zip
{
    internal class ZipDirectory : IDirectory
    {
        protected readonly string name;
        protected IList<ZipDirectory> subDirectories = new List<ZipDirectory>();
        protected IList<ZipFile> files = new List<ZipFile>();
        protected ZipRootDirectory root;

        public ZipDirectory (string name, ZipRootDirectory root)
	    {
            this.name = name;
            this.root = root;
	    }

        public string Name
        {
            get { return name; }
        }

        public IEnumerable<IDirectory> SubDirectories
        {
            get { return subDirectories; }
        }

        public IEnumerable<IFile> Files
        {
            get { return files; }
        }

        internal void AddDirectory(IEnumerable<string> path)
        {
            if(path.Any())
            {
                ZipDirectory newDir = AddOrGetDirectory(path.First());
                newDir.AddDirectory(path.Skip(1));
            }
        }


        internal void AddFile(IEnumerable<string> path, string fullName)
        {
            string str = path.First();
            var rest = path.Skip(1);
            if(rest.Any())
            {
                ZipDirectory newDir = AddOrGetDirectory(str);
                newDir.AddFile(rest, fullName);
            }
            else
            {
                ZipFile file = new ZipFile(str, fullName, root);
                files.Add(file);
            }
        }

        protected void SortEntries()
        {
            subDirectories = subDirectories.OrderBy(d => d.Name).ToList();
            files = files.OrderBy(d => d.Name).ToList();
            foreach (var dir in subDirectories)
            {
                dir.SortEntries();
            }
        }

        private ZipDirectory AddOrGetDirectory(string name)
        {
            ZipDirectory result = subDirectories.FirstOrDefault(d => d.Name == name);
            if (result == null)
            {
                result = new ZipDirectory(name, root);
                subDirectories.Add(result);
            }
            return result;
        }


    }
}
