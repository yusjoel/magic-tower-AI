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
        ///     道具预制体
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        ///     怪物预制体
        /// </summary>
        public GameObject MonsterPrefab;

        public GameObject RoadPrefab;

        public GameObject ShopPrefab;

        public GameObject StairDownPrefab;

        public GameObject StairUpPrefab;

        public GameObject WallPrefab;

        public GameObject HeroPrefab;

        /// <summary>
        ///     当前楼层
        /// </summary>
        private int floor;

        private Graph graph;

        private void Draw()
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
                    t.anchoredPosition = new Vector2(i * 32, j * 32);
                }
            }

            var heroNode = graph.heroNode;
            if (heroNode.f == floor)
            {
                var go = Instantiate(HeroPrefab, transform);
                go.SetActive(true);
                var t = go.GetComponent<RectTransform>();
                t.anchoredPosition = new Vector2(heroNode.x * 32, heroNode.y * 32);
            }
        }

        private void CreateRoad(int i, int j)
        {
            var go = Instantiate(RoadPrefab, transform);
            go.SetActive(true);
            var t = go.GetComponent<RectTransform>();
            t.anchoredPosition = new Vector2(i * 32, j * 32);
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
            ReadGraph("map1", false);

            floor = 0;
            Draw();
        }

        private void Update()
        {
        }
    }
}
