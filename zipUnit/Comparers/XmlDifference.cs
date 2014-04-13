using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Comparers
{
    public enum XmlDifferenceType
    {
        DifferentNode,
        ExtraNode,
        MissingNode,
        ExtraAttribute,
        MissingAttribute,
        AttributeValue,
        NodeValue
    }

    public class XmlDifference
    {
        private readonly XmlDifferenceType differenceType;
        private readonly string expectedXPath;
        private readonly string actualXPath;
        private readonly string valueExpected;
        private readonly string valueActual;

        public XmlDifference(XmlDifferenceType differenceType, string expectedXPath, string actualXPath)
            : this(differenceType, expectedXPath, actualXPath, null, null)
        { }

        public XmlDifference(XmlDifferenceType differenceType, string expectedXPath, string actualXPath, string valueExpected, string valueActual)
        {
            this.differenceType = differenceType;
            this.valueActual = valueActual;
            this.valueExpected = valueExpected;
            this.actualXPath = actualXPath;
            this.expectedXPath = expectedXPath;
        }

        public XmlDifferenceType DifferenceType
        {
            get { return differenceType; }
        }

        public string ExpectedXPath
        {
            get { return expectedXPath; }
        }

        public string ActualXPath
        {
            get { return actualXPath; }
        }

        public string ValueExpected
        {
            get { return valueExpected; }
        }

        public string ValueActual
        {
            get { return valueActual; }
        }

        public override string ToString()
        {
            return
                String.Format(
                    "Difference type: {0}, expected: {1}, actual: {2}\r\n  Position in expected document: {3}\r\n  Position in actual document: {4}", differenceType, valueExpected ?? "", valueActual ?? "", expectedXPath ?? "", actualXPath ?? "");
        }
    }
}
