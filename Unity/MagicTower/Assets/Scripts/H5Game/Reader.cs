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
        ///     工程路径
        /// </summary>
        private readonly string projectPath;

        private List<string> floorIds;

        public List<string> FloorIds
        {
            get { return floorIds; }
        }

        public Reader(string projectPath)
        {
            this.projectPath = projectPath;
        }

        public void ReadDataJs()
        {
            string dataJsPath = Path.Combine(projectPath, "data.js");
            using (var streamReader = new StreamReader(File.OpenRead(dataJsPath)))
            {
                streamReader.ReadLine();
                string json = streamReader.ReadToEnd();
                var jsonObject = JsonConvert.DeserializeObject(json) as JObject;
                if (jsonObject == null)
                    return;

                var main = jsonObject["main"].Value<JObject>();
                floorIds = main["floorIds"].Values<string>().ToList();
            }
        }

    }
}
