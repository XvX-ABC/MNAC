namespace ConfigFramework
{
    /// <summary>
    /// 内置配置的读取来源。业务方可用 AssetBundle / Addressables / 网络实现它注入。
    /// </summary>
    public interface IConfigSource
    {
        /// <summary>读取某张表的原始字节。找不到返回 null。</summary>
        byte[] Load(string tableName);

        /// <summary>读取内置 manifest。可返回 null（热更时用远程 manifest 兜底）。</summary>
        string ReadManifest();
    }
}
