using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Gempoll.Editor.Tests
{
    public class GraphTest
    {
        private string GetMap1Path()
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
            return map1;
        }

        /// <summary>
        ///     测试读取地图文件
        /// </summary>
        [Test]
        public void TestReadMap()
        {
            string map1 = GetMap1Path();
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

        /// <summary>
        ///     测试创建图(不合并节点)
        /// </summary>
        [Test]
        public void TestBuildGraphNoMerge()
        {
            string map1 = GetMap1Path();
            using (var fileStream = File.OpenRead(map1))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, false, false);
                graph.Build();

                int valid = 0;
                var stringBuilder = new StringBuilder();
                foreach (var node in graph.list)
                {
                    stringBuilder.AppendLine(node.ToString());
                    if (node.doors.Count > 0 || node.monsters.Count > 0)
                        valid++;
                }

                stringBuilder.AppendLine(valid + "/" + graph.list.Count + " nodes in total.");

                string result = stringBuilder.ToString();

                string nodesFile = map1.Replace(".txt", "-nodes-no-merge.txt");
                string expected = File.ReadAllText(nodesFile);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
