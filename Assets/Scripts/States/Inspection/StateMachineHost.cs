using UnityEngine;

namespace MNAC.States
{
    /// <summary>
    /// 最小状态机宿主：负责每帧 Tick，并给 Inspector 提供只读快照。
    /// 具体项目按自己的 Context/状态类型实现 <see cref="Inspection"/> 与 <see cref="Tick"/>。
    /// </summary>
    public abstract class StateMachineHost : MonoBehaviour
    {
        /// <summary>供 Inspector 绘制转移矩阵的只读视图，通常是状态机实例本身。</summary>
        public abstract IStateMachineInspection Inspection { get; }

        /// <summary>驱动器：一般直接调用 <c>machine.Update(deltaTime)</c>。</summary>
        protected abstract void Tick(float deltaTime);

        protected virtual void Update()
        {
            if (!isActiveAndEnabled)
                return;
            Tick(Time.deltaTime);
        }
    }
}
