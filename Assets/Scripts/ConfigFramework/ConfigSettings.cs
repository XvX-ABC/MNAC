namespace ConfigFramework
{
    /// <summary>
    /// ConfigSystem 初始化参数。所有可替换项都通过这里注入，模块不硬编码路径。
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>内置配置的加载来源（默认 StreamingAssets）。可注入自定义实现（AssetBundle / Addressables）。</summary>
        public IConfigSource Source = new StreamingAssetsConfigSource();

        /// <summary>内置配置子目录名（相对 Source 根，如 "Config"）。</summary>
        public string BuiltinDir = "Config";

        /// <summary>热更目录绝对路径。留空则用默认 persistentDataPath/ConfigPatch。</summary>
        public string PatchDir = null;

        /// <summary>远程 manifest 地址。留空则禁用热更（仅用内置）。</summary>
        public string RemoteManifestUrl = null;

        /// <summary>是否在加载后自动解析表间引用（Luban ref）。</summary>
        public bool AutoResolveRef = true;
    }
}
