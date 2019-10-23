using NUnit.Framework;
using System;
using System.IO;

namespace Gempoll.Editor.Tests
{
    public class GameInfoTest
    {
        /// <summary>
        ///     测试读取地图文件
        /// </summary>
        [Test]
        public void TestReadMap()
        {
            string map1 = Helper.GetStreamingAssetPath("map1.txt");
            using (var fileStream = File.OpenRead(map1))
            {
                var scanner = new Scanner(fileStream);
                var gameInfo = new GameInfo();
                gameInfo.Deserialize(scanner);

                // 起始的3个数据
                Assert.AreEqual(1, gameInfo.FloorCount);
                Assert.AreEqual(11, gameInfo.RowCount);
                Assert.AreEqual(11, gameInfo.ColumnCount);

                // 最后的3个数据
                Assert.AreEqual(0, gameInfo.HeroFloor);
                Assert.AreEqual(0, gameInfo.HeroPositionX);
                Assert.AreEqual(5, gameInfo.HeroPositionY);

                // 应该正好读完
                Assert.Throws<IndexOutOfRangeException>(() =>
                {
                    scanner.NextInt();
                });
            }
        }

        [Test]
        public void TestSerialize()
        {
            string map1 = Helper.GetStreamingAssetPath("map1.txt");
            string expected = File.ReadAllText(map1);

            using (var fileStream = File.OpenRead(map1))
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.NewLine = "\n";
                var scanner = new Scanner(fileStream);
                var gameInfo = new GameInfo();
                gameInfo.Deserialize(scanner);
                gameInfo.Serialize(streamWriter);
                streamWriter.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(memoryStream))
                {
                    string text = streamReader.ReadToEnd();
                    Assert.AreEqual(expected, text);
                }
            }
        }

        //[Test]
        public void TestSerializeFile()
        {
            string map1 = Helper.GetStreamingAssetPath("map3.txt");
            string map1Copy = Helper.GetStreamingAssetPath("map3-copy.txt");

            using (var fileStream = File.OpenRead(map1))
            using (var streamWriter = new StreamWriter(File.Open(map1Copy, FileMode.Create)))
            {
                streamWriter.NewLine = "\n";
                var scanner = new Scanner(fileStream);
                var gameInfo = new GameInfo();
                gameInfo.Deserialize(scanner);
                gameInfo.Serialize(streamWriter);
            }
        }
    }
}
