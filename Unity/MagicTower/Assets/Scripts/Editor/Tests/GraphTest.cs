using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Gempoll.Editor.Tests
{
    public class GraphTest
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
                var graph = new Graph(scanner, false, false);
                // 起始的3个数据
                Assert.AreEqual(1, graph.GameInfo.FloorCount);
                Assert.AreEqual(11, graph.GameInfo.RowCount);
                Assert.AreEqual(11, graph.GameInfo.ColumnCount);

                // 最后的3个数据
                Assert.AreEqual(0, graph.HeroNode.Floor);
                Assert.AreEqual(0, graph.HeroNode.X);
                Assert.AreEqual(5, graph.HeroNode.Y);

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
            string mapPath = Helper.GetStreamingAssetPath($"{mapName}.txt");
            using (var fileStream = File.OpenRead(mapPath))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, true, true);
                graph.Build();
                var answer = graph.Run();

                var stringBuilder = new StringBuilder();

                if (answer == null)
                {
                    stringBuilder.AppendLine("No solution!");
                }
                else
                {
                    foreach (string s in answer.Route)
                    {
                        stringBuilder.AppendLine(s);
                    }
                }

                string result = stringBuilder.ToString();
                string answerPath = Helper.GetStreamingAssetPath($"{mapName}-answer.txt");
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
            string mapPath = Helper.GetStreamingAssetPath($"{mapName}.txt");
            using (var fileStream = File.OpenRead(mapPath))
            {
                var scanner = new Scanner(fileStream);
                var graph = new Graph(scanner, shouldMerge, false);
                graph.Build();

                int valid = 0;
                var stringBuilder = new StringBuilder();
                foreach (var node in graph.Nodes)
                {
                    stringBuilder.AppendLine(node.ToString());
                    if (node.Doors.Count > 0 || node.Monsters.Count > 0)
                        valid++;
                }

                stringBuilder.AppendLine(valid + "/" + graph.Nodes.Count + " nodes in total.");

                string result = stringBuilder.ToString();

                string postfix = shouldMerge ? "" : "-no-merge";
                string nodesPath = Helper.GetStreamingAssetPath($"{mapName}-nodes{postfix}.txt");
                string expected = File.ReadAllText(nodesPath);

                Assert.AreEqual(expected, result);
            }
        }
    }
}
