using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZipUnit.Report;

namespace ZipUnit.Comparers
{
    public class BinaryComparer : IComparer
    {
        public IFileDifference DifferenceOrNull(string name, Stream expected, Stream actual)
        {
            const int bufferSize = 2048 * 2;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = expected.Read(buffer1, 0, bufferSize);
                int count2 = actual.Read(buffer2, 0, bufferSize);

                if (count1 > count2)
                {
                    return new BinaryFileDifference(name, "The expected file was longer than the actual");
                }

                if(count2 < count1 )
                {
                    return new BinaryFileDifference(name, "The expected file was shorter than the actual");
                }

                if (count1 == 0)
                {
                    return null;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return new BinaryFileDifference(name, "The files were different");
                    }
                }
            }
        }
    }
}
