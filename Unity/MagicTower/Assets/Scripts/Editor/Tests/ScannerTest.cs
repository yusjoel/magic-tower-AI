using NUnit.Framework;
using System.IO;

namespace Gempoll.Editor.Tests
{
    public class ScannerTest
    {
        [Test]
        public void Test1()
        {
            int expected = 100;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(100);
                streamWriter.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin);
                var scanner = new Scanner(memoryStream);
                Assert.AreEqual(expected, scanner.NextInt());
            }
        }
    }
}
