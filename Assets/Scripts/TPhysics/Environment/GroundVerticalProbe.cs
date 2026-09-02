using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    public class GroundVerticalProbe
    {

        Vector3 _pos;
        LayerMask _groundMask;
        World _world;

        float _height;
        Vector3 _contactPoint;
        Collider _hitCollider;

        bool _enabled;


        Vector3 _worldUp => _world == null ? World.DefaultUp : _world.Up;

        internal Vector3 Position { get => _pos; set => _pos = value; }
        public float Height { get => _height; }
        public Vector3 ContactPoint { get => _contactPoint; }
        public Collider HitCollider { get => _hitCollider; }
        internal bool enabled
        {
            get => _enabled;
            set
            {
                if (!value)
                    Reset();
                _enabled = value;
            }
        }

        internal World world { get => _world; set => _world = value; }

        internal GroundVerticalProbe(IGroundDetectionDefinitions definitions, World world = default) : this(definitions.GroundMask, world)
        {
        }
        internal GroundVerticalProbe(LayerMask groundMask, World world = default)
        {
            _pos = TPhysicsHelper.InvalidPosition;
        }
        internal void OnFixedUpdate()
        {
            if (!enabled || _pos == TPhysicsHelper.InvalidPosition)
                return;
            if (Physics.Raycast(_pos, _worldUp, out var hitInfo, Mathf.Infinity, _groundMask))
            {
                _hitCollider = hitInfo.collider;
                _contactPoint = hitInfo.point;
                _height = (_pos - _contactPoint).magnitude;
            }
        }
        void Reset()
        {
            _pos = TPhysicsHelper.InvalidPosition;
            _height = 0f;
            _contactPoint = TPhysicsHelper.InvalidPosition;
            _hitCollider = null;
        }
    }
}
