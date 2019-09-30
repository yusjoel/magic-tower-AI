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

        /// <summary>
        /// 节点列表
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

        /// <summary>
        ///     楼梯
        ///     <para>默认每个层最多只有一个向上楼梯和一个向下楼梯</para>
        ///     <para>第1维: 楼层编号</para>
        ///     <para>第2维: 4个整型, 代表向上楼梯的坐标, 和向下楼梯的坐标</para>
        /// </summary>
        private readonly int[][] stair;

        private int bossId = -1;

        private readonly Hero hero;

        public Node heroNode;

        private Shop shop;

        private bool shouldEat;

        private readonly bool shouldMerge;

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
    }
}
