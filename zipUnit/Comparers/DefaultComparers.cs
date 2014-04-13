using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZipUnit.Lists;

namespace ZipUnit.Comparers
{
    public static class DefaultComparers
    {
        public readonly static IComparer BinaryComparer = new BinaryComparer();
        public readonly static IComparer TextComparer = new TextComparer(new CompositeIndexedComparer<string>());
        public readonly static XmlComparer XmlComparer = new XmlComparer(new CompositeIndexedComparer<string>());

        public static readonly IDictionary<string, IComparer> ForExtensions = GetDefaultComparers(); //Static because of the specific way unit tests are usually written

        public static void RegisterComparer(string extension, IComparer comparer)
        {
            ForExtensions[extension] = comparer;
        }

        public static IDictionary<string, IComparer> GetDefaultComparers()
        {
            return new Dictionary<string, IComparer>(StringComparer.InvariantCultureIgnoreCase) {
                {"txt", TextComparer},
                {"htm", TextComparer},
                {"html", TextComparer},
                {"cs", TextComparer},
                {"cpp", TextComparer},
                {"java", TextComparer},
                {"h", TextComparer},
                {"tex", TextComparer},
                {"xml", XmlComparer},
                {"xhtml", XmlComparer},
                {"xht", XmlComparer},
                {"csproj", XmlComparer}
            };
        } 
    }
}
