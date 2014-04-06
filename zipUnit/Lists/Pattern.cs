using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipUnit.Lists
{
    public static class Pattern
    {
        public static bool MatchesAny(string fullName, IEnumerable<string> anyPatters)
        {
            return anyPatters.Any(p => Matches(fullName, p));
        }

        /// <summary>
        /// Checks if a full path matches a pattern (case insensitive).
        /// The directories should be split by a slash (/)
        /// Asterix (*) matches any name on one level (so it doesn't mach /)
        /// If the path begins with two slashes (//) then the pattern can start anywhere in the tree
        /// </summary>
        public static bool Matches(string fullName, string pattern)
        {
            bool startAnywhere = pattern.StartsWith("//");
            if (startAnywhere)
            {
                pattern = pattern.Substring(2);
            } 
            if(pattern.Contains("//"))
            {
                throw new ArgumentException("Pattern can only start with '//'.");
            }
            if(fullName.Contains("//"))
            {
                throw new ArgumentException("Path cannot contain '//'.");
            }
            
            string[] nameParts = fullName.ToLower().Split('/').Where(s=>!String.IsNullOrEmpty(s)).ToArray();
            string[] patternParts = pattern.ToLower().Split('/').Where(s => !String.IsNullOrEmpty(s)).ToArray();
            if (nameParts.Length < patternParts.Length || (!startAnywhere && nameParts.Length != patternParts.Length)) return false;
            for(int i=0; i<patternParts.Length; ++i)
            {
                if (!OneLevelMatches(nameParts[nameParts.Length - i - 1], patternParts[patternParts.Length - i - 1])) return false;
            }
            return true;
        }

        private static bool OneLevelMatches(string name, string pattern)
        {
            int i = 0;
            int j = 0;
            bool wasStar = false;
            while (i < name.Length && j < pattern.Length)
            {
                if (wasStar)
                {
                    if (name[i] == pattern[j])
                    {
                        wasStar = false;
                        i++;
                        j++;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    if (pattern[j] == '*')
                    {
                        wasStar = true;
                        j++;
                    }
                    else if (pattern[j] == '?' || name[i] == pattern[j])
                    {
                        j++;
                        i++;
                    }
                    else return false;
                }
            }
            if (j < pattern.Length) return false;
            if (!wasStar && i < name.Length) return false;
            return true;
        }

        public static string Extension(string fullName)
        {
            int lastDot = fullName.LastIndexOf('.');
            int lastSlash = fullName.LastIndexOf('/');
            if (lastDot <= 0 || lastDot <= lastSlash+1) return "";  //handle when filename starts with a dot
            return fullName.Substring(lastDot + 1);
        }

    }
}
