using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ZipUnitTests.Comparers
{
    public static class AssertUtil
    {
        public static  void HasNumberOfLines(int n, string actual)
        {
            var lines = Regex.Split(actual, "\r\n|\r|\n").Count(s => !String.IsNullOrWhiteSpace(s));
            Assert.AreEqual(n, lines, "Expected " + n + " lines, but was " + lines + ". Actual string: " + actual);
        }
    }
}
