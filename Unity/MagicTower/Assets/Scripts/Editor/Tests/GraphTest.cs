using NUnit.Framework;
using System;
using System.IO;

namespace Gempoll.Editor.Tests
{
    public class GraphTest
    {
        [Test]
        public void TestReadMap()
        {
            // in Visual Studio
            // <unity-project>\Temp\bin\Debug\
            // in Unity Editor
            // <unity-project>\Library\ScriptAssemblies
            string basePath = Path.GetDirectoryName(typeof(GraphTest).Assembly.Location);
            Assert.IsNotNull(basePath);

            if (basePath.EndsWith("ScriptAssemblies"))
                basePath = $"{basePath}/../..";
            else
                basePath = $"{basePath}/../../..";

            // 尽量不要使用UnityEngine下的方法, 如Application.streamingAssetsPath
            // 在VisualStudio中会报错: ECall 方法必须打包到系统模块中
            string map1 = $"{basePath}/Assets/StreamingAssets/map1.txt";
            using (var fileStream = File.OpenRead(map1))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, false, false);
                // 起始的3个数据
                Assert.AreEqual(1, graph.floorCount);
                Assert.AreEqual(11, graph.rowCount);
                Assert.AreEqual(11, graph.columnCount);

                // 最后的3个数据
                Assert.AreEqual(0, graph.heroNode.f);
                Assert.AreEqual(0, graph.heroNode.x);
                Assert.AreEqual(5, graph.heroNode.y);

                // 应该正好读完
                Assert.Throws<IndexOutOfRangeException>(() =>
                {
                    scanner.NextInt();
                });
            }
        }
    }
}
