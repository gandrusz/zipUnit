using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Report;

namespace ZipUnit.Comparers
{
    public class XmlFileDifference : IFileDifference
    {
        private readonly string name;
        private readonly string message;
        private readonly IList<XmlDifference> differences;

        public XmlFileDifference(string name, IList<XmlDifference> differences)
        {
            this.name = name;
            this.differences = differences;
            StringBuilder sb = new StringBuilder();
            foreach (XmlDifference difference in differences)
            {
                sb.AppendLine(difference.ToString());
            }
            message = sb.ToString();
        }

        public string FullName
        {
            get { return name; }
        }

        public string Message
        {
            get { return message; }
        }

        public IList<XmlDifference> Differences
        {
            get { return differences; }
        }

        public override string ToString()
        {
            return GetType().Name + ": " + message;
        }
    }
}
