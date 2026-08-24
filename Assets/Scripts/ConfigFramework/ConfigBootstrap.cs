#if LUBAN_ENABLED
using ConfigFramework;

/// <summary>
/// 把 Luban 生成的表类注册进 ConfigSystem（唯一知道 Luban API 的地方）。
/// 说明：
///  1. 需先在 Player Settings / asmdef 定义 LUBAN_ENABLED 宏；
///  2. 需引入 Luban 生成代码（code_protobuf3 生成的 TbXxx 类）与 Google.Protobuf；
///  3. 反序列化方法名以 Luban 实际生成代码为准（下面用 Deserialize 作示意）。
/// 满足以上条件后本文件才会参与编译。
/// </summary>
public static class ConfigBootstrap
{
    public static void Init()
    {
        ConfigSystem.Initialize(new ConfigSettings
        {
            RemoteManifestUrl = null,   // 填远程 manifest 地址以启用热更
        });

        ConfigSystem.RegisterTable<TbWeapon>("weapon", TbWeapon.Deserialize);
        ConfigSystem.RegisterTable<TbEnemy>("enemy", TbEnemy.Deserialize);
        ConfigSystem.RegisterTable<TbWave>("wave", TbWave.Deserialize);

        ConfigSystem.Load();
    }
}
#endif
