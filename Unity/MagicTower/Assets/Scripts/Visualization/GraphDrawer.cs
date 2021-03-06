﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Gempoll.Visualization
{
    public class GraphDrawer : MonoBehaviour
    {
        /// <summary>
        ///     线宽
        /// </summary>
        private readonly float lineWidth = 4;

        /// <summary>
        ///     图块高
        /// </summary>
        private readonly float tileHeight = 32f;

        /// <summary>
        ///     图块宽
        /// </summary>
        private readonly float tileWidth = 32f;

        /// <summary>
        ///     背景层根节点
        /// </summary>
        public Transform BackgroundRoot;

        /// <summary>
        ///     门预制体
        /// </summary>
        public GameObject DoorPrefab;

        /// <summary>
        ///     前景层根节点
        /// </summary>
        public Transform ForegroundRoot;

        /// <summary>
        ///     英雄预制体
        /// </summary>
        public GameObject HeroPrefab;

        /// <summary>
        ///     水平的线的预制体
        /// </summary>
        public GameObject HorizonLinePrefab;

        /// <summary>
        ///     道具预制体
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        ///     遮罩预制体 (用于标记当前节点)
        /// </summary>
        public GameObject MaskPrefab;

        /// <summary>
        ///     怪物预制体
        /// </summary>
        public GameObject MonsterPrefab;

        /// <summary>
        ///     节点层根节点
        /// </summary>
        public Transform NodeRoot;

        /// <summary>
        ///     道路预制体
        /// </summary>
        public GameObject RoadPrefab;

        /// <summary>
        ///     商店预制体
        /// </summary>
        public GameObject ShopPrefab;

        public bool ShouldEat;

        public bool ShouldMerge;

        /// <summary>
        ///     向下楼梯预制体
        /// </summary>
        public GameObject StairDownPrefab;

        /// <summary>
        ///     向上楼梯预制体
        /// </summary>
        public GameObject StairUpPrefab;

        /// <summary>
        ///     垂直的线的预制体
        /// </summary>
        public GameObject VerticalLinePrefab;

        /// <summary>
        ///     墙预制体
        /// </summary>
        public GameObject WallPrefab;

        private Coroutine coroutineOfSolve;

        /// <summary>
        ///     合并操作的枚举器
        /// </summary>
        private IEnumerator enumeratorOfMerge;

        /// <summary>
        ///     求解操作的枚举器
        /// </summary>
        private IEnumerator enumeratorOfSolve;

        /// <summary>
        ///     当前楼层
        /// </summary>
        private int floor;

        /// <summary>
        ///     当前图
        /// </summary>
        private Graph graph;

        private bool solved;

        /// <summary>
        ///     描绘当前楼层
        /// </summary>
        private void DrawFloor()
        {
            for (int i = 0; i < graph.GameInfo.RowCount; i++)
            for (int j = 0; j < graph.GameInfo.ColumnCount; j++)
            {
                CreateRoad(i, j);

                int n = graph.GameInfo.Grid[floor, i, j];
                GameObject go = null;
                switch (n)
                {
                    case ObjectId.DOOR_YELLOW:
                    case ObjectId.DOOR_BLUE:
                    case ObjectId.DOOR_RED:
                    case ObjectId.DOOR_GREEN:
                        go = CreateDoor(n);
                        break;
                    case ObjectId.YELLOW_KEY:
                    case ObjectId.BLUE_KEY:
                    case ObjectId.RED_KEY:
                    case ObjectId.GREEN_KEY:
                    case ObjectId.SWORD:
                    case ObjectId.SHIELD:
                    case ObjectId.BLUE_JEWEL:
                    case ObjectId.RED_JEWEL:
                    case ObjectId.GREEN_JEWEL:
                    case ObjectId.RED_POTION:
                    case ObjectId.BLUE_POTION:
                    case ObjectId.YELLOW_POTION:
                    case ObjectId.GREEN_POTION:
                        go = CreateItem(n);
                        break;
                    case ObjectId.ROAD:
                        //go = Instantiate(RoadPrefab, transform);
                        break;
                    case ObjectId.WALL:
                        go = Instantiate(WallPrefab, ForegroundRoot);
                        break;
                    case ObjectId.SHOP:
                        go = Instantiate(ShopPrefab, ForegroundRoot);
                        break;
                    case ObjectId.UPSTAIR:
                        go = Instantiate(StairUpPrefab, ForegroundRoot);
                        break;
                    case ObjectId.DOWNSTAIR:
                        go = Instantiate(StairDownPrefab, ForegroundRoot);
                        break;
                    default:
                        if (n >= ObjectId.MONSTER_BOUND && n <= ObjectId.BOSS_INDEX)
                            go = CreateMonster(n);
                        else
                            Debug.LogError("Unknown id: " + n);

                        break;
                }

                if (go)
                {
                    go.SetActive(true);
                    var t = go.GetComponent<RectTransform>();
                    t.anchoredPosition = CalculatePosition(i, j);
                }
            }

            var heroNode = graph.HeroNode;
            if (heroNode.Floor == floor)
            {
                var go = Instantiate(HeroPrefab, transform);
                go.SetActive(true);
                var t = go.GetComponent<RectTransform>();
                t.anchoredPosition = CalculatePosition(heroNode.X, heroNode.Y);
            }
        }

        /// <summary>
        ///     计算图块的中心位置
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private Vector2 CalculatePosition(int i, int j)
        {
            float x = (i - graph.GameInfo.RowCount / 2f + 0.5f) * tileWidth;
            float y = (j - graph.GameInfo.ColumnCount / 2f + 0.5f) * tileHeight;
            return new Vector2(x, y);
        }

        /// <summary>
        ///     在背景层创建路的图块
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void CreateRoad(int i, int j)
        {
            var go = Instantiate(RoadPrefab, BackgroundRoot);
            go.SetActive(true);
            var t = go.GetComponent<RectTransform>();
            t.anchoredPosition = CalculatePosition(i, j);
        }

        /// <summary>
        ///     在前景层创建怪物图块
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private GameObject CreateMonster(int n)
        {
            var go = Instantiate(MonsterPrefab, ForegroundRoot);
            var monster = go.GetComponent<Monster>();
            monster.SetMonsterId(n);
            return go;
        }

        /// <summary>
        ///     在前景层创建道具图块
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private GameObject CreateItem(int n)
        {
            var go = Instantiate(ItemPrefab, ForegroundRoot);
            var item = go.GetComponent<Item>();
            item.SetItem(n);
            return go;
        }

        /// <summary>
        ///     在前景层创建门图块
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private GameObject CreateDoor(int n)
        {
            var go = Instantiate(DoorPrefab, ForegroundRoot);
            var door = go.GetComponent<Door>();
            door.SetDoorType(Helper.GetDoorType(n));
            return go;
        }

        /// <summary>
        ///     获取地图路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetMapPath(string fileName)
        {
            string mapPath = $"{Application.streamingAssetsPath}/{fileName}";
            return mapPath;
        }

        /// <summary>
        ///     创建图
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="shouldMerge"></param>
        private void ReadGraph(string mapName, bool shouldMerge)
        {
            string mapPath = GetMapPath($"{mapName}.txt");
            using (var fileStream = File.OpenRead(mapPath))
            {
                var scanner = new Scanner(fileStream);
                graph = new Graph(scanner, shouldMerge, ShouldEat);
            }
        }

        private void Start()
        {
            ReadGraph("map2", ShouldMerge);

            floor = 0;
            DrawFloor();

            graph.Build();
            DrawNodes();

            if (!ShouldMerge)
                enumeratorOfMerge = graph.MergeNodeAsync();
        }

        private void OnGUI()
        {
            var rect = new Rect(50, 50, 300, 100);

            if (!ShouldMerge)
            {
                if (enumeratorOfMerge != null)
                    if (GUI.Button(rect, "单步执行合并节点"))
                    {
                        if (enumeratorOfMerge.MoveNext())
                            DrawNodes();
                        else
                            enumeratorOfMerge = null;
                    }

                rect.y += 120;
                if (enumeratorOfMerge != null)
                    if (GUI.Button(rect, "合并节点"))
                    {
                        while (enumeratorOfMerge.MoveNext())
                        {
                        }
                        enumeratorOfMerge = null;
                        DrawNodes();
                    }
            }

            if (!solved)
            {
                rect.y += 120;
                if (GUI.Button(rect, "单步求解"))
                {
                    if (coroutineOfSolve != null)
                        return;

                    if (enumeratorOfSolve == null)
                        enumeratorOfSolve = graph.RunAsync();

                    if (enumeratorOfSolve.MoveNext())
                    {
                        var state = enumeratorOfSolve.Current as State;
                        DrawSolution(state);
                    }
                    else
                    {
                        solved = true;
                        enumeratorOfSolve = null;
                    }
                }

                rect.y += 120;
                if (GUI.Button(rect, "连续求解"))
                {
                    if (enumeratorOfSolve != null)
                        return;

                    coroutineOfSolve = StartCoroutine(SolveAsync());
                }
            }
        }

        private IEnumerator SolveAsync()
        {
            enumeratorOfSolve = graph.RunAsync();
            while (enumeratorOfSolve.MoveNext())
            {
                var state = enumeratorOfSolve.Current as State;
                DrawSolution(state);
                yield return null;
            }

            solved = true;
            enumeratorOfSolve = null;
        }

        private void DrawSolution(State state)
        {
            for (int i = 0; i < NodeRoot.childCount; i++)
                Destroy(NodeRoot.GetChild(i).gameObject);

            foreach (var node in graph.Nodes)
                DrawNode(node);

            // 描绘队列中的State
            int nodesCount = graph.Nodes.Count;
            var visitedNodes = new List<Node>();
            for (int i = 0; i < nodesCount; i++)
            {
                bool visited = state.VisitedNodes[i];
                if (visited)
                    visitedNodes.Add(graph.Nodes[i]);
            }

            // 对所有访问过的几点加遮罩
            foreach (var visitedNode in visitedNodes)
                DrawNodeMask(visitedNode);
        }

        /// <summary>
        ///     描绘所有节点
        /// </summary>
        private void DrawNodes()
        {
            for (int i = 0; i < NodeRoot.childCount; i++)
                Destroy(NodeRoot.GetChild(i).gameObject);

            foreach (var node in graph.Nodes)
            {
                DrawNode(node);

                if (enumeratorOfMerge != null && enumeratorOfMerge.Current == node)
                    DrawNodeMask(node);
            }
        }

        /// <summary>
        ///     当前节点加遮罩
        /// </summary>
        /// <param name="node"></param>
        private void DrawNodeMask(Node node)
        {
            var nodes = new List<Node>(node.MergedNodes) { node };
            foreach (var n in nodes)
            {
                var go = Instantiate(MaskPrefab, NodeRoot);
                go.SetActive(true);
                var t = go.GetComponent<RectTransform>();
                t.anchoredPosition = CalculatePosition(n.X, n.Y);
            }
        }

        /// <summary>
        ///     描绘1个节点
        /// </summary>
        /// <param name="node"></param>
        private void DrawNode(Node node)
        {
            var nodes = new List<Node>(node.MergedNodes) { node };

            foreach (var n in nodes)
            {
                bool foundTopNode = nodes.Any(n2 => n2.X == n.X && n2.Y == n.Y + 1);
                if (!foundTopNode)
                    CreateLine(n.X, n.Y, LinePosition.Top);

                bool foundBottomNode = nodes.Any(n2 => n2.X == n.X && n2.Y == n.Y - 1);
                if (!foundBottomNode)
                    CreateLine(n.X, n.Y, LinePosition.Bottom);

                bool foundLeftNode = nodes.Any(n2 => n2.X == n.X - 1 && n2.Y == n.Y);
                if (!foundLeftNode)
                    CreateLine(n.X, n.Y, LinePosition.Left);

                bool foundRightNode = nodes.Any(n2 => n2.X == n.X + 1 && n2.Y == n.Y);
                if (!foundRightNode)
                    CreateLine(n.X, n.Y, LinePosition.Right);
            }
        }

        /// <summary>
        ///     描边
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="linePosition"></param>
        private void CreateLine(int i, int j, LinePosition linePosition)
        {
            var position = CalculatePosition(i, j);
            if (linePosition == LinePosition.Left)
                position.x = position.x - 0.5f * tileWidth + 0.5f * lineWidth;
            else if (linePosition == LinePosition.Right)
                position.x = position.x + 0.5f * tileWidth - 0.5f * lineWidth;
            else if (linePosition == LinePosition.Top)
                position.y = position.y + 0.5f * tileHeight - 0.5f * lineWidth;
            else if (linePosition == LinePosition.Bottom)
                position.y = position.y - 0.5f * tileHeight + 0.5f * lineWidth;

            var prefab = linePosition == LinePosition.Top || linePosition == LinePosition.Bottom
                ? HorizonLinePrefab
                : VerticalLinePrefab;
            var go = Instantiate(prefab, NodeRoot);
            go.SetActive(true);
            var t = go.GetComponent<RectTransform>();
            t.anchoredPosition = position;
        }

        /// <summary>
        ///     边的位置
        /// </summary>
        private enum LinePosition
        {
            Top,

            Bottom,

            Left,

            Right
        }
    }
}
