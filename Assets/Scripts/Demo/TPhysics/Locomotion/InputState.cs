using System;
using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// 输入抽象层（Demo 运行用，不入核心库）。
    /// 承载方向 + 跳跃/冲刺请求，并预留**自定义输入与玩法**的扩展空间：
    /// 派生本类可加入自有的输入字段/状态，并覆写 <see cref="CollectInput"/> 在每物理步把
    /// 外部输入系统（键鼠/手柄/网络等）写入 <see cref="MoveDirection"/>/<see cref="Jump"/>/<see cref="Dash"/>，
    /// 或维护派生类自己的玩法数据。也可覆写 <see cref="Reset"/> 一并清理自定义状态。
    [Serializable]
    public class InputState
    {
        [SerializeField] Vector3 _moveDirection;
        [SerializeField] bool _jump;
        [SerializeField] bool _dash;

        public InputState()
        {
        }

        public InputState(Vector3 moveDirection, bool jump = false, bool dash = false)
        {
            _moveDirection = moveDirection;
            _jump = jump;
            _dash = dash;
        }

        /// 世界系移动方向；zero 表示无移动输入。
        public Vector3 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        public bool Jump
        {
            get => _jump;
            set => _jump = value;
        }

        public bool Dash
        {
            get => _dash;
            set => _dash = value;
        }

        public bool HasMove => _moveDirection.sqrMagnitude > 1e-6f;

        /// 每物理步由驱动层先于映射逻辑调用一次。
        /// 默认空实现；派生类在此接入自定义输入/玩法（写 MoveDirection/Jump/Dash 或自有状态）。
        public virtual void CollectInput(float fixedDeltaTime)
        {
        }

        /// 清空输入。派生类可覆写以一并重置自定义状态。
        public virtual void Reset()
        {
            _moveDirection = Vector3.zero;
            _jump = false;
            _dash = false;
        }
    }
}
