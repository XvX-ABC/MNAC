using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ConfigFramework
{
    /// <summary>
    /// 配置热更：对比 manifest → 下载差异表 → 原子替换 → 触发重载。
    /// </summary>
    public static class ConfigHotUpdate
    {
        /// <summary>解析热更目录（用户未指定则用默认 persistentDataPath/ConfigPatch）。</summary>
        public static string ResolvePatchDir(ConfigSettings settings)
        {
            return string.IsNullOrEmpty(settings.PatchDir)
                ? Path.Combine(Application.persistentDataPath, "ConfigPatch")
                : settings.PatchDir;
        }

        /// <summary>热更目录命中某表则返回其字节，否则 null（回退内置）。</summary>
        public static byte[] TryLoadPatch(ConfigSettings settings, string tableName)
        {
            string file = Path.Combine(ResolvePatchDir(settings), tableName + ".bytes");
            return File.Exists(file) ? File.ReadAllBytes(file) : null;
        }

        /// <summary>热更流程（协程）：拉远程 manifest → 下载差异表 → 原子替换 → 重载。</summary>
        public static IEnumerator CheckUpdateAsync(ConfigSettings settings)
        {
            if (string.IsNullOrEmpty(settings.RemoteManifestUrl)) yield break;

            // 1. 拉取远程 manifest
            var remote = default(ConfigManifest);
            using (var req = UnityWebRequest.Get(settings.RemoteManifestUrl))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success) yield break;
                remote = ConfigManifest.FromJson(req.downloadHandler.text);
            }
            if (remote == null || remote.Tables == null) yield break;

            // 2. 对比本地，找出差异表
            string localJson = settings.Source != null ? settings.Source.ReadManifest() : null;
            var local = string.IsNullOrEmpty(localJson) ? null : ConfigManifest.FromJson(localJson);

            string baseUrl = settings.RemoteManifestUrl.Substring(0, settings.RemoteManifestUrl.LastIndexOf('/') + 1);

            foreach (var e in remote.Tables)
            {
                if (local != null && local.Has(e.Table, e.Hash)) continue;

                // 3. 下载差异表（约定：manifest 同目录下 <Table>.bytes）
                byte[] data = null;
                using (var dl = UnityWebRequest.Get(baseUrl + e.Table + ".bytes"))
                {
                    yield return dl.SendWebRequest();
                    if (dl.result != UnityWebRequest.Result.Success) continue;
                    data = dl.downloadHandler.data;
                }

                // 4. 原子替换
                SaveAtomic(settings, e.Table, data);
            }

            // 5. 保存新 manifest，触发重载
            Directory.CreateDirectory(ResolvePatchDir(settings));
            File.WriteAllText(Path.Combine(ResolvePatchDir(settings), "manifest.json"), remote.ToJson());
            ConfigSystem.Reload();
        }

        /// <summary>先写临时文件再 rename，防半写/脏文件。</summary>
        static void SaveAtomic(ConfigSettings settings, string tableName, byte[] data)
        {
            string dir = ResolvePatchDir(settings);
            Directory.CreateDirectory(dir);
            string tmp = Path.Combine(dir, tableName + ".bytes.tmp");
            string dest = Path.Combine(dir, tableName + ".bytes");
            File.WriteAllBytes(tmp, data);
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(tmp, dest);   // 同目录下 rename，具备原子性
        }
    }
}
