using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Gempoll.Editor.Tests
{
    public class GraphTest
    {
        private string GetMapPath(string fileName)
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
            string map1 = $"{basePath}/Assets/StreamingAssets/{fileName}";
            return map1;
        }

        /// <summary>
        ///     测试读取地图文件
        /// </summary>
        [Test]
        public void TestReadMap()
        {
            string map1 = GetMapPath("map1.txt");
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
        public void TestBuildGraphNoMerge1()
        {
            BuildGraph("map1", false);
        }

        [Test]
        public void TestBuildGraphNoMerge2()
        {
            BuildGraph("map2", false);
        }

        /// <summary>
        ///     测试创建图(合并节点)
        /// </summary>
        [Test]
        public void TestBuildGraph1()
        {
            BuildGraph("map1", true);
        }

        [Test]
        public void TestBuildGraph2()
        {
            BuildGraph("map2", true);
        }

        [Test]
        public void TestRunGraph1()
        {
            RunGraph("map1");
        }

        [Test]
        public void TestRunGraph2()
        {
            RunGraph("map2");
        }

        /// <summary>
        ///  求解图
        /// </summary>
        /// <param name="mapName"></param>
        private void RunGraph(string mapName)
        {
            string mapPath = GetMapPath($"{mapName}.txt");
            using (var fileStream = File.OpenRead(mapPath))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, true, true);
                graph.Build();
                var answer = graph.run();

                var stringBuilder = new StringBuilder();

                if (answer == null)
                {
                    stringBuilder.AppendLine("No solution!");
                }
                else
                {
                    foreach (string s in answer.route)
                    {
                        stringBuilder.AppendLine(s);
                    }
                }

                string result = stringBuilder.ToString();
                string answerPath = GetMapPath($"{mapName}-answer.txt");
                string expected = File.ReadAllText(answerPath);

                Assert.AreEqual(expected, result);
            }
        }

        /// <summary>
        ///  创建图
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="shouldMerge"></param>
        private void BuildGraph(string mapName, bool shouldMerge)
        {
            string mapPath = GetMapPath($"{mapName}.txt");
            using (var fileStream = File.OpenRead(mapPath))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, shouldMerge, false);
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

                string postfix = shouldMerge ? "" : "-no-merge";
                string nodesPath = GetMapPath($"{mapName}-nodes{postfix}.txt");
                string expected = File.ReadAllText(nodesPath);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
