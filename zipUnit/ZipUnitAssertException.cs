using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Report;

namespace ZipUnit
{
    public class ZipUnitAssertException : Exception
    {
        private readonly MatchReport report;

        public ZipUnitAssertException(MatchReport report)
            : this(report.ToString())
        {
            this.report = report;
        }

        public ZipUnitAssertException(string message) : base(message)
        {
            
        }

        public MatchReport Report
        {
            get { return report; }
        }
    
    }
}
