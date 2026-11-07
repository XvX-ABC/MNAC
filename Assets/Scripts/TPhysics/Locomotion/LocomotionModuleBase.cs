using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 运动模块基类：生命周期追踪由 LocomotionCore 内部的私有状态机负责，
    /// 模块自身不保存状态，只暴露 Start/Update/End → On* 钩子的转发。
    /// <see cref="priority"/> 决定链中执行顺序（升序，同值保持装配顺序）。
    public abstract class LocomotionModuleBase : ILocomotionModule
    {
        [SerializeField] protected bool enabled;
        [SerializeField, Tooltip("执行顺序：数值小的先执行；同值保持装配列表顺序（运行时可修改，赋值即重排）")]
        protected int priority;

        protected LocomotionModuleBase(int priority = 0)
        {
            this.priority = priority;
        }

        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// 所属装配核心（由 <see cref="LocomotionCore.AddModule"/> 设置）。
        /// 模块同一时间只归属一个核心；不在核心中时不会触发重排。
        internal LocomotionCore Owner { get; set; }

        /// 执行顺序：数值小的先执行；同值保持装配列表顺序。
        /// 运行时可修改，赋值即通知所属核心重排；**Update 期间禁止修改**（忽略并告警）。
        public virtual int Priority
        {
            get => priority;
            set
            {
                if (priority == value)
                    return;
                if (Owner != null && Owner.IsUpdating)
                {
                    Debug.LogWarning($"[{GetType().Name}] 不能在 Update 期间修改 Priority（当前 {priority} → 请求 {value}），已忽略。");
                    return;
                }
                priority = value;
                Owner?.Reposition(this);
            }
        }

        // 生命周期钩子默认透传，模块只覆盖需要处理的那一个。
        public virtual Context OnStart(Context context) => context;
        public virtual Context OnUpdate(Context context) => context;
        public virtual Context OnEnd(Context context) => context;

        /// 调试绘制钩子：由宿主（如 <see cref="LocomotionVisualRunner"/>）每帧驱动，
        /// 模块可在其中用 Gizmos/Debug.DrawLine 画出自身的调试信息（方向、目标速度、时间轴进度等）。
        /// 默认空实现，不影响运行时行为。
        public virtual void OnDrawGizmos(Context context)
        {
        }

        public Context Start(Context context) => OnStart(context);
        public Context Update(Context context) => OnUpdate(context);
        public Context End(Context context) => OnEnd(context);
    }
}
