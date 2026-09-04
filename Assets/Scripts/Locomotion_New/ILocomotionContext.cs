using UnityEngine;

namespace MNAC.Locomotion.Interfaces
{
    /// <summary>
    /// 运动系统上下文接口，解耦物理系统
    /// </summary>
    public interface ILocomotionContext
    {
        // 基础运动属性
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Velocity { get; set; }
        float Speed { get; }
        Vector3 Forward { get; }
        Vector3 Up { get; }
        Vector3 Right { get; }

        // 时间相关
        float DeltaTime { get; }
        float FixedDeltaTime { get; }
        int UpdateCount { get; }

        /// <summary>
        /// 从底层系统同步数据到上下文
        /// 读取当前的实际状态
        /// </summary>
        void Synchronize();

        /// <summary>
        /// 将上下文中的变化应用到底层系统
        /// 将运动变化实际应用到物理引擎或其他系统
        /// </summary>
        void Apply();
    }
}