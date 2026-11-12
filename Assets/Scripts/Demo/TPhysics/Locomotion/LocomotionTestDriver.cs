using System.Collections.Generic;
using MNAC.Interaction;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// <summary>
    /// A 方案运动试验场的驱动层：
    /// - WASD / Space / Shift 驱动移动、跳跃、冲刺
    /// - 数字键 1-8 热切换模块预设
    /// - Ctrl 急停，Q/E 切换/清除转向目标
    /// - 统一重置刚体、地面检测和统计峰值
    ///
    /// 不修改任何核心运动模块；只负责模块装配、输入映射和指标观测。
    /// 执行顺序设为 -100，保证输入在 LocomotionVisualRunner.FixedUpdate 之前写入模块。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class LocomotionTestDriver : MonoBehaviour
    {
        public enum Preset
        {
            Move,
            Dash,
            Jump,
            Rotate,
            Stop,
            Drag,
            Posture,
            Full,
        }

        public static readonly string[] PresetLabels =
        {
            "Move",
            "Dash",
            "Jump",
            "Rotate",
            "Stop",
            "Drag",
            "Posture",
            "Full",
        };

        [Header("场景引用")]
        public LocomotionVisualRunner runner;
        public Rigidbody body;
        public Camera viewCamera;
        public TransformPositionTarget[] rotationTargets;

        [Header("数值")]
        [Min(0f)] public float moveMaxSpeed = 6f;
        [Min(0f)] public float moveAcceleration = 14f;
        [Min(0f)] public float dashMaxSpeed = 16f;
        [Min(0f)] public float dashAcceleration = 30f;
        [Min(0f)] public float jumpHeight = 1.8f;
        [Min(0f)] public float jumpHoldSeconds = 0.35f;
        [Min(0f)] public float dashHoldSeconds = 0.4f;
        [Min(1)] public int postureSamples = 8;
        [Min(0f)] public float dragDuration = 1.5f;
        public Vector2 dragRange = new Vector2(0.2f, 3f);

        readonly List<ILocomotionModule> _activeModules = new List<ILocomotionModule>();

        VerticalPostureEvaluator _posture;
        HorizontalLocomotion _move;
        JumpLocomotion _jump;
        BoostingLocomotion _dash;
        RotationByMouseOrTargetLocomotion _rotation;
        StopLocomotion _stop;
        MutativeDrag _drag;

        Vector3 _spawnPosition;
        Quaternion _spawnRotation;
        Vector3 _inputDirection;
        Vector3 _brakeStart;
        Vector3 _startPosition;

        bool _ready;
        bool _hasMove;
        bool _movePausedByDash;
        bool _braking;
        float _jumpTimer;
        float _dashTimer;
        bool _jumpRequested;
        bool _dashRequested;
        bool _resetRequested;
        bool _targetCycleRequested;
        bool _targetClearRequested;
        int _presetRequested = -1;
        int _targetIndex = -1;
        IPositionTarget _target;

        public Preset CurrentPreset { get; private set; }
        public string CurrentPresetName => PresetLabels[(int)CurrentPreset];
        public IReadOnlyList<ILocomotionModule> ActiveModules => _activeModules;
        public string TargetLabel { get; private set; } = "鼠标（无目标）";

        public bool IsGrounded => runner != null && runner.GroundDetector != null && runner.GroundDetector.IsGrounded;
        public Vector3 GroundNormal => runner != null && runner.GroundDetector != null
            ? runner.GroundDetector.GroundsNormal
            : Vector3.zero;
        public float GroundAngle => IsGrounded ? Vector3.Angle(WorldUp, GroundNormal) : -1f;

        public Vector3 WorldUp => runner != null && runner.Core != null
            ? runner.Core.Context.World.Up
            : Vector3.up;

        public VerticalPosture Posture
        {
            get
            {
                if (runner == null || runner.Core == null)
                    return VerticalPosture.Holding;
                return runner.Core.Context.VerticalPosture;
            }
        }

        public float Speed { get; private set; }
        public float HorizontalSpeed { get; private set; }
        public float VerticalSpeed { get; private set; }
        public float PeakHorizontalSpeed { get; private set; }
        public float JumpApex { get; private set; }
        public float DistanceFromSpawn { get; private set; }
        public float TimeInRun { get; private set; }
        public float TimeTo10m { get; private set; } = -1f;
        public float TimeTo20m { get; private set; } = -1f;
        public float LastStopDistance { get; private set; }
        public float CurrentDrag => body != null ? body.drag : 0f;

        void Awake()
        {
            if (runner == null)
                runner = GetComponent<LocomotionVisualRunner>();
            if (runner == null)
                runner = FindObjectOfType<LocomotionVisualRunner>();
            if (body == null && runner != null)
                body = runner.GetComponent<Rigidbody>();
            if (viewCamera == null)
                viewCamera = Camera.main;
            if (rotationTargets == null || rotationTargets.Length == 0)
                rotationTargets = FindObjectsOfType<TransformPositionTarget>();
        }

        void Start()
        {
            if (runner == null || body == null || runner.Core == null)
            {
                Debug.LogError("[LocomotionTestDriver] 缺少 LocomotionVisualRunner / Rigidbody / LocomotionCore，测试驱动已禁用。", this);
                enabled = false;
                return;
            }

            _spawnPosition = body.position;
            _spawnRotation = body.rotation;
            _ready = true;
            ApplyPreset((int)Preset.Move);
        }

        void Update()
        {
            if (!_ready)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                _resetRequested = true;

            for (int i = 0; i < PresetLabels.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    _presetRequested = i;
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _targetCycleRequested = true;
            if (Input.GetKeyDown(KeyCode.E))
                _targetClearRequested = true;

            if (Input.GetKeyDown(KeyCode.Space))
                _jumpRequested = true;
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                _dashRequested = true;
        }

        void FixedUpdate()
        {
            if (!_ready)
                return;

            if (_resetRequested)
            {
                _resetRequested = false;
                ApplyPreset((int)CurrentPreset);
                return;
            }

            if (_presetRequested >= 0)
            {
                int requested = _presetRequested;
                _presetRequested = -1;
                ApplyPreset(requested);
                return;
            }

            if (_targetCycleRequested)
            {
                _targetCycleRequested = false;
                CycleTarget();
            }

            if (_targetClearRequested)
            {
                _targetClearRequested = false;
                ClearTarget();
            }

            UpdateRotationInput();
            UpdateMoveDirection();
            HandleBrake();
            HandleJump();
            HandleDash();
            UpdateMetrics();
        }

        void ApplyPreset(int index)
        {
            if (runner == null || runner.Core == null)
                return;

            CurrentPreset = (Preset)Mathf.Clamp(index, 0, PresetLabels.Length - 1);
            BuildModules();
            ResetRun();
            runner.Core.SetModules(_activeModules);
        }

        void BuildModules()
        {
            _activeModules.Clear();
            _move = null;
            _jump = null;
            _dash = null;
            _rotation = null;
            _stop = null;
            _drag = null;

            _posture = new VerticalPostureEvaluator(postureSamples, -100) { Enabled = true };
            _activeModules.Add(_posture);

            if (CurrentPreset != Preset.Posture)
            {
                _move = new HorizontalLocomotion(moveMaxSpeed, moveAcceleration) { Enabled = true };
                _activeModules.Add(_move);
            }

            switch (CurrentPreset)
            {
                case Preset.Move:
                    break;

                case Preset.Dash:
                    _dash = new BoostingLocomotion(dashMaxSpeed, dashAcceleration) { Enabled = false };
                    _activeModules.Add(_dash);
                    break;

                case Preset.Jump:
                    _jump = new JumpLocomotion(jumpHeight) { Enabled = false };
                    _activeModules.Add(_jump);
                    break;

                case Preset.Rotate:
                    _rotation = new RotationByMouseOrTargetLocomotion(viewCamera) { Enabled = true };
                    _activeModules.Add(_rotation);
                    break;

                case Preset.Stop:
                    _stop = new StopLocomotion { Enabled = false };
                    _activeModules.Add(_stop);
                    break;

                case Preset.Drag:
                    _drag = new MutativeDrag(dragDuration, dragRange) { Enabled = true };
                    _activeModules.Add(_drag);
                    break;

                case Preset.Posture:
                    _move = new HorizontalLocomotion(moveMaxSpeed, moveAcceleration) { Enabled = true };
                    _activeModules.Add(_move);
                    _jump = new JumpLocomotion(jumpHeight) { Enabled = false };
                    _activeModules.Add(_jump);
                    break;

                case Preset.Full:
                    _jump = new JumpLocomotion(jumpHeight) { Enabled = false };
                    _activeModules.Add(_jump);
                    _dash = new BoostingLocomotion(dashMaxSpeed, dashAcceleration) { Enabled = false };
                    _activeModules.Add(_dash);
                    _rotation = new RotationByMouseOrTargetLocomotion(viewCamera) { Enabled = true };
                    _activeModules.Add(_rotation);
                    _drag = new MutativeDrag(dragDuration, dragRange) { Enabled = true };
                    _activeModules.Add(_drag);
                    _stop = new StopLocomotion { Enabled = false };
                    _activeModules.Add(_stop);
                    break;
            }
        }

        void ResetRun()
        {
            if (body == null)
                return;

            _jumpTimer = 0f;
            _dashTimer = 0f;
            _movePausedByDash = false;
            _braking = false;
            _jumpRequested = false;
            _dashRequested = false;
            _hasMove = false;
            _inputDirection = Vector3.zero;

            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.drag = 0f;
            body.angularDrag = 0.05f;
            body.position = _spawnPosition;
            body.rotation = _spawnRotation;
            body.WakeUp();

            if (runner != null && runner.GroundDetector is GroundDetector detector)
            {
                detector.Enabled = true;
                detector.Reset();
                detector.Position = _spawnPosition;
            }

            Physics.SyncTransforms();

            _startPosition = _spawnPosition;
            _brakeStart = _spawnPosition;
            TimeInRun = 0f;
            TimeTo10m = -1f;
            TimeTo20m = -1f;
            LastStopDistance = 0f;
            DistanceFromSpawn = 0f;
            PeakHorizontalSpeed = 0f;
            JumpApex = 0f;
            Speed = 0f;
            HorizontalSpeed = 0f;
            VerticalSpeed = 0f;
        }

        void UpdateRotationInput()
        {
            if (_rotation == null)
                return;

            _rotation.Camera = viewCamera;
            _rotation.MouseScreenPosition = Input.mousePosition;
            _rotation.Target = _target;
        }

        void UpdateMoveDirection()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _inputDirection = new Vector3(h, 0f, v);
            if (_inputDirection.sqrMagnitude > 1f)
                _inputDirection.Normalize();
            _hasMove = _inputDirection.sqrMagnitude > 1e-6f;

            if (_move != null)
                _move.DirectionVector = _inputDirection;

            if (_dash != null)
            {
                var dashDirection = _hasMove
                    ? _inputDirection
                    : Vector3.ProjectOnPlane(body.transform.forward, WorldUp).normalized;
                _dash.DirectionVector = dashDirection;
            }
        }

        void HandleBrake()
        {
            if (_stop == null)
                return;

            bool brake = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (CurrentPreset == Preset.Stop && !_hasMove)
                brake = true;

            if (brake && !_braking)
            {
                _braking = true;
                _brakeStart = body.position;
            }
            else if (!brake && _braking)
            {
                _braking = false;
                LastStopDistance = HorizontalDistance(body.position, _brakeStart);
            }

            SetModuleEnabled(_stop, brake);
        }

        void HandleJump()
        {
            if (_jumpRequested)
            {
                _jumpRequested = false;
                if (_jump != null && IsGrounded && _jumpTimer <= 0f)
                {
                    _jumpTimer = jumpHoldSeconds;
                    SetModuleEnabled(_jump, true);
                }
            }

            if (_jumpTimer > 0f)
            {
                _jumpTimer -= Time.fixedDeltaTime;
                if (_jumpTimer <= 0f)
                    SetModuleEnabled(_jump, false);
            }
        }

        void HandleDash()
        {
            if (_dashRequested)
            {
                _dashRequested = false;
                if (_dash != null && _dashTimer <= 0f)
                {
                    _dashTimer = dashHoldSeconds;
                    _movePausedByDash = _move != null && _move.Enabled;
                    if (_movePausedByDash)
                        SetModuleEnabled(_move, false);
                    SetModuleEnabled(_dash, true);
                }
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= Time.fixedDeltaTime;
                if (_dashTimer <= 0f)
                {
                    SetModuleEnabled(_dash, false);
                    if (_movePausedByDash)
                    {
                        SetModuleEnabled(_move, true);
                        _movePausedByDash = false;
                    }
                }
            }
        }

        void UpdateMetrics()
        {
            TimeInRun += Time.fixedDeltaTime;

            var velocity = body.velocity;
            var up = WorldUp;
            Speed = velocity.magnitude;
            HorizontalSpeed = Vector3.ProjectOnPlane(velocity, up).magnitude;
            VerticalSpeed = Vector3.Dot(velocity, up);
            PeakHorizontalSpeed = Mathf.Max(PeakHorizontalSpeed, HorizontalSpeed);

            float height = Vector3.Dot(body.position - _spawnPosition, up);
            JumpApex = Mathf.Max(JumpApex, height);

            DistanceFromSpawn = HorizontalDistance(body.position, _startPosition);
            if (TimeTo10m < 0f && DistanceFromSpawn >= 10f)
                TimeTo10m = TimeInRun;
            if (TimeTo20m < 0f && DistanceFromSpawn >= 20f)
                TimeTo20m = TimeInRun;
        }

        void SetModuleEnabled(ILocomotionModule module, bool enabled)
        {
            if (module == null || runner == null || runner.Core == null)
                return;
            if (module.Enabled == enabled)
                return;

            if (enabled)
                runner.Core.EnableModule(module);
            else
                runner.Core.DisableModule(module);
        }

        void CycleTarget()
        {
            if (rotationTargets == null || rotationTargets.Length == 0)
            {
                ClearTarget();
                return;
            }

            for (int i = 0; i < rotationTargets.Length; i++)
            {
                _targetIndex = (_targetIndex + 1) % rotationTargets.Length;
                if (rotationTargets[_targetIndex] != null)
                    break;
            }

            _target = rotationTargets[_targetIndex];
            TargetLabel = rotationTargets[_targetIndex] != null
                ? rotationTargets[_targetIndex].name
                : "鼠标（无目标）";
        }

        void ClearTarget()
        {
            _target = null;
            _targetIndex = -1;
            TargetLabel = "鼠标（无目标）";
        }

        float HorizontalDistance(Vector3 a, Vector3 b)
        {
            return Vector3.ProjectOnPlane(a - b, WorldUp).magnitude;
        }

        void OnDrawGizmosSelected()
        {
            if (body == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(body.position, body.position + body.velocity);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(body.position, body.position + _inputDirection * 2f);

            if (IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(body.position, body.position + GroundNormal * 2f);
            }
        }
    }
}
