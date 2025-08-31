using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TMPro;
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
            if (!CheckGroundSlope(ground))
                return;
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
            var index = _grounds.FindIndex(g => g.Equals(collision));
            if (index == -1)
                return;
            _grounds[index].Update();
        }
        public void OnCollisionExit(Collision collision)
        {
            if (!_enabled)
                return;
            if (collision == null)
                throw new ArgumentNullException(nameof(collision));
            var ground = new Ground(collision);
            _grounds.Remove(ground);




            if (_grounds.Count == 0)
                _probe.enabled = true;
        }
        public void OnFixedUpdate()
        {

            if (!_enabled)
                return;
            Debug.Log("grounds count:  " + _grounds.Count);
            _probe.OnFixedUpdate();
        }
    }
}
