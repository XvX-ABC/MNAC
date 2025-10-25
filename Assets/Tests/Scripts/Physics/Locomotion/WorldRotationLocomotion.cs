using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    internal class WorldRotationLocomotion : LocomotionModuleBase
    {
        Quaternion _offset;
        Vector3 _origin;
        Vector3 _forward;
        Vector3 _targetPos;
        public WorldRotationLocomotion()
        {
            _forward = Vector3.forward;
            _offset = Quaternion.identity;
        }

        public Quaternion Offset { get => _offset; set => _offset = value; }
        public Vector3 Origin { get => _origin; set => _origin = value; }
        public Vector3 Forward { get => _forward; set => _forward = value; }
        public Vector3 TargetPos { get => _targetPos; set => _targetPos = value; }

        public override Context OnEnd(Context context)
        {
            return context;
        }

        public override Context OnStart(Context context)
        {
            return context;
        }

        public override Context OnUpdate(Context context)
        {
            context.CurrentRotation = CalculateRotation(context);
            return context;
        }
        Quaternion CalculateRotation(Context context)
        {
            var tpos = _targetPos - _origin;
            var r = Quaternion.FromToRotation(_forward, tpos);
            if (_offset != Quaternion.identity)
                return Quaternion.Slerp(context.CurrentRotation, _offset * r, Time.deltaTime * 15);
            else
                return Quaternion.Slerp(context.CurrentRotation, r, Time.deltaTime * 15);
        }
    }
}
