using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Report;

namespace ZipUnit.Comparers
{
    public class BinaryFileDifference : IFileDifference
    {
        private readonly string name;
        private readonly string message;

        public BinaryFileDifference(string name, string message)
        {
            this.name = name;
            this.message = message;
        }

        public string FullName
        {
            get { return name; }
        }

        public string Message
        {
            get { return message; }
        }
    }
}
