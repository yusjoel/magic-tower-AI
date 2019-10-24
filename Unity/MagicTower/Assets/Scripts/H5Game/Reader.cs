using Gempoll.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Gempoll.H5Game
{
    /// <summary>
    ///     读取h5mota的游戏数据
    /// </summary>
    public class Reader
    {
        /// <summary>
        ///     使用到的所有物件编号列表
        /// </summary>
        private readonly List<int> objectIdList = new List<int>();

        /// <summary>
        ///     工程路径
        /// </summary>
        private readonly string projectPath;

        /// <summary>
        ///     初始楼层编号
        /// </summary>
        private string initialFloorId;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="projectPath"></param>
        public Reader(string projectPath)
        {
            this.projectPath = projectPath;
        }

        /// <summary>
        ///     游戏信息
        /// </summary>
        public GameInfo GameInfo { get; } = new GameInfo();

        /// <summary>
        ///     游戏名
        /// </summary>
        public string GameName { get; private set; }

        /// <summary>
        ///     使用到的楼层
        /// </summary>
        public List<string> FloorIds { get; private set; }

        public void Read()
        {
            ReadDataJs();
        }

        /// <summary>
        ///     读取所有的楼层信息
        /// </summary>
        public void ReadFloors()
        {
            for (int floorIndex = 0; floorIndex < FloorIds.Count; floorIndex++)
            {
                string floorId = FloorIds[floorIndex];
                string floorJsPath = Path.Combine(projectPath, "floors/" + floorId + ".js");
                string json = ReadJsFile(floorJsPath);

                var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
                if (jsonObject == null)
                    return;

                // 居然允许没有这两个值...
                //int rowCount = jsonObject["width"].Value<int>();
                //int columnCount = jsonObject["height"].Value<int>();

                var rows = jsonObject["map"].Values<JArray>().ToList();

                int rowCount = rows.Count;
                Debug.Assert(rowCount > 0);
                var firstRow = rows[0].ToList();
                int columnCount = firstRow.Count;
                Debug.Assert(columnCount > 0);

                if (GameInfo.Grid == null)
                {
                    GameInfo.RowCount = rowCount;
                    GameInfo.ColumnCount = columnCount;
                    GameInfo.Grid = new int[GameInfo.FloorCount, rowCount, columnCount];
                }
                else
                {
                    Debug.Assert(GameInfo.RowCount == rowCount);
                    Debug.Assert(GameInfo.ColumnCount == columnCount);
                }

                int rowIndex = 0;
                foreach (var row in rows)
                {
                    var list = row.Values<int>();
                    int columnIndex = 0;
                    foreach (int objectId in list)
                    {
                        GameInfo.Grid[floorIndex, rowIndex, columnIndex] = objectId;
                        objectIdList.AddUnique(objectId);
                        columnIndex++;
                    }
                    rowIndex++;
                }
            }
        }

        /// <summary>
        ///     读取Js文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ReadJsFile(string path)
        {
            using (var streamReader = new StreamReader(File.OpenRead(path)))
            {
                streamReader.ReadLine();
                string json = streamReader.ReadToEnd();
                return json;
            }
        }

        /// <summary>
        ///     读取工程数据
        /// </summary>
        public void ReadDataJs()
        {
            string dataJsPath = Path.Combine(projectPath, "data.js");
            string json = ReadJsFile(dataJsPath);
            var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
            if (jsonObject == null)
                return;

            var main = jsonObject["main"].Value<JObject>();
            FloorIds = main["floorIds"].Values<string>().ToList();

            GameInfo.FloorCount = FloorIds.Count;

            var firstData = jsonObject["firstData"].Value<JObject>();
            GameName = firstData["name"].Value<string>();

            initialFloorId = firstData["floorId"].Value<string>();

            var heroObject = firstData["hero"].Value<JObject>();
            int hitPoint = heroObject["hp"].Value<int>();
            int attack = heroObject["atk"].Value<int>();
            int defense = heroObject["def"].Value<int>();
            int magicDefense = heroObject["mdef"].Value<int>();
            int money = heroObject["money"].Value<int>();
            var items = heroObject["items"].Value<JObject>();
            var keys = items["keys"].Value<JObject>();
            int yellowKeyCount = keys["yellowKey"].Value<int>();
            int blueKeyCount = keys["blueKey"].Value<int>();
            int redKeyCount = keys["redKey"].Value<int>();

            var location = heroObject["loc"].Value<JObject>();
            int heroPositionX = location["x"].Value<int>();
            int heroPositionY = location["y"].Value<int>();
            var hero = new Hero(hitPoint, attack, defense, magicDefense, money, yellowKeyCount, blueKeyCount,
                redKeyCount, 0);

            GameInfo.Hero = hero;
            GameInfo.HeroFloor = FloorIds.IndexOf(initialFloorId);
            GameInfo.HeroPositionX = heroPositionX;
            GameInfo.HeroPositionY = heroPositionY;

            var values = jsonObject["values"].Value<JObject>();
            GameInfo.AttackOfRedJewel = values["redJewel"].Value<int>();
            GameInfo.DefenseOfBlueJewel = values["blueJewel"].Value<int>();
            GameInfo.MagicDefenseOfGreenJewel = values["greenJewel"].Value<int>();
            GameInfo.HitPointOfRedPotion = values["redPotion"].Value<int>();
            GameInfo.HitPointOfBluePotion = values["bluePotion"].Value<int>();
            GameInfo.HitPointOfYellowPotion = values["yellowPotion"].Value<int>();
            GameInfo.HitPointOfGreenPotion = values["greenPotion"].Value<int>();
        }
    }
}
