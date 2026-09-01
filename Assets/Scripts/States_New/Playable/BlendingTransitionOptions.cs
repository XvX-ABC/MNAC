using System;
using UnityEngine;

namespace MNAC.StatesNew
{
#pragma warning disable 0649 // 序列化字段由 Unity 赋值，编译期视为未使用
    /// <summary>混合过渡的参数配置（可序列化，供 Inspector/配置资源使用）。</summary>
    [Serializable]
    public class BlendingTransitionOptions
    {
        [SerializeField] float duration;
        [SerializeField] float offset;
        [SerializeField] bool enableFixedExit;
        [SerializeField] float fixedExitTime;
        [SerializeField] InterruptionMode interruptionMode = BlendingTransition<object>.INTERRUPTION_MODE_DEFAULT;

        /// <summary>过渡时长（秒）。</summary>
        public float Duration { get => duration; set => duration = value; }

        /// <summary>目标 Timeline 前进偏移量（跳过开头段）。</summary>
        public float Offset { get => offset; set => offset = value; }

        /// <summary>源状态播到该归一化时刻时强制触发转移；-1 表示无效（不启用）。</summary>
        public float FixedExitTime
        {
            get => enableFixedExit ? fixedExitTime : BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE;
            set => fixedExitTime = value;
        }

        public InterruptionMode InterruptionMode { get => interruptionMode; set => interruptionMode = value; }
    }
}
