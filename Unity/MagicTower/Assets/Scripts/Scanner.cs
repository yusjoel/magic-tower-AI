using System;
using System.IO;

namespace Gempoll
{
    /// <summary>
    ///     模拟java.util.Scanner, 读取一个流, 提供nextInt()方法
    /// </summary>
    public class Scanner
    {
        private readonly string[] substrings;

        private int index;

        public Scanner(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new ArgumentException("流不能读");

            try
            {
                string allLines;
                using (var streamReader = new StreamReader(stream))
                {
                    allLines = streamReader.ReadToEnd();
                }

                substrings = allLines.Split(new[] { '\t', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public int NextInt()
        {
            if (index < 0 || index >= substrings.Length)
                throw new IndexOutOfRangeException();

            string substring = substrings[index];
            index++;

            int result;
            try
            {
                if (!int.TryParse(substring, out result))
                    throw new InvalidCastException(substring);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }
    }
}
