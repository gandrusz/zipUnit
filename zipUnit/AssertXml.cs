using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ZipUnit.Comparers;

namespace ZipUnit
{
    public class AssertXml
    {
        private readonly XmlNode actual;
        private readonly string name = "N/A";
        private XmlComparer comparer;

        public static void AreEqual(string expectedXml, string actualXml)
        {
            new AssertXml(actualXml).Matches(expectedXml);
        }

        public static void AreEqual(string expectedXml, string actualXml, XmlComparer comparer)
        {
            new AssertXml(actualXml, comparer).Matches(expectedXml);
        }

        public static void AreEqual(XmlNode expected, XmlNode actual, XmlComparer comparer)
        {
            new AssertXml(actual, comparer).Matches(expected);
        }

        public AssertXml(string actualXml)
            : this(actualXml, DefaultComparers.XmlComparer)
        {}

        public AssertXml(string actualXml, XmlComparer comparer)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(actualXml);
            actual = doc;
            this.comparer = comparer;
        }

        public AssertXml(XmlNode actual, XmlComparer comparer)
        {
            this.actual = actual;
            this.comparer = comparer;
        }

        public AssertXml UsingComparer(XmlComparer comparer)
        {
            this.comparer = comparer;
            return this;
        }

        public void Matches(String expectedXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(expectedXml);
            Matches(doc);
        }

        public void Matches(XmlNode expected)
        {
            var report = comparer.DifferenceOrNull(name, expected, actual);
            if (report != null) throw new ZipUnitAssertException(report.Message);
        }
    }
}
