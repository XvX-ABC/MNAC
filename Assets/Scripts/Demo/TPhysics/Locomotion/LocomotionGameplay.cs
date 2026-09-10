using System;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// 基础三功能(移动/跳跃/冲刺)的玩法组装与输入驱动（Demo 运行层，不入核心库）。
    /// 每个 FixedUpdate 调用 <see cref="Tick"/>：先让 <see cref="Input"/> 采集自定义输入，
    /// 再把输入映射到模块（移动方向 → HorizontalLocomotion.DirectionVector；跳跃/冲刺 → 边沿启用对应模块）。
    /// 扩展点：<see cref="BeforeDrive"/>/<see cref="AfterDrive"/> 事件；派生本类覆写 <see cref="OnTick"/> 可注入自定义玩法。
    public class LocomotionGameplay
    {
        public InputState Input { get; }
        public HorizontalLocomotion Move { get; }
        public JumpLocomotion Jump { get; }
        public BoostingLocomotion Dash { get; }
        public LocomotionCore Core { get; }

        public IGroundDetector Ground => Core.Context.GroundDetector;

        public float JumpHoldSeconds = 0.35f;
        public float DashHoldSeconds = 0.4f;

        /// 每物理步、采集输入之后、映射模块之前触发（参数 = fixedDeltaTime）。
        public event Action<float> BeforeDrive;
        /// 每物理步、模块映射之后触发（参数 = fixedDeltaTime）。
        public event Action<float> AfterDrive;

        float _jumpTimer;
        float _dashTimer;
        bool _jumpPrev;
        bool _dashPrev;
        bool _movePausedByDash;

        /// <param name="input">自定义输入源；传 null 用默认 <see cref="InputState"/>（可传派生类以接入自有输入/玩法）。</param>
        public LocomotionGameplay(Rigidbody rb, IGroundDetector ground, InputState input = null, Vector3? fallbackForward = null)
        {
            Input = input ?? new InputState();
            var fwd = (fallbackForward ?? Vector3.forward).normalized;
            Move = new HorizontalLocomotion(6f, 14f, fwd) { Enabled = true };
            Jump = new JumpLocomotion(1.8f);
            Dash = new BoostingLocomotion(16f, 30f, fwd);
            Core = new LocomotionCore(new MNAC.TPhysics.World(), rb, ground);
            Core.SetModules(new ILocomotionModule[] { Move, Jump, Dash });
        }

        public bool IsJumping => _jumpTimer > 0f;
        public bool IsDashing => _dashTimer > 0f;

        /// 每物理步调用一次。
        public void Tick()
        {
            var dt = Time.fixedDeltaTime;
            Input.CollectInput(dt);
            BeforeDrive?.Invoke(dt);
            OnTick(dt);
            AfterDrive?.Invoke(dt);
        }

        /// 派生类可覆写以注入自定义玩法/额外模块驱动。
        protected virtual void OnTick(float fixedDeltaTime)
        {
            if (Input.HasMove)
            {
                Move.DirectionVector = Input.MoveDirection.normalized;
                Dash.DirectionVector = Move.DirectionVector;
            }

            bool jumpEdge = Input.Jump && !_jumpPrev;
            _jumpPrev = Input.Jump;
            bool dashEdge = Input.Dash && !_dashPrev;
            _dashPrev = Input.Dash;

            if (jumpEdge && Ground.IsGrounded && _jumpTimer <= 0f)
            {
                _jumpTimer = JumpHoldSeconds;
                Core.EnableModule(Jump);
            }
            if (_jumpTimer > 0f)
            {
                _jumpTimer -= fixedDeltaTime;
                if (_jumpTimer <= 0f)
                    Core.DisableModule(Jump);
            }

            if (dashEdge && _dashTimer <= 0f)
            {
                _dashTimer = DashHoldSeconds;
                // 冲刺期间停用常开移动，避免 Move 限速把冲刺压回 maxSpeed
                if (Move.Enabled)
                {
                    Core.DisableModule(Move);
                    _movePausedByDash = true;
                }
                Core.EnableModule(Dash);
            }
            if (_dashTimer > 0f)
            {
                _dashTimer -= fixedDeltaTime;
                if (_dashTimer <= 0f)
                {
                    Core.DisableModule(Dash);
                    if (_movePausedByDash)
                    {
                        Core.EnableModule(Move);
                        _movePausedByDash = false;
                    }
                }
            }
        }
    }
}
