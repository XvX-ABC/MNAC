using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigFramework
{
    /// <summary>
    /// 配置版本清单：版本号 + 每张表的 hash，用于热更差异对比。
    /// 序列化用 JsonUtility（与 MNAC 现有 JSON 配置一致）。
    /// </summary>
    [Serializable]
    public class ConfigManifest
    {
        [Serializable]
        public class Entry
        {
            public string Table;   // 表名
            public string Hash;    // 该表 .bytes 的 hash
        }

        public string Version;
        public List<Entry> Tables = new List<Entry>();

        public string ToJson() => JsonUtility.ToJson(this, true);

        public static ConfigManifest FromJson(string json) => JsonUtility.FromJson<ConfigManifest>(json);

        /// <summary>是否已包含指定表的指定 hash（未变化）。</summary>
        public bool Has(string table, string hash)
        {
            for (int i = 0; i < Tables.Count; i++)
            {
                if (Tables[i].Table == table && Tables[i].Hash == hash) return true;
            }
            return false;
        }
    }
}
