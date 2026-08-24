using System.IO;
using UnityEngine;

namespace ConfigFramework
{
    /// <summary>默认加载来源：从 StreamingAssets 读取内置配置。</summary>
    public class StreamingAssetsConfigSource : IConfigSource
    {
        /// <summary>配置子目录名。初始化时由 ConfigSettings.BuiltinDir 覆盖。</summary>
        public string SubDir = "Config";

        public byte[] Load(string tableName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, SubDir, tableName + ".bytes");
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public string ReadManifest()
        {
            string path = Path.Combine(Application.streamingAssetsPath, SubDir, "manifest.json");
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }
    }
}
