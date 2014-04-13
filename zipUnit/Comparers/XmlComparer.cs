using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ZipUnit.Lists;
using ZipUnit.Report;

namespace ZipUnit.Comparers
{
    public class XmlComparer : IComparer
    {
        private const string XMLNS = "xmlns";
        IIndexedComparer<string> indexedComparer;

        public XmlComparer(IIndexedComparer<string> indexedComparer)
        {
            this.indexedComparer = indexedComparer;
        }


        public IFileDifference DifferenceOrNull(string name, Stream expected, Stream actual)
        {
            XmlDocument expectedDocument = new XmlDocument();
            expectedDocument.Load(expected);

            XmlDocument actualDocument = new XmlDocument();
            actualDocument.Load(actual);

            return DifferenceOrNull(name, expectedDocument, actualDocument);
        }

        public XmlFileDifference DifferenceOrNull(string name, string expectedXml, string actualXml)
        {
            XmlDocument expectedDocument = new XmlDocument();
            expectedDocument.LoadXml(expectedXml);

            XmlDocument actualDocument = new XmlDocument();
            actualDocument.LoadXml(actualXml);

            return DifferenceOrNull(name, expectedDocument, actualDocument);
        }

        public XmlFileDifference DifferenceOrNull(string name, XmlNode expected, XmlNode actual)
        {
            IList<XmlDifference> differences = CompareElements(expected, actual);
            if (differences.Count == 0) return null;
            return new XmlFileDifference(name, differences);
        }

        public IList<XmlDifference> CompareElements(XmlNode expected, XmlNode actual)
        {
            List<XmlDifference> result = new List<XmlDifference>();

            if (expected == null && actual == null)
            {
                return result;
            }
            if (expected == null)
            {                
                result.AddRange(HandleDifference(XmlDifferenceType.ExtraNode, null, GetXPath(actual)));
                return result;
            }
            if (actual == null)
            {
                result.AddRange(HandleDifference(XmlDifferenceType.MissingNode, GetXPath(expected), null));
                return result;
            }
            if (expected.Name != actual.Name)
            {
                result.AddRange(HandleDifference(XmlDifferenceType.DifferentNode, GetXPath(expected), GetXPath(actual), expected.LocalName, actual.LocalName));
            }
            if (expected.NodeType != actual.NodeType)
            {
                //TODO more details needed?
                result.AddRange(HandleDifference(XmlDifferenceType.DifferentNode, GetXPath(expected), GetXPath(actual), expected.NodeType.ToString(), actual.NodeType.ToString()));
            }
            if (expected.Value != actual.Value)
            {
                result.AddRange(HandleDifference(XmlDifferenceType.NodeValue, GetXPath(expected), GetXPath(actual), expected.Value, actual.Value));
            }

            result.AddRange(CompareAttributes(expected, actual));

            //compare children
            IList<string> expectedNames = GetNodeNames(expected.ChildNodes);
            IList<string> actualNames = GetNodeNames(actual.ChildNodes);
            var diffEnumerator = indexedComparer.IndexDiff(expectedNames, actualNames).GetEnumerator();
            IndexDifference nextDifference = diffEnumerator.MoveNext() ? diffEnumerator.Current : null;
            int i = 0;
            int j = 0;
            while (i < expectedNames.Count || j < actualNames.Count)
            {
                if(nextDifference!= null && (i==nextDifference.ExpectedIndex || j==nextDifference.ActualIndex))
                {
                    var expectedOrNull = expected.ChildNodes[nextDifference.ExpectedIndex];
                    var actualOrNull = actual.ChildNodes[nextDifference.ActualIndex];
                    result.AddRange(CompareElements(expectedOrNull, actualOrNull));
                    if (i == nextDifference.ExpectedIndex) i++;
                    if (j == nextDifference.ActualIndex) j++;
                    nextDifference = diffEnumerator.MoveNext() ? diffEnumerator.Current : null;
                }
                else
                {
                    result.AddRange(CompareElements(expected.ChildNodes[i], actual.ChildNodes[j]));
                    i++;
                    j++;
                }
            }
            return result;
        }

        private IEnumerable<XmlDifference> CompareAttributes(XmlNode expected, XmlNode actual)
        {
            List<XmlDifference> result = new List<XmlDifference>();
            if (expected.Attributes == actual.Attributes)
            {
                return result;
            }
            if (expected.Attributes == null)
            {
                foreach (XmlAttribute actualAttribute in actual.Attributes)
                {
                    result.AddRange(HandleDifference(XmlDifferenceType.ExtraAttribute, null, GetXPath(actualAttribute)));
                }
                return result;
            }
            if (actual.Attributes == null)
            {
                foreach (XmlAttribute expectedAttribute in expected.Attributes)
                {
                    result.AddRange(HandleDifference(XmlDifferenceType.MissingAttribute, GetXPath(expectedAttribute), null));
                }
                return result;
            }

            foreach (XmlAttribute expectedAttribute in expected.Attributes)
            {
                if (!IsSpecialAttribute(expectedAttribute))
                {
                    XmlAttribute actualAttribute = actual.Attributes[expectedAttribute.Name];
                    if (actualAttribute == null)
                    {
                        result.AddRange(HandleDifference(XmlDifferenceType.MissingAttribute, GetXPath(expectedAttribute), null));
                        continue;
                    }
                    if (actualAttribute.Value != expectedAttribute.Value)
                    {
                        result.AddRange(HandleDifference(XmlDifferenceType.AttributeValue, GetXPath(expectedAttribute),
                                         GetXPath(actualAttribute), expectedAttribute.Value, actualAttribute.Value));
                    }
                }
            }

            foreach (XmlAttribute actualAttribute in actual.Attributes)
            {
                XmlAttribute expectedAttribute = expected.Attributes[actualAttribute.Name];
                if (expectedAttribute == null && !IsSpecialAttribute(actualAttribute))
                {
                    result.AddRange(HandleDifference(XmlDifferenceType.ExtraAttribute, null, GetXPath(actualAttribute)));
                }
            }
            return result;
        }

        private IEnumerable<XmlDifference> HandleDifference(XmlDifferenceType differenceType, string expectedXPath, string actualXPath)
        {
            return HandleDifference(differenceType, expectedXPath, actualXPath, null, null);
        }

        private IEnumerable<XmlDifference> HandleDifference(XmlDifferenceType differenceType, string expectedXPath, string actualXPath, string valueExpected, string valueActual)
        {
            XmlDifference difference = new XmlDifference(differenceType, expectedXPath, actualXPath, valueExpected, valueActual);
            if (!IgnoreDifference(difference))
            {
                yield return difference;
            }
        }

        /// <summary>
        /// Override in a derived class to have better control over which differences are reported
        /// </summary>
        public virtual bool IgnoreDifference(XmlDifference difference)
        {
            return false;
        }

        
        private static bool IsSpecialAttribute(XmlAttribute attribute)
        {
            if (attribute.Name == XMLNS || attribute.Prefix == XMLNS)
            {
                return true;
            }
            return false;
        }

        private static string GetXPath(XmlNode node)
        {
            StringBuilder xPathSB = new StringBuilder();
            if (node == null)
            {
                return null;
            }
            XmlAttribute attribute = node as XmlAttribute;
            if (attribute != null)
            {
                GetXPathForNodeRecursive(attribute.OwnerElement, xPathSB);
                xPathSB.AppendFormat("/@{0}", attribute.Name);
            }
            else if (node.NodeType == XmlNodeType.Text)
            {
                GetXPathForNodeRecursive(node.ParentNode, xPathSB);
                xPathSB.Append("/text()");
            }
            else
            {
                GetXPathForNodeRecursive(node, xPathSB);
            }
            return xPathSB.ToString();
        }

        private static void GetXPathForNodeRecursive(XmlNode node, StringBuilder xPathSB)
        {
            int number = 1;
            XmlNode sibling = node.PreviousSibling;
            while (sibling != null)
            {
                if (sibling.Name == node.Name)
                {
                    number++;
                }
                sibling = sibling.PreviousSibling;
            }
            if (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
            {
                GetXPathForNodeRecursive(node.ParentNode, xPathSB);
            }
            xPathSB.AppendFormat("/{0}[{1}]", node.LocalName, number);
        }

        private IList<string> GetNodeNames(XmlNodeList xmlNodes)
        {
            List<string> result = new List<string>();
            foreach(XmlNode node in xmlNodes)
            {
                result.Add(node.Name);
            }
            return result;
        }
    }
}
