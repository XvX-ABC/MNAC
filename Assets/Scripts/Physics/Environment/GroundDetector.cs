using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public class GroundDetector : IGroundDetector
    {

        bool _enabled;

        World _world;
        float _maxSlope;
        LayerMask _groundMask;

        GroundVerticalProbe _probe;

        List<Ground> _grounds;
        Vector3 _groundsNormal;


        Vector3 _worldUpVector => _world == null ? World.DefaultUp : _world.Up;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _probe.enabled = value ? _grounds.Count == 0 : false;
                _enabled = value;
                _grounds.TrimExcess();
            }
        }
        public Vector3 Position
        {
            get => _probe.Position;
            set => _probe.Position = value;
        }
        public IReadOnlyList<Ground> Grounds { get => _grounds; }
        public GroundVerticalProbe Probe { get => _probe; }
        public World World
        {
            get => _world;
            set
            {
                _world = value;
                _probe.world = value;
            }
        }
        public float MaxSlope { get => _maxSlope; set => _maxSlope = value; }
        public LayerMask GroundMask { get => _groundMask; set => _groundMask = value; }

        public GroundDetector([NotNull] IGroundDetectionDefinitions definitions, World world = default) : this(definitions.MaxSlope, definitions.GroundMask, world)
        {
        }
        public GroundDetector(float maxSlope, LayerMask groundMask, World world = default)
        {
            _maxSlope = maxSlope;
            _groundMask = groundMask;
            _probe = new(groundMask, world);
            _grounds = new();
        }
        bool CheckGroundSlope(Ground ground)
        {
            var normal = ground.Normal;
            var angle = Vector3.Angle(normal, _worldUpVector);
            return angle <= _maxSlope;
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (!_enabled)
                return;
            if (collision == null)
                throw new ArgumentNullException(nameof(collision));

            var layer = collision.gameObject.layer;
            if (((1 << layer) & _groundMask.value) == 0)
                return;
            var ground = new Ground(collision);
            _grounds.Add(ground);


            if (_grounds.Count > 0)
            {
                _probe.enabled = false;
            }

        }
        public void OnCollisionStay(Collision collision)
        {
            if (!_enabled)
                return;
            var layer = collision.gameObject.layer;
            if (((1 << layer) & _groundMask) == 0)
                return;
            var index = _grounds.FindIndex(g => g.Obj == collision.gameObject);
            if (index == -1)
                return;
            var g = _grounds[index];
            g.Update(collision);
            _grounds[index] = g;
        }
        public void OnCollisionExit(Collision collision)
        {
            if (!_enabled)
                return;
            if (collision == null)
                throw new ArgumentNullException(nameof(collision));
            var index = _grounds.FindIndex(g => g.Obj == collision.gameObject);
            if (index == -1)
                return;
            _grounds.RemoveAt(index);




            if (_grounds.Count == 0)
                _probe.enabled = true;
        }
        public Vector3 GroundsNormal
        {
            get
            {
                return _groundsNormal;
            }
        }
        Vector3 CalculateGroundsNormal()
        {
            if (_grounds.Count == 0)
                return Vector3.zero;
            var result = Vector3.zero;
            foreach (var g in _grounds)
            {
                if (CheckGroundSlope(g))
                {
                    result += g.Normal;
                }
            }
            return (result / _grounds.Count).normalized;
        }
        public void OnLateUpdate()
        {
            if (!_enabled)
                return;
            _groundsNormal = CalculateGroundsNormal();
        }
        public void OnFixedUpdate()
        {
            if (!_enabled)
                return;
            _probe.OnFixedUpdate();
        }
    }
}
