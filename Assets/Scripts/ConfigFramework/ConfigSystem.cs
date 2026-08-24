using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigFramework
{
    /// <summary>
    /// 配置系统对外门面。业务方只与它交互。
    /// 生命周期：Initialize → RegisterTable（多次）→ Load；热更用 CheckUpdateAsync；重载后订阅 ConfigReloaded。
    /// </summary>
    public static class ConfigSystem
    {
        static ConfigSettings _settings;
        public static bool Initialized { get; private set; }

        // 表类型 -> 表名（定位文件） / 加载委托 / 已加载实例
        static readonly Dictionary<Type, string> _tableNames = new Dictionary<Type, string>();
        static readonly Dictionary<Type, Func<byte[], object>> _loaders = new Dictionary<Type, Func<byte[], object>>();
        static readonly Dictionary<Type, object> _tables = new Dictionary<Type, object>();

        // ---------- 初始化 ----------

        public static void Initialize(ConfigSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Source == null) settings.Source = new StreamingAssetsConfigSource();
            _settings = settings;
            if (_settings.Source is StreamingAssetsConfigSource sa)
                sa.SubDir = _settings.BuiltinDir;
            Initialized = true;
        }

        // ---------- 注册表（屏蔽 Luban 反序列化 API） ----------

        /// <summary>注册一张表：表类型、表名（用于定位文件）、字节→表 的反序列化委托。</summary>
        public static void RegisterTable<TTable>(string tableName, Func<byte[], TTable> deserializer)
            where TTable : class
        {
            if (deserializer == null) throw new ArgumentNullException(nameof(deserializer));
            _tableNames[typeof(TTable)] = tableName;
            _loaders[typeof(TTable)] = bytes => deserializer(bytes);
        }

        // ---------- 加载 / 重载 ----------

        public static void Load()
        {
            if (!Initialized) throw new InvalidOperationException("ConfigSystem 未初始化，请先调用 Initialize()");

            _tables.Clear();
            foreach (var kv in _loaders)
            {
                Type type = kv.Key;
                string tableName = _tableNames[type];

                // 热更目录优先，回退内置
                byte[] bytes = ConfigHotUpdate.TryLoadPatch(_settings, tableName)
                            ?? _settings.Source.Load(tableName);
                if (bytes == null)
                {
                    Debug.LogWarning($"[ConfigFramework] 缺少配置表 {tableName}");
                    continue;
                }

                try
                {
                    _tables[type] = kv.Value(bytes);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ConfigFramework] 表 {tableName} 反序列化失败: {ex}");
                }
            }

            if (_settings.AutoResolveRef) ResolveAllRefs();
            ConfigReloaded?.Invoke();
        }

        /// <summary>异步加载（当前为同步实现的包装，保留接口以兼容协程调用）。</summary>
        public static IEnumerator LoadAsync()
        {
            Load();
            yield break;
        }

        public static void Reload() => Load();

        // ---------- 数据访问 ----------

        public static TTable GetTable<TTable>() where TTable : class
        {
            return _tables.TryGetValue(typeof(TTable), out object t) ? (TTable)t : null;
        }

        public static bool TryGetTable<TTable>(out TTable table) where TTable : class
        {
            table = GetTable<TTable>();
            return table != null;
        }

        // ---------- 热更 ----------

        /// <summary>检查并应用热更（协程）。业务方在 MonoBehaviour 中 StartCoroutine 调用。</summary>
        public static IEnumerator CheckUpdateAsync()
        {
            if (!Initialized) throw new InvalidOperationException("ConfigSystem 未初始化");
            yield return ConfigHotUpdate.CheckUpdateAsync(_settings);
        }

        // ---------- 事件 ----------

        /// <summary>加载/重载完成后触发。业务方订阅并重读配置，避免持有旧引用。</summary>
        public static event Action ConfigReloaded;

        static void ResolveAllRefs()
        {
            foreach (var table in _tables.Values)
            {
                if (table is IResolvable r) r.ResolveRef();
            }
        }
    }

    /// <summary>
    /// 表类可选实现：用于表间引用（Luban ref）解析。
    /// Luban 生成的表类可通过薄适配实现该接口，具体 ref 解析调用以 Luban 生成代码为准。
    /// </summary>
    public interface IResolvable
    {
        void ResolveRef();
    }
}
