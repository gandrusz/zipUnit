using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Comparers;
using ZipUnit.Lists;

namespace ZipUnitTests.Comparers
{
    [TestFixture]
    public class XmlComparerTests
    {
        public const string XML = "<a p='p'><b/><c>Text</c><d/><d/></a>";
        public const string XML_extraAttribute = "<a p='p'><b/><c q='q'>Text</c><d/><d/></a>";
        public const string XML_extraAttributeTopLevel = "<a p='p' q='q'><b/><c>Text</c><d/><d/></a>";
        public const string XML_differentAttribute = "<a q='p'><b/><c>Text</c><d/><d/></a>";
        public const string XML_differentAttributeValue = "<a p='r'><b/><c>Text</c><d/><d/></a>";
        public const string XML_schema = "<a p='p' xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><b/><c>Text</c><d/><d/></a>";
        public const string XML_missingNode = "<a p='p'><b/><d/><d/></a>";
        public const string XML_differentNode = "<a p='p'><e/><c>Text</c><d/><d/></a>";
        public const string XML_differentText = "<a p='p'><b/><c>Text1</c><d/><d/></a>";
        public const string XML_additionalText = "<a p='p'><b>more text</b><c>Text</c><d/><d/></a>";
        public const string XML_whitespace = "<a p='p'><b/> <c>Text</c><d/>\n<d/></a>";

        [Test]
        public void ReturnsNullIfEqual()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            Assert.IsNull(comparer.DifferenceOrNull("name", XML, XML));
            Assert.IsNull(comparer.DifferenceOrNull("name", XML_extraAttribute, XML_extraAttribute));
            Assert.IsNull(comparer.DifferenceOrNull("name", XML_whitespace, XML_whitespace));
        }

        [Test]
        public void ReturnsNullIfNamespaceDifferent()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            Assert.IsNull(comparer.DifferenceOrNull("name", XML, XML_schema));
            Assert.IsNull(comparer.DifferenceOrNull("name", XML_schema, XML));
        }

        [Test]
        public void ReturnsNullIfWhitespaceDifferent()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            Assert.IsNull(comparer.DifferenceOrNull("name", XML, XML_whitespace));
            Assert.IsNull(comparer.DifferenceOrNull("name", XML_whitespace, XML));
        }



        [Test]
        public void CompareFindsExtraAttribute()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_extraAttribute);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.ExtraAttribute, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/c[1]/@q", difference.Differences[0].ActualXPath);
        }

        [Test]
        public void CompareFindsMissingAttribute()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML_extraAttribute, XML);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.MissingAttribute, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/c[1]/@q", difference.Differences[0].ExpectedXPath);
        }

        [Test]
        public void CompareFindsExtraAttributeWithOtherAttributes()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_extraAttributeTopLevel);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.ExtraAttribute, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/@q", difference.Differences[0].ActualXPath);
        }

        [Test]
        public void CompareFindsMissingAttributeWithOtherAttributes()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML_extraAttributeTopLevel, XML);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.MissingAttribute, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/@q", difference.Differences[0].ExpectedXPath);
        }

        [Test]
        public void CompareFindsDifferentAttribute()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_differentAttribute);
            Assert.NotNull(difference);
            Assert.AreEqual(2, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.MissingAttribute, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/@p", difference.Differences[0].ExpectedXPath);
            Assert.AreEqual(XmlDifferenceType.ExtraAttribute, difference.Differences[1].DifferenceType);
            Assert.AreEqual("/a[1]/@q", difference.Differences[1].ActualXPath);
        }

        [Test]
        public void CompareFindsDifferentAttributeValue()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_differentAttributeValue);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.AttributeValue, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/@p", difference.Differences[0].ExpectedXPath);
            Assert.AreEqual("/a[1]/@p", difference.Differences[0].ActualXPath);
            Assert.AreEqual("p", difference.Differences[0].ValueExpected);
            Assert.AreEqual("r", difference.Differences[0].ValueActual);
        }

        [Test]
        public void CompareFindsMissingNode()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_missingNode);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.MissingNode, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/c[1]", difference.Differences[0].ExpectedXPath);
        }

        [Test]
        public void CompareFindsAdditionalNode()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML_missingNode, XML);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.ExtraNode, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/c[1]", difference.Differences[0].ActualXPath);
        }

        [Test]
        public void CompareFindsDifferentNode()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_differentNode);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.DifferentNode, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/b[1]", difference.Differences[0].ExpectedXPath);
            Assert.AreEqual("/a[1]/e[1]", difference.Differences[0].ActualXPath);
            Assert.AreEqual("b", difference.Differences[0].ValueExpected);
            Assert.AreEqual("e", difference.Differences[0].ValueActual);
        }

        [Test]
        public void CompareFindsDifferentNodeValue()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_differentText);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.NodeValue, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/c[1]/text()", difference.Differences[0].ExpectedXPath);
            Assert.AreEqual("/a[1]/c[1]/text()", difference.Differences[0].ActualXPath);
            Assert.AreEqual("Text", difference.Differences[0].ValueExpected);
            Assert.AreEqual("Text1", difference.Differences[0].ValueActual);
        }

        [Test]
        public void CompareFindsAdditionalText()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML, XML_additionalText);
            Assert.NotNull(difference);
            Assert.AreEqual(1, difference.Differences.Count);
            Assert.AreEqual(XmlDifferenceType.ExtraNode, difference.Differences[0].DifferenceType);
            Assert.AreEqual("/a[1]/b[1]/text()", difference.Differences[0].ActualXPath);
        }

        [Test]
        public void ReportsMultipleDifferences()
        {
            XmlComparer comparer = new XmlComparer(new CompositeIndexedComparer<string>());
            XmlFileDifference difference = comparer.DifferenceOrNull("name", XML_differentAttributeValue, XML_additionalText);
            Assert.NotNull(difference);
            Assert.AreEqual(2, difference.Differences.Count);
        }
    }
}
