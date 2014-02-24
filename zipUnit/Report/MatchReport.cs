using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ZipUnit.Report
{
    public class MatchReport
    {
        private const string INDENT = "  ";
        private readonly IList<string> missing = new List<string>();
        private readonly IList<string> additional = new List<string>();
        private readonly IList<IFileDifference> fileDifferences = new List<IFileDifference>();

        public MatchReport(IList<string> missing, IList<string> additional, IList<IFileDifference> fileDifferences)
        {
            this.missing = missing;
            this.additional = additional;
            this.fileDifferences = fileDifferences;
        }


        public bool Failed
        {
            get
            {
                return Missing.Count > 0 || Additional.Count > 0 || FileDifferences.Count > 0;
            }
        }

        public IList<string> Missing
        {
            get { return missing; }
        }

        public IList<string> Additional
        {
            get { return additional; }
        }

        public IList<IFileDifference> FileDifferences
        {
            get { return fileDifferences; }
        } 


        public override string ToString()
        {
            if (!Failed) return "No differences found";
            StringBuilder sb = new StringBuilder();
            if(Missing.Count>0)
            {
                sb.AppendLine("There were " + Missing.Count + " files missing:");
                foreach (var file in Missing) sb.AppendLine(INDENT + file);
            }
            if (Additional.Count > 0)
            {
                sb.AppendLine("There were " + Additional.Count + " unexpected files:");
                foreach (var file in Additional) sb.AppendLine(INDENT + file);
            }
            if (FileDifferences.Count > 0)
            {
                sb.AppendLine("There were " + FileDifferences.Count + " differences:");
                foreach (var diff in FileDifferences)
                {
                    var lines = Regex.Split(diff.Message, "\r\n|\r|\n");
                    sb.AppendLine(INDENT + diff.GetType().Name+": " + diff.FullName);
                    foreach(var line in lines)
                    {
                        sb.AppendLine(INDENT + INDENT + line);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
