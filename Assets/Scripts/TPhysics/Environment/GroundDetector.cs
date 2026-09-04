using System;
using System.Collections.Generic;
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    /// 地面检测器（纯逻辑，无 MonoBehaviour）。
    /// 核心语义：只有满足坡度判据的"支撑类接触"才进 Grounds（墙/天花板/太陡不算），
    /// 因而 <see cref="IGroundDetector.IsGrounded"/> 即 Grounds 非空。
    /// 地面数据只存聚合值不存 Unity 接触数组（无每帧分配）；法线在读取时按需聚合，
    /// 不存在"必须某帧先重算"的时序坑。垂直探针（原 GroundVerticalProbe）并入本类。
    public class GroundDetector : IGroundDetector
    {
        const float InvalidDistance = -1f;

        bool _enabled;
        bool _grounded;
        World _world;
        LayerMask _groundMask;
        float _maxSlope;
        float _maxSlopeCos;
        float _maxGroundDistance;

        Vector3 _position;
        List<Ground> _grounds;
        Vector3 _groundContactPoint;

        public event Action<bool> GroundingChanged;

        public GroundDetector(EnvironmentSettings settings, World world = default)
            : this(settings.MaxSlope, settings.GroundMask, world, settings.MaxGroundDistance)
        {
        }

        public GroundDetector(float maxSlope, LayerMask groundMask, World world = default, float maxGroundDistance = 1f)
        {
            _world = world ?? World.Default;
            _groundMask = groundMask;
            _maxGroundDistance = Mathf.Max(0f, maxGroundDistance);
            _position = TPhysicsHelper.InvalidPosition;
            _grounds = new();
            _grounded = false;
            MaxSlope = maxSlope;
        }

        Vector3 WorldUp => _world.Up;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                SyncGrounding();
            }
        }

        /// 探针原点（通常来自刚体位置）。无效位置 = 不探测。
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public float MaxSlope
        {
            get => _maxSlope;
            set
            {
                _maxSlope = Mathf.Clamp(value, 0f, 90f);
                _maxSlopeCos = Ground.MaxSlopeCos(_maxSlope);
            }
        }

        public bool IsGrounded => _grounded;

        public IReadOnlyList<Ground> Grounds => _grounds;

        public Vector3 GroundsNormal
        {
            get
            {
                Vector3 sum = Vector3.zero;
                int weight = 0;
                for (int i = 0; i < _grounds.Count; i++)
                {
                    var g = _grounds[i];
                    if (!g.IsValid || g.Obj == null)
                        continue;
                    sum += g.Normal * g.SupportCount;
                    weight += g.SupportCount;
                }
                return weight > 0 ? (sum / weight).normalized : Vector3.zero;
            }
        }

        public float GroundDistance
        {
            get
            {
                if (!_enabled || _maxGroundDistance <= 0f || _position == TPhysicsHelper.InvalidPosition)
                    return InvalidDistance;
                if (_grounded)
                    return 0f;
                if (!Physics.Raycast(_position, WorldUp, out var hit, _maxGroundDistance, _groundMask))
                    return Mathf.Infinity;
                _groundContactPoint = hit.point;
                return (_position - hit.point).magnitude;
            }
        }

        public Vector3 GroundContactPoint
        {
            get
            {
                var _ = GroundDistance;
                return _groundContactPoint;
            }
        }

        /// 物理步驱动（可选，供外部按 FixedUpdate 调用）；当前数据读取出均为按需计算，可不调用。
        public void OnFixedUpdate()
        {
            PruneInvalid();
            _ = GroundDistance;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (!_enabled || !MatchesGroundMask(collision))
                return;
            if (!Ground.TryCreate(collision, WorldUp, _maxSlopeCos, out var ground))
                return;
            var index = IndexOf(ground.Obj);
            if (index >= 0)
                _grounds[index] = ground;
            else
                _grounds.Add(ground);
            SyncGrounding();
        }

        public void OnCollisionStay(Collision collision)
        {
            if (!_enabled || !MatchesGroundMask(collision))
                return;
            var index = IndexOf(collision.gameObject);
            if (!Ground.TryCreate(collision, WorldUp, _maxSlopeCos, out var ground))
            {
                // 仍贴着该物体但已无支撑接触（如从支撑面滑开只剩侧碰）→ 移除
                if (index >= 0)
                {
                    _grounds.RemoveAt(index);
                    SyncGrounding();
                }
                return;
            }
            if (index >= 0)
                _grounds[index] = ground;
            else
                _grounds.Add(ground);
            SyncGrounding();
        }

        public void OnCollisionExit(Collision collision)
        {
            if (!_enabled)
                return;
            var index = IndexOf(collision.gameObject);
            if (index < 0)
                return;
            _grounds.RemoveAt(index);
            SyncGrounding();
        }

        /// 清空地面与探针状态（传送/重置时调用）。
        public void Reset()
        {
            _grounds.Clear();
            _position = TPhysicsHelper.InvalidPosition;
            _groundContactPoint = default;
            SyncGrounding();
        }

        int IndexOf(GameObject obj)
        {
            for (int i = 0; i < _grounds.Count; i++)
            {
                if (_grounds[i].Obj == obj)
                    return i;
            }
            return -1;
        }

        bool MatchesGroundMask(Collision collision)
        {
            if (collision == null)
                return false;
            var layer = collision.gameObject.layer;
            return ((1 << layer) & _groundMask.value) != 0;
        }

        void PruneInvalid()
        {
            for (int i = _grounds.Count - 1; i >= 0; i--)
            {
                if (!_grounds[i].IsValid || _grounds[i].Obj == null)
                    _grounds.RemoveAt(i);
            }
            if (_grounds.Count > 0)
                SyncGrounding();
        }

        void SyncGrounding()
        {
            var was = _grounded;
            _grounded = _enabled && _grounds.Count > 0;
            if (was != _grounded)
                GroundingChanged?.Invoke(_grounded);
        }
    }
}
