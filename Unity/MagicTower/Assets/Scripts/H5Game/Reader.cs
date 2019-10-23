using Gempoll.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        ///     地图高
        /// </summary>
        private int floorHeight;

        /// <summary>
        ///     地图宽
        /// </summary>
        private int floorWidth;

        private GameInfo gameInfo = new GameInfo();

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="projectPath"></param>
        public Reader(string projectPath)
        {
            this.projectPath = projectPath;
        }

        /// <summary>
        ///     使用到的楼层
        /// </summary>
        public List<string> FloorIds { get; private set; }

        /// <summary>
        ///     读取所有的楼层信息
        /// </summary>
        public void ReadFloors()
        {
            foreach (string floorId in FloorIds)
            {
                string floorJsPath = Path.Combine(projectPath, "floors/" + floorId + ".js");
                string json = ReadJsFile(floorJsPath);

                var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
                if (jsonObject == null)
                    return;

                floorWidth = jsonObject["width"].Value<int>();
                floorHeight = jsonObject["height"].Value<int>();

                var map = jsonObject["map"].Values<JArray>();
                foreach (var row in map)
                {
                    var list = row.Values<int>();
                    foreach (int objectId in list)
                        objectIdList.AddUnique(objectId);
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
        }
    }
}
