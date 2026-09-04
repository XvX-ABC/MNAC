using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    /// 一块"支撑地面"的聚合数据（一个碰撞体 = 一条记录）。
    /// 仅统计支撑类接触：接触法线方向在 <see cref="EnvironmentSettings.MaxSlope"/> 内且大致指向 up 一侧，
    /// 墙/天花板/太陡接触不进入 Normal，故列表内每项都代表可站立表面。
    /// 不持有 Unity 接触数组，只存聚合结果，避免每帧分配。
    public readonly struct Ground
    {
        /// 支撑接触的碰撞体所属 GameObject（同旧版 key，用于去重）。
        public GameObject Obj { get; }
        /// 实际提供支撑的碰撞体。
        public Collider Collider { get; }
        /// 支撑接触法线的归一化平均。
        public Vector3 Normal { get; }
        /// 命中支撑判据的接触点数量。
        public int SupportCount { get; }
        /// 支撑接触点的平均位置。
        public Vector3 ContactPoint { get; }
        /// 该地面是否仍处于"被支撑"状态（无支撑接触即为失效）。
        public bool IsValid => SupportCount > 0;

        internal Ground(GameObject obj, Collider collider, Vector3 normalSum, Vector3 pointSum, int count)
        {
            Obj = obj;
            Collider = collider;
            Normal = count > 0 ? (normalSum / count).normalized : Vector3.zero;
            ContactPoint = count > 0 ? pointSum / count : Vector3.zero;
            SupportCount = count;
        }

        /// 从碰撞回调里筛选支撑接触并生成 <see cref="Ground"/>。
        /// <param name="maxSlopeCos">允许坡度的余弦值（调用方用 <see cref="MaxSlopeCos(float)"/> 计算）。</param>
        /// <returns>找到至少一个支撑接触才返回 true。</returns>
        internal static bool TryCreate(Collision collision, Vector3 worldUp, float maxSlopeCos, out Ground ground)
        {
            ground = default;
            if (collision == null)
                return false;

            var normalSum = Vector3.zero;
            var pointSum = Vector3.zero;
            int count = 0;
            var contacts = collision.contacts;
            for (int i = 0; i < contacts.Length; i++)
            {
                var point = contacts[i];
                if (Vector3.Dot(point.normal, worldUp) < maxSlopeCos)
                    continue;
                normalSum += point.normal;
                pointSum += point.point;
                count++;
            }
            if (count == 0)
                return false;

            ground = new Ground(collision.gameObject, collision.collider, normalSum, pointSum, count);
            return true;
        }

        /// 角度(度) → 支撑判定用的余弦阈值。
        public static float MaxSlopeCos(float maxSlopeDegrees) => Mathf.Cos(maxSlopeDegrees * Mathf.Deg2Rad);
    }
}
