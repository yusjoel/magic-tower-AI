using System;
using System.Collections;
using System.Collections.Generic;
using Medallion.Collections;
using NUnit.Framework;

namespace Gempoll
{
    /// <summary>
    ///     图 (TODO: 职责太多, 需要拆分)
    /// </summary>
    public class Graph
    {
        /// <summary>
        ///     层的列数
        /// </summary>
        public readonly int ColumnCount;

        /// <summary>
        ///     总层数
        /// </summary>
        public readonly int FloorCount;

        /// <summary>
        ///     节点列表
        /// </summary>
        public readonly List<Node> Nodes;

        // TODO: 读取的地图信息应该独立出去成为一个类
        /// <summary>
        ///     地图
        /// </summary>
        public readonly int[,,] Map;

        /// <summary>
        ///     层的行数
        /// </summary>
        public readonly int RowCount;

        private readonly Dictionary<int, Monster> monsterMap;

        private readonly int attackOfRedJewel;

        private readonly int hitPointOfBluePotion;

        private readonly int defenseOfBlueJewel;

        private readonly int hitPointOfGreenPotion;

        private readonly int magicDefenseOfGreenJewel;

        private readonly int hitPointOfRedPotion;

        private readonly int defenseOfShield;

        private readonly int attackOfSword;

        private readonly int hitPointOfYellowPotion;

        private readonly bool shouldMerge;

        /// <summary>
        ///     楼梯
        ///     <para>默认每个层最多只有一个向上楼梯和一个向下楼梯</para>
        ///     <para>第1维: 楼层编号</para>
        ///     <para>第2维: 4个整型, 代表向上楼梯的坐标, 和向下楼梯的坐标</para>
        /// </summary>
        private readonly int[][] stair;

        public int NodeIdOfBoss = -1;

        public Node HeroNode;

        public Shop Shop;

        public bool ShouldEat;

        public Graph(Scanner scanner, bool shouldMerge, bool shouldEat)
        {
            Nodes = new List<Node>();
            FloorCount = scanner.NextInt();
            RowCount = scanner.NextInt();
            ColumnCount = scanner.NextInt();
            Map = new int[FloorCount, RowCount, ColumnCount];
            stair = new int[FloorCount][];

            for (int i = 0; i < FloorCount; i++)
                stair[i] = new[] { -1, -1, -1, -1 };

            for (int i = 0; i < FloorCount; i++)
            for (int j = 0; j < RowCount; j++)
            for (int k = 0; k < ColumnCount; k++)
                Map[i, j, k] = scanner.NextInt();

            // 读取道具属性
            attackOfRedJewel = scanner.NextInt();
            defenseOfBlueJewel = scanner.NextInt();
            magicDefenseOfGreenJewel = scanner.NextInt();
            hitPointOfRedPotion = scanner.NextInt();
            hitPointOfBluePotion = scanner.NextInt();
            hitPointOfYellowPotion = scanner.NextInt();
            hitPointOfGreenPotion = scanner.NextInt();
            attackOfSword = scanner.NextInt();
            defenseOfShield = scanner.NextInt();

            // 读取怪物列表
            monsterMap = new Dictionary<int, Monster>();
            int monsterCount = scanner.NextInt();
            for (int i = 0; i < monsterCount; i++)
            {
                int id = scanner.NextInt();
                var monster = new Monster(id, scanner.NextInt(), scanner.NextInt(), scanner.NextInt(),
                    scanner.NextInt(), scanner.NextInt());
                monsterMap.Add(id, monster);
            }

            // 读取商店信息
            Shop = new Shop(scanner.NextInt(), scanner.NextInt(), scanner.NextInt(),
                scanner.NextInt(), scanner.NextInt(), scanner.NextInt());

            // 读取英雄初始状态和位置
            int hitPoint = scanner.NextInt();
            int attack = scanner.NextInt();
            int defense = scanner.NextInt();
            int magicDefense = scanner.NextInt();
            int money = scanner.NextInt();
            int yellowKeyCount = scanner.NextInt();
            int blueKeyCount = scanner.NextInt();
            int redKeyCount = scanner.NextInt();
            int floor = scanner.NextInt();
            int x = scanner.NextInt();
            int y = scanner.NextInt();

            var hero = new Hero(hitPoint, attack, defense, magicDefense, money, yellowKeyCount, blueKeyCount,
                redKeyCount, 0);
            HeroNode = new Node(0, floor, x, y).SetHero(hero);

            this.shouldMerge = shouldMerge;
            ShouldEat = shouldEat;
        }

        public void Build()
        {
            Nodes.Add(HeroNode);

            BuildMap();

            if (shouldMerge)
                MergeNode();

            // set id
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].SetId(i);
                if (Nodes[i].ObjectId == ObjectId.BOSS_INDEX)
                    NodeIdOfBoss = i;
            }
        }

        private void BuildMap()
        {
            for (int i = 0; i < FloorCount; i++)
            for (int j = 0; j < RowCount; j++)
            for (int k = 0; k < ColumnCount; k++)
            {
                Node node = null;
                if (Map[i, j, k] == ObjectId.UPSTAIR)
                {
                    stair[i][0] = j;
                    stair[i][1] = k;
                }
                if (Map[i, j, k] == ObjectId.DOWNSTAIR)
                {
                    stair[i][2] = j;
                    stair[i][3] = k;
                }
                if (Map[i, j, k] == ObjectId.YELLOW_KEY)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetYellowKeyCount(1));
                if (Map[i, j, k] == ObjectId.BLUE_KEY)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetBlueKeyCount(1));
                if (Map[i, j, k] == ObjectId.RED_KEY)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetRedKeyCount(1));
                if (Map[i, j, k] == ObjectId.GREEN_KEY)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetGreenKeyCount(1));
                if (Map[i, j, k] == ObjectId.RED_JEWEL)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetAttack(attackOfRedJewel));
                if (Map[i, j, k] == ObjectId.BLUE_JEWEL)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetDefense(defenseOfBlueJewel));
                if (Map[i, j, k] == ObjectId.GREEN_JEWEL)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(
                        new Item().SetMagicDefense(magicDefenseOfGreenJewel));
                if (Map[i, j, k] == ObjectId.RED_POTION)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetHitPoint(hitPointOfRedPotion));
                if (Map[i, j, k] == ObjectId.BLUE_POTION)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetHitPoint(hitPointOfBluePotion));
                if (Map[i, j, k] == ObjectId.YELLOW_POTION)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetHitPoint(hitPointOfYellowPotion));
                if (Map[i, j, k] == ObjectId.GREEN_POTION)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetHitPoint(hitPointOfGreenPotion));
                if (Map[i, j, k] == ObjectId.SWORD)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetAttack(attackOfSword));
                if (Map[i, j, k] == ObjectId.SHIELD)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetDefense(defenseOfShield));
                if (Map[i, j, k] == ObjectId.SHOP)
                    node = new Node(Map[i, j, k], i, j, k).SetItem(new Item().SetSpecial(1));

                if (Map[i, j, k] == ObjectId.DOOR_YELLOW)
                    node = new Node(Map[i, j, k], i, j, k).SetDoor(1);
                if (Map[i, j, k] == ObjectId.DOOR_BLUE)
                    node = new Node(Map[i, j, k], i, j, k).SetDoor(2);
                if (Map[i, j, k] == ObjectId.DOOR_RED)
                    node = new Node(Map[i, j, k], i, j, k).SetDoor(3);
                if (Map[i, j, k] == ObjectId.DOOR_GREEN)
                    node = new Node(Map[i, j, k], i, j, k).SetDoor(4);
                if (Map[i, j, k] >= ObjectId.MONSTER_BOUND)
                {
                    var monster = monsterMap[Map[i, j, k]];
                    if (monster == null) continue;

                    node = new Node(Map[i, j, k], i, j, k).SetMonster(monster);
                }

                if (node != null)
                    Nodes.Add(node);
            }

            // build graph
            int len = Nodes.Count;
            for (int i = 0; i < len; i++)
            for (int j = i + 1; j < len; j++)
            {
                var n1 = Nodes[i];
                var n2 = Nodes[j];
                if (IsLinked(n1.Floor, n1.X, n1.Y, n2.Floor, n2.X, n2.Y))
                {
                    n1.Link(n2);
                    n2.Link(n1);
                }
            }
        }

        private bool Check(Node u, Node v)
        {
            foreach (var x in u.LinkedNodes)
            foreach (var y in u.LinkedNodes)
            {
                if (x == y || x == v || y == v) continue;

                if (!x.LinkedNodes.Contains(y) || !y.LinkedNodes.Contains(x)) return false;
            }
            return true;
        }

        /// <summary>
        ///     判断两个节点是否连通
        /// </summary>
        /// <param name="f1">节点1的层</param>
        /// <param name="x1">节点1的x坐标</param>
        /// <param name="y1">节点1的y坐标</param>
        /// <param name="f2">节点2的层</param>
        /// <param name="x2">节点2的x坐标</param>
        /// <param name="y2">节点2的y坐标</param>
        /// <returns></returns>
        private bool IsLinked(int f1, int x1, int y1, int f2, int x2, int y2)
        {
            // 不同层的的情况
            // 总是令节点1高于节点2
            if (f1 < f2) return IsLinked(f2, x2, y2, f1, x1, y1);

            // 转化为节点1连通向下楼梯以及节点2连通向上楼梯
            if (f1 != f2)
                return IsLinked(f1, x1, y1, f1, stair[f1][2], stair[f1][3])
                    && IsLinked(f1 - 1, stair[f1 - 1][0], stair[f1 - 1][1], f2, x2, y2);

            // 同层的情况
            if (x1 == x2 && y1 == y2) return true;

            var visited = new bool[RowCount, ColumnCount];
            visited[x1, y1] = true;

            var queue = new Queue<int>();
            queue.Enqueue(x1);
            queue.Enqueue(y1);
            while (queue.Count > 0)
            {
                int x = queue.Dequeue();
                int y = queue.Dequeue();
                var directions = new[] { new[] { -1, 0 }, new[] { 0, 1 }, new[] { 1, 0 }, new[] { 0, -1 } };
                foreach (var dir in directions)
                {
                    int nx = x + dir[0], ny = y + dir[1];
                    if (nx < 0 || nx >= RowCount || ny < 0 || ny >= ColumnCount) continue;

                    if (nx == x2 && ny == y2) return true;

                    if (visited[nx, ny] || Map[f1, nx, ny] != ObjectId.ROAD
                        && Map[f1, nx, ny] != ObjectId.UPSTAIR && Map[f1, nx, ny] != ObjectId.DOWNSTAIR)
                        continue;

                    visited[nx, ny] = true;
                    queue.Enqueue(nx);
                    queue.Enqueue(ny);
                }
            }

            return false;
        }

        private void MergeNode()
        {
            for (int i = 1; i < Nodes.Count; i++)
            {
                var n1 = Nodes[i];
                for (int j = i + 1; j < Nodes.Count; j++)
                {
                    var n2 = Nodes[j];
                    if (ShouldMerge(n1, n2))
                    {
                        n1.Merge(n2);
                        Nodes.RemoveAt(j);
                        MergeNode();
                        return;
                    }
                }
            }
        }

        public IEnumerator MergeNodeAsync()
        {
            bool merged = false;

            for (int i = 1; i < Nodes.Count; i++)
            {
                var n1 = Nodes[i];
                for (int j = i + 1; j < Nodes.Count; j++)
                {
                    var n2 = Nodes[j];
                    if (ShouldMerge(n1, n2))
                    {
                        n1.Merge(n2);
                        Nodes.RemoveAt(j);
                        merged = true;
                        break;
                    }
                }
                if (merged)
                    break;

                yield return n1;
            }

            if (merged)
            {
                var enumerator = MergeNodeAsync();
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

    #if false
        private void PrintQueue(PriorityQueue<State> queue)
        {
            TestContext.Out.WriteLine("=====QUEUE=====");
            var array = new State[queue.Count];
            var collection = (ICollection<State>) queue;
            collection.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                var state = array[i];
                TestContext.Out.WriteLine("{0}, id: {1}, cnt: {2}, score: {3}", i, state.Id, state.Count,
                    state.GetScore());
            }
        }
    #endif

        public IEnumerator RunAsync()
        {
            var state = new State(this, Nodes[0]);
            State answer = null;

            var set = new HashSet<long>();
            var map = new Dictionary<long, int>();

            var queue = new PriorityQueue<State>(State.GetComparer());

            int stateId = 1;

            queue.Enqueue(state);

            while (queue.Count > 0)
            {
                state = queue.Dequeue();
                yield return state;

                if (!set.Add(state.Hash()))
                    continue;

                if (state.ShouldStop())
                {
                    if (answer == null || answer.GetScore() < state.GetScore())
                        answer = state;
                    continue;
                }

                // extend
                foreach (var node in state.Current.LinkedNodes)
                {
                    // visited
                    if (state.VisitedNodes[node.Id]) continue;

                    // extend
                    var another = new State(state).Merge(node);
                    if (another == null) continue;

                    long hash = another.Hash();
                    int score;
                    if (!map.TryGetValue(hash, out score))
                        score = -1;
                    if (score > another.GetScore()) continue;

                    map[hash] = another.GetScore();
                    another.Id = stateId++;
                    queue.Enqueue(another);
                }
            }
        }

        public State Run()
        {
            var state = new State(this, Nodes[0]);
            State answer = null;

            int index = 0, solutions = 0;

            var set = new HashSet<long>();
            var map = new Dictionary<long, int>();

            // !!! start bfs !!!!!

            int start = DateTime.Now.Millisecond;

            var queue = new PriorityQueue<State>(State.GetComparer());

            int stateId = 1;

            queue.Enqueue(state);

            while (queue.Count > 0)
            {
                state = queue.Dequeue();
                //PrintQueue(queue);

                //if (state.GetScore() == 1035)
                //    break;
                //Console.WriteLine("Poll: {0}, Cnt {1}, Score {2}", state.current, state.cnt, state.GetScore());

                if (!set.Add(state.Hash())) continue;

                if (state.ShouldStop())
                {
                    if (answer == null || answer.GetScore() < state.GetScore())
                        answer = state;
                    solutions++;
                    continue;
                }

                // extend
                foreach (var node in state.Current.LinkedNodes)
                {
                    // visited
                    if (state.VisitedNodes[node.Id]) continue;

                    // extend
                    var another = new State(state).Merge(node);
                    if (another == null) continue;

                    long hash = another.Hash();
                    int score;
                    if (!map.TryGetValue(hash, out score))
                        score = -1;
                    if (score > another.GetScore()) continue;

                    map[hash] = another.GetScore();
                    another.Id = stateId++;
                    queue.Enqueue(another);

                    //PrintQueue(queue);
                }

                index++;
                if (index % 1000 == 0)
                    TestContext.Out.WriteLine("Calculating... {0} calculated, {1} still in queue.", index, queue.Count);
            }
            TestContext.Out.WriteLine("cnt: " + index + "; solutions: " + solutions);

            //if (answer == null)
            //{
            //    Console.WriteLine("No solution!");
            //}
            //else
            //{
            //    foreach (string s in answer.route)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}

            int end = DateTime.Now.Millisecond;
            TestContext.Out.WriteLine("Time used: {0:f3}s", (end - start) / 1000f);

            return answer;
        }

        private bool ShouldMerge(Node n1, Node n2)
        {
            // 说明参考下面内的文章
            // https://ckcz123.com/blog/posts/magic-tower-ai-ii/

            // 1. 如果这两个节点不相连，则不合并。
            if (!n1.LinkedNodes.Contains(n2) || !n2.LinkedNodes.Contains(n1))
                return false;

            // 2. 如果这两个节点都是宝物节点，则直接合并。
            if (n1.Item != null && n2.Item != null)
                return true;

            // 3. 如果一个是宝物节点，另一个是消耗节点，则不合并。
            if (n1.Item != null || n2.Item != null)
                return false;

            // 任意一个是Boss节点, 不能合并
            if (n1.ObjectId == ObjectId.BOSS_INDEX || n2.ObjectId == ObjectId.BOSS_INDEX)
                return false;

            // 4. 如果都是消耗节点，且存在第三个节点同时和这两个节点相连，则不合并
            foreach (var node in n2.LinkedNodes)
                if (n1.LinkedNodes.Contains(node))
                    return false;

            // 5. 如果都是消耗节点，且其中某个节点相连的其他所有节点，不是两两相连，则不合并。
            // 4.5.都是为了防止从任意节点打开了n1, n2中任一节点, 出现新的选项
            // TODO:这里可以优化, 应该考虑英雄的位置
            if (!Check(n1, n2))
                return false;

            if (!Check(n2, n1))
                return false;

            return true;
        }
    }
}
