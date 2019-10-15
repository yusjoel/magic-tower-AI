using System.IO;
using UnityEngine;

namespace Gempoll.Visualization
{
    public class GraphDrawer : MonoBehaviour
    {
        /// <summary>
        ///     门预制体
        /// </summary>
        public GameObject DoorPrefab;

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
        ///     怪物预制体
        /// </summary>
        public GameObject MonsterPrefab;

        /// <summary>
        ///     道路预制体
        /// </summary>
        public GameObject RoadPrefab;

        /// <summary>
        ///     商店预制体
        /// </summary>
        public GameObject ShopPrefab;

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

        /// <summary>
        ///     当前楼层
        /// </summary>
        private int floor;

        /// <summary>
        ///     当前图
        /// </summary>
        private Graph graph;

        private float lineWidth = 4;

        private readonly float spriteHeight = 32f;

        private readonly float spriteWidth = 32f;

        /// <summary>
        ///     描绘当前楼层
        /// </summary>
        private void DrawFloor()
        {
            for (int i = 0; i < graph.rowCount; i++)
            for (int j = 0; j < graph.columnCount; j++)
            {
                CreateRoad(i, j);

                int n = graph.map[floor, i, j];
                GameObject go = null;
                switch (n)
                {
                    case Graph.DOOR_YELLOW:
                    case Graph.DOOR_BLUE:
                    case Graph.DOOR_RED:
                    case Graph.DOOR_GREEN:
                        go = CreateDoor(n);
                        break;
                    case Graph.YELLOW_KEY:
                    case Graph.BLUE_KEY:
                    case Graph.RED_KEY:
                    case Graph.GREEN_KEY:
                    case Graph.SWORD:
                    case Graph.SHIELD:
                    case Graph.BLUE_JEWEL:
                    case Graph.RED_JEWEL:
                    case Graph.GREEN_JEWEL:
                    case Graph.RED_POTION:
                    case Graph.BLUE_POTION:
                    case Graph.YELLOW_POTION:
                    case Graph.GREEN_POTION:
                        go = CreateItem(n);
                        break;
                    case Graph.ROAD:
                        //go = Instantiate(RoadPrefab, transform);
                        break;
                    case Graph.WALL:
                        go = Instantiate(WallPrefab, transform);
                        break;
                    case Graph.SHOP:
                        go = Instantiate(ShopPrefab, transform);
                        break;
                    case Graph.UPSTAIR:
                        go = Instantiate(StairUpPrefab, transform);
                        break;
                    case Graph.DOWNSTAIR:
                        go = Instantiate(StairDownPrefab, transform);
                        break;
                    default:
                        if (n >= Graph.MONSTER_BOUND && n <= Graph.BOSS_INDEX)
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

            var heroNode = graph.heroNode;
            if (heroNode.f == floor)
            {
                var go = Instantiate(HeroPrefab, transform);
                go.SetActive(true);
                var t = go.GetComponent<RectTransform>();
                t.anchoredPosition = CalculatePosition(heroNode.x, heroNode.y);
            }
        }

        private Vector2 CalculatePosition(int i, int j)
        {
            float x = (i - graph.rowCount / 2f + 0.5f) * spriteWidth;
            float y = (j - graph.columnCount / 2f + 0.5f) * spriteHeight;
            return new Vector2(x, y);
        }

        private void CreateRoad(int i, int j)
        {
            var go = Instantiate(RoadPrefab, transform);
            go.SetActive(true);
            var t = go.GetComponent<RectTransform>();
            t.anchoredPosition = CalculatePosition(i, j);
        }

        private GameObject CreateMonster(int n)
        {
            var go = Instantiate(MonsterPrefab, transform);
            var monster = go.GetComponent<Monster>();
            monster.SetMonsterId(n);
            return go;
        }

        private GameObject CreateItem(int n)
        {
            var go = Instantiate(ItemPrefab, transform);
            var item = go.GetComponent<Item>();
            item.SetItem(n);
            return go;
        }

        private GameObject CreateDoor(int n)
        {
            var go = Instantiate(DoorPrefab, transform);
            var door = go.GetComponent<Door>();
            door.SetDoorType(Helper.GetDoorType(n));
            return go;
        }

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
                graph = new Graph(scanner, shouldMerge, false);
            }
        }

        private void Start()
        {
            ReadGraph("map2", false);

            floor = 0;
            DrawFloor();

            graph.Build();
            DrawNodes();
        }

        private enum LinePosition
        {
            Top,

            Bottom,

            Left,

            Right
        }

        private void DrawNodes()
        {
            foreach (var node in graph.list)
            {
                // 上
                CreateLine(node.x, node.y, LinePosition.Top);
                // 下
                CreateLine(node.x, node.y, LinePosition.Bottom);
                // 左
                CreateLine(node.x, node.y, LinePosition.Left);
                // 右
                CreateLine(node.x, node.y, LinePosition.Right);
            }
        }

        private void CreateLine(int i, int j, LinePosition linePosition)
        {
            var position = CalculatePosition(i, j);
            if(linePosition == LinePosition.Left)
                position.x = position.x - 0.5f * spriteWidth + 0.5f * lineWidth;
            else if (linePosition == LinePosition.Right)
                position.x = position.x + 0.5f * spriteWidth - 0.5f * lineWidth;
            else if (linePosition == LinePosition.Top)
                position.y = position.y + 0.5f * spriteHeight - 0.5f * lineWidth;
            else if (linePosition == LinePosition.Bottom)
                position.y = position.y - 0.5f * spriteHeight + 0.5f * lineWidth;

            var prefab = linePosition == LinePosition.Top || linePosition == LinePosition.Bottom
                ? HorizonLinePrefab
                : VerticalLinePrefab;
            var go = Instantiate(prefab, transform);
            go.SetActive(true);
            var t = go.GetComponent<RectTransform>();
            t.anchoredPosition = position;
        }
    }
}
