using System;
using System.Collections.Generic;
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    /// 地面检测对外的最小数据视图（Locomotion 层只读这些）。
    /// 语义约定：Grounds 只含"可支撑表面"，故 Grounds.Count &gt; 0 即等于 IsGrounded。
    public interface IGroundDetector
    {
        bool Enabled { get; set; }
        /// 是否存在可支撑地面。
        bool IsGrounded { get; }
        /// 支撑面法线（按支撑接触数加权平均）；无支撑时为 Vector3.zero（调用方自行回退 world.Up）。
        Vector3 GroundsNormal { get; }
        /// 当前支撑地面列表（只读）。
        IReadOnlyList<Ground> Grounds { get; }
        /// 空中到地面的距离：支撑时 0；射线范围内命中=距离；范围内未命中=Mathf.Infinity；关闭/位置无效/未测=-1。
        float GroundDistance { get; }
        /// 落地/离地切换事件（参数 true = 落地）。
        event Action<bool> GroundingChanged;
    }
}
