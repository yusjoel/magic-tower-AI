using Medallion.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Gempoll
{
    public class Graph
    {
        public static readonly int ROAD = 0;

        public static readonly int WALL = 1;

        public static readonly int RED_JEWEL = 27;

        public static readonly int BLUE_JEWEL = 28;

        public static readonly int GREEN_JEWEL = 29;

        public static readonly int YELLOW_KEY = 21;

        public static readonly int BLUE_KEY = 22;

        public static readonly int RED_KEY = 23;

        public static readonly int GREEN_KEY = 24;

        public static readonly int RED_POTION = 31;

        public static readonly int BLUE_POTION = 32;

        public static readonly int GREEN_POTION = 33;

        public static readonly int YELLOW_POTION = 34;

        public static readonly int SWORD = 35;

        public static readonly int SHIELD = 36;

        public static readonly int SHOP = 40;

        public static readonly int DOOR_YELLOW = 81;

        public static readonly int DOOR_BLUE = 82;

        public static readonly int DOOR_RED = 83;

        public static readonly int DOOR_GREEN = 84;

        public static readonly int UPSTAIR = 87;

        public static readonly int DOWNSTAIR = 88;

        public static readonly int MONSTER_BOUND = 201;

        public static readonly int BOSS_INDEX = 299;

        /// <summary>
        ///     层的列数
        /// </summary>
        public readonly int columnCount;

        /// <summary>
        ///     总层数
        /// </summary>
        public readonly int floorCount;

        private readonly Hero hero;

        /// <summary>
        ///     节点列表
        /// </summary>
        public readonly List<Node> list;

        /// <summary>
        ///     地图
        /// </summary>
        private readonly int[,,] map;

        private readonly Dictionary<int, Monster> monsterMap;

        private readonly int p_atk;

        private readonly int p_blue;

        private readonly int p_def;

        private readonly int p_green;

        private readonly int p_mdef;

        private readonly int p_red;

        private readonly int p_shield;

        private readonly int p_sword;

        private readonly int p_yellow;

        /// <summary>
        ///     层的行数
        /// </summary>
        public readonly int rowCount;

        private readonly bool shouldMerge;

        /// <summary>
        ///     楼梯
        ///     <para>默认每个层最多只有一个向上楼梯和一个向下楼梯</para>
        ///     <para>第1维: 楼层编号</para>
        ///     <para>第2维: 4个整型, 代表向上楼梯的坐标, 和向下楼梯的坐标</para>
        /// </summary>
        private readonly int[][] stair;

        public int bossId = -1;

        public Node heroNode;

        public Shop shop;

        public bool shouldEat;

        public Graph(Scanner scanner, bool shouldMerge, bool shouldEat)
        {
            list = new List<Node>();
            floorCount = scanner.NextInt();
            rowCount = scanner.NextInt();
            columnCount = scanner.NextInt();
            map = new int[floorCount, rowCount, columnCount];
            stair = new int[floorCount][];

            for (int i = 0; i < floorCount; i++)
                stair[i] = new[] { -1, -1, -1, -1 };

            for (int i = 0; i < floorCount; i++)
            for (int j = 0; j < rowCount; j++)
            for (int k = 0; k < columnCount; k++)
                map[i, j, k] = scanner.NextInt();

            // 读取道具属性
            p_atk = scanner.NextInt();
            p_def = scanner.NextInt();
            p_mdef = scanner.NextInt();
            p_red = scanner.NextInt();
            p_blue = scanner.NextInt();
            p_yellow = scanner.NextInt();
            p_green = scanner.NextInt();
            p_sword = scanner.NextInt();
            p_shield = scanner.NextInt();

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
            shop = new Shop(scanner.NextInt(), scanner.NextInt(), scanner.NextInt(),
                scanner.NextInt(), scanner.NextInt(), scanner.NextInt());

            // 读取英雄初始状态和位置
            int hp = scanner.NextInt();
            int atk = scanner.NextInt();
            int def = scanner.NextInt();
            int mdef = scanner.NextInt();
            int money = scanner.NextInt();
            int yellow = scanner.NextInt();
            int blue = scanner.NextInt();
            int red = scanner.NextInt();
            int floor = scanner.NextInt();
            int x = scanner.NextInt();
            int y = scanner.NextInt();

            hero = new Hero(hp, atk, def, mdef, money, yellow, blue, red, 0);
            heroNode = new Node(0, floor, x, y).setHero(hero);

            this.shouldMerge = shouldMerge;
            this.shouldEat = shouldEat;
        }

        public void Build()
        {
            list.Add(heroNode);

            buildMap();

            if (shouldMerge)
                mergeNode();

            // set id
            for (int i = 0; i < list.Count; i++)
            {
                list[i].setId(i);
                if (list[i].type == BOSS_INDEX)
                    bossId = i;
            }
        }

        private void mergeNode()
        {
            for (int i = 1; i < list.Count; i++)
            {
                var n1 = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    var n2 = list[j];
                    if (ShouldMerge(n1, n2))
                    {
                        n1.merge(n2);
                        list.RemoveAt(j);
                        mergeNode();
                        return;
                    }
                }
            }
        }

        private bool ShouldMerge(Node n1, Node n2)
        {
            // 说明参考下面内的文章
            // https://ckcz123.com/blog/posts/magic-tower-ai-ii/

            // 1. 如果这两个节点不相连，则不合并。
            if (!n1.linked.Contains(n2) || !n2.linked.Contains(n1))
                return false;

            // 2. 如果这两个节点都是宝物节点，则直接合并。
            if (n1.item != null && n2.item != null)
                return true;

            // 3. 如果一个是宝物节点，另一个是消耗节点，则不合并。
            if (n1.item != null || n2.item != null)
                return false;

            // 任意一个是Boss节点, 不能合并
            if (n1.type == BOSS_INDEX || n2.type == BOSS_INDEX)
                return false;

            // 4. 如果都是消耗节点，且存在第三个节点同时和这两个节点相连，则不合并
            foreach (var node in n2.linked)
                if (n1.linked.Contains(node))
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

        private bool Check(Node u, Node v)
        {
            foreach (var x in u.linked)
            foreach (var y in u.linked)
            {
                if (x == y || x == v || y == v) continue;

                if (!x.linked.Contains(y) || !y.linked.Contains(x)) return false;
            }
            return true;
        }

        private void buildMap()
        {
            for (int i = 0; i < floorCount; i++)
            for (int j = 0; j < rowCount; j++)
            for (int k = 0; k < columnCount; k++)
            {
                Node node = null;
                if (map[i, j, k] == UPSTAIR)
                {
                    stair[i][0] = j;
                    stair[i][1] = k;
                }
                if (map[i, j, k] == DOWNSTAIR)
                {
                    stair[i][2] = j;
                    stair[i][3] = k;
                }
                if (map[i, j, k] == YELLOW_KEY)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setYellow(1));
                if (map[i, j, k] == BLUE_KEY)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setBlue(1));
                if (map[i, j, k] == RED_KEY)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setRed(1));
                if (map[i, j, k] == GREEN_KEY)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setGreen(1));
                if (map[i, j, k] == RED_JEWEL)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setAtk(p_atk));
                if (map[i, j, k] == BLUE_JEWEL)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setDef(p_def));
                if (map[i, j, k] == GREEN_JEWEL)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setMdef(p_mdef));
                if (map[i, j, k] == RED_POTION)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setHp(p_red));
                if (map[i, j, k] == BLUE_POTION)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setHp(p_blue));
                if (map[i, j, k] == YELLOW_POTION)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setHp(p_yellow));
                if (map[i, j, k] == GREEN_POTION)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setHp(p_green));
                if (map[i, j, k] == SWORD)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setAtk(p_sword));
                if (map[i, j, k] == SHIELD)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setDef(p_shield));
                if (map[i, j, k] == SHOP)
                    node = new Node(map[i, j, k], i, j, k).setItem(new Item().setSpecial(1));

                if (map[i, j, k] == DOOR_YELLOW)
                    node = new Node(map[i, j, k], i, j, k).setDoor(1);
                if (map[i, j, k] == DOOR_BLUE)
                    node = new Node(map[i, j, k], i, j, k).setDoor(2);
                if (map[i, j, k] == DOOR_RED)
                    node = new Node(map[i, j, k], i, j, k).setDoor(3);
                if (map[i, j, k] == DOOR_GREEN)
                    node = new Node(map[i, j, k], i, j, k).setDoor(4);
                if (map[i, j, k] >= MONSTER_BOUND)
                {
                    var monster = monsterMap[map[i, j, k]];
                    if (monster == null) continue;

                    node = new Node(map[i, j, k], i, j, k).setMonster(monster);
                }

                if (node != null)
                    list.Add(node);
            }

            // build graph
            int len = list.Count;
            for (int i = 0; i < len; i++)
            for (int j = i + 1; j < len; j++)
            {
                var n1 = list[i];
                var n2 = list[j];
                if (isLinked(n1.f, n1.x, n1.y, n2.f, n2.x, n2.y))
                {
                    n1.addNode(n2);
                    n2.addNode(n1);
                }
            }
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
        private bool isLinked(int f1, int x1, int y1, int f2, int x2, int y2)
        {
            // 不同层的的情况
            // 总是令节点1高于节点2
            if (f1 < f2) return isLinked(f2, x2, y2, f1, x1, y1);

            // 转化为节点1连通向下楼梯以及节点2连通向上楼梯
            if (f1 != f2)
                return isLinked(f1, x1, y1, f1, stair[f1][2], stair[f1][3])
                    && isLinked(f1 - 1, stair[f1 - 1][0], stair[f1 - 1][1], f2, x2, y2);

            // 同层的情况
            if (x1 == x2 && y1 == y2) return true;

            var visited = new bool[rowCount, columnCount];
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
                    if (nx < 0 || nx >= rowCount || ny < 0 || ny >= columnCount) continue;

                    if (nx == x2 && ny == y2) return true;

                    if (visited[nx, ny] || map[f1, nx, ny] != ROAD
                        && map[f1, nx, ny] != UPSTAIR && map[f1, nx, ny] != DOWNSTAIR)
                        continue;

                    visited[nx, ny] = true;
                    queue.Enqueue(nx);
                    queue.Enqueue(ny);
                }
            }

            return false;
        }

        private void PrintQueue(PriorityQueue<State> queue)
        {
            TestContext.Out.WriteLine("=====QUEUE=====");
            var array = new State[queue.Count];
            var collection = (ICollection<State>) queue;
            collection.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                var state = array[i];
                TestContext.Out.WriteLine("{0}, id: {1}, cnt: {2}, score: {3}", i, state.Id, state.cnt, state.getScore());
            }
        }

        public State run()
        {
            var state = new State(this, list[0]);
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

                //if (state.getScore() == 1035)
                //    break;
                //Console.WriteLine("Poll: {0}, Cnt {1}, Score {2}", state.current, state.cnt, state.getScore());

                if (!set.Add(state.hash())) continue;

                if (state.shouldStop())
                {
                    if (answer == null || answer.getScore() < state.getScore())
                        answer = state;
                    solutions++;
                    continue;
                }

                // extend
                foreach (var node in state.current.linked)
                {
                    // visited
                    if (state.visited[node.id]) continue;

                    // extend
                    var another = new State(state).merge(node);
                    if (another == null) continue;

                    long hash = another.hash();
                    int score;
                    if (!map.TryGetValue(hash, out score))
                        score = -1;
                    if (score > another.getScore()) continue;

                    map[hash] = another.getScore();
                    another.Id = stateId++;
                    queue.Enqueue(another);

                    //PrintQueue(queue);
                }

                index++;
                if (index % 1000 == 0)
                {
                    TestContext.Out.WriteLine("Calculating... {0} calculated, {1} still in queue.", index, queue.Count);
                }
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
    }
}
