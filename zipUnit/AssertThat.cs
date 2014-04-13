using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Report;
using ZipUnit.Zip;
using ZipUnit.Lists;
using ZipUnit.Comparers;

namespace ZipUnit
{
    public class AssertThat
    {
        public static AssertZip ZipFile(string actualPath)
        {
            return new AssertZip(new ZipRootDirectory(actualPath));
        }

        public static AssertFile File(string actualPath)
        {
            return new AssertFile(actualPath);
        }

        public static AssertXml XmlFile(string actualXml)
        {
            return new AssertXml(actualXml);
        }
    }
}
