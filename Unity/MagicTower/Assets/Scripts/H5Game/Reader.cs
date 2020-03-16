using Gempoll.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        ///     怪物的编号与名称映射表
        /// </summary>
        private readonly Dictionary<int, string> monsterIdNameMap = new Dictionary<int, string>();

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
            ReadFloors();
            ReadMapsJs();
            ReadEnemiesJs();
        }

        /// <summary>
        ///     读取物件信息
        /// </summary>
        private void ReadMapsJs()
        {
            string mapsJsPath = Path.Combine(projectPath, "maps.js");
            string json = ReadJsFile(mapsJsPath);
            var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
            if (jsonObject == null)
                return;

            foreach (var pair in jsonObject)
            {
                string key = pair.Key;
                int objectId;
                if (!int.TryParse(key, out objectId))
                    continue;

                if (!objectIdList.Contains(objectId))
                    continue;

                var info = pair.Value as JObject;
                if (info == null)
                    continue;

                string className = info["cls"].Value<string>();
                string objectName = info["id"].Value<string>();
                ValidateObjectDefinition(objectId, className, objectName);

                if (className.StartsWith("enemy")) monsterIdNameMap[objectId] = objectName;
            }
        }

        /// <summary>
        ///     验证物件的定义是否一致
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="className"></param>
        /// <param name="objectName"></param>
        private void ValidateObjectDefinition(int objectId, string className, string objectName)
        {
            // 这里仅验证, 以后有需要进行映射
            if (objectId == ObjectId.WALL)
                Debug.Assert(className == "terrains");
            else if (objectId == ObjectId.YELLOW_KEY)
                Debug.Assert(objectName == "yellowKey");
            else if (objectId == ObjectId.BLUE_KEY)
                Debug.Assert(objectName == "blueKey");
            else if (objectId == ObjectId.RED_KEY)
                Debug.Assert(objectName == "redKey");
            else if (objectId == ObjectId.GREEN_KEY)
                Debug.Assert(objectName == "greenKey");
            else if (objectId == ObjectId.RED_JEWEL)
                Debug.Assert(objectName == "redJewel");
            else if (objectId == ObjectId.BLUE_JEWEL)
                Debug.Assert(objectName == "blueJewel");
            else if (objectId == ObjectId.GREEN_JEWEL)
                Debug.Assert(objectName == "greenJewel");
            else if (objectId == ObjectId.RED_POTION)
                Debug.Assert(objectName == "redPotion");
            else if (objectId == ObjectId.BLUE_POTION)
                Debug.Assert(objectName == "bluePotion");
            else if (objectId == ObjectId.GREEN_POTION)
                Debug.Assert(objectName == "greenPotion");
            else if (objectId == ObjectId.YELLOW_POTION)
                Debug.Assert(objectName == "yellowPotion");
            else if (objectId == ObjectId.SWORD)
                Debug.Assert(objectName == "sword1");
            else if (objectId == ObjectId.SHIELD)
                Debug.Assert(objectName == "shield1");
            else if (objectId == ObjectId.DOOR_YELLOW)
                Debug.Assert(objectName == "yellowDoor");
            else if (objectId == ObjectId.DOOR_BLUE)
                Debug.Assert(objectName == "blueDoor");
            else if (objectId == ObjectId.DOOR_RED)
                Debug.Assert(objectName == "redDoor");
            else if (objectId == ObjectId.DOOR_GREEN)
                Debug.Assert(objectName == "greenDoor");
            else if (objectId == ObjectId.UPSTAIR)
                Debug.Assert(objectName == "upFloor");
            else if (objectId == ObjectId.DOWNSTAIR)
                Debug.Assert(objectName == "downFloor");
            else if (objectId > ObjectId.MONSTER_BOUND)
                Debug.Assert(className.StartsWith("enemy"));
            else
                Console.WriteLine($"未定义物件: {objectId} {className} {objectName}");
        }

        /// <summary>
        ///     读取怪物信息
        /// </summary>
        private void ReadEnemiesJs()
        {
            string enemiesJsPath = Path.Combine(projectPath, "enemys.js");
            string json = ReadJsFile(enemiesJsPath);
            var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
            if (jsonObject == null)
                return;

            foreach (var pair in monsterIdNameMap)
            {
                int monsterId = pair.Key;
                string monsterName = pair.Value;

                var monsterObject = jsonObject[monsterName] as JObject;
                if (monsterObject == null)
                    continue;

                int hitPoint = monsterObject["hp"].Value<int>();
                int attack = monsterObject["atk"].Value<int>();
                int defense = monsterObject["def"].Value<int>();
                int money = monsterObject["money"].Value<int>();

                var specialToken = monsterObject["special"];
                int special = 0;
                if (specialToken.Type == JTokenType.Integer)
                {
                    special = specialToken.Value<int>();
                }
                else if (specialToken.Type == JTokenType.Array)
                {
                    var specials = specialToken as JArray;
                    if (specials == null)
                        continue;

                    foreach (int value in specials.Values<int>())
                        special |= value;
                }

                if (special > 0) Console.WriteLine($"{monsterName} {monsterObject["name"]} 拥有特殊能力 {specialToken}");

                var monster = new Monster(monsterId, hitPoint, attack, defense, money, special);
                GameInfo.MonsterMap[monsterId] = monster;
            }
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
