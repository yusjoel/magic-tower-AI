using Gempoll.Extensions;
using Gempoll.H5Game;
using NUnit.Framework;

namespace Gempoll.Editor.Tests
{
    public class H5GameReaderTest
    {
        [Test]
        public void TestReadDataJs()
        {
            var reader = new Reader(@"F:\Games\Play\PC\蓝宝石实战塔\project\");
            reader.ReadDataJs();
            Assert.AreEqual("[MT0, MT1, MT2, MT3]", reader.FloorIds.Serialize());
        }
    }
}
