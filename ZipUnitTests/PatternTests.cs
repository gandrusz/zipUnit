using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipUnit.Lists;

namespace ZipUnitTests
{
    [TestFixture]
    public class PatternTests
    {
        [TestCase("Fred", "fred", Result = true)]
        [TestCase("Fred", "F?ed", Result = true)]
        [TestCase("Fred", "*d", Result = true)]
        [TestCase("Fred", "F*d", Result = true)]
        [TestCase("Fred", "F*", Result = true)]
        [TestCase("Fred", "*", Result = true)]
        [TestCase("Fred", "F*f", Result = false)]
        [TestCase("Fred", "*f", Result = false)]
        [TestCase("Fred", "r*", Result = false)]
        [TestCase("Fred/a/b/c.txt", "Fred/a/b/c.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "/Fred/a/b/c.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "//Fred/a/b/c.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "//a/b/c.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "//*.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "Fred/a/b/*.txt", Result = true)]
        [TestCase("/Fred/a/b/c.txt", "Fred/a/b/c.txt", Result = true)]
        [TestCase("Fred/a/b/c.txt", "/a/b/c.txt", Result = false)]
        [TestCase("Fred/a/b/c.txt", "a/b/c.txt", Result = false)]
        [TestCase("Fred/a/b/c.txt", "*/b/c.txt", Result = false)]
        [TestCase("Fred/a/b/c.txt", "//d.txt", Result = false)]
        [TestCase("Fred/a/b/c.txt", "Fred/a/b/c", Result = false)]
        [TestCase("Fred/a/b/c.txt", "Fred/a/b/", Result = false)]
        public bool MachesTest(string name, string pattern)
        {
            return Pattern.Matches(name, pattern);
        }
    }
}
