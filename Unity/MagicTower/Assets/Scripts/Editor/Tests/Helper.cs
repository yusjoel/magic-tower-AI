using NUnit.Framework;
using System.IO;

namespace Gempoll.Editor.Tests
{
    public static class Helper
    {
        /// <summary>
        ///     获取StreamingAssets目录下文件的路径, 保证在Unity3D和VS环境下都能正确获取
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetStreamingAssetPath(string fileName)
        {
            // in Visual Studio
            // <unity-project>\Temp\bin\Debug\
            // in Unity Editor
            // <unity-project>\Library\ScriptAssemblies
            string basePath = Path.GetDirectoryName(typeof(GameInfoTest).Assembly.Location);
            Assert.IsNotNull(basePath);

            if (basePath.EndsWith("ScriptAssemblies"))
                basePath = $"{basePath}/../..";
            else
                basePath = $"{basePath}/../../..";

            // 尽量不要使用UnityEngine下的方法, 如Application.streamingAssetsPath
            // 在VisualStudio中会报错: ECall 方法必须打包到系统模块中
            string mapPath = $"{basePath}/Assets/StreamingAssets/{fileName}";
            return mapPath;
        }
    }
}
