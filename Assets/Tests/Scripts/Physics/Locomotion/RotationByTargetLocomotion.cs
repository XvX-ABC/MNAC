using Tests.Interaction;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class RotationByTargetLocomotion : LocomotionModuleBase
    {
        RotationLocomotion _b;
        IPositionTarget _target;

        public IPositionTarget Target { get => _target; set => _target = value; }

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
            if (_target == null)
                return context;
            var p = context.WorldPlane;
            var origin = context.CurrentPosition;
            var tpos = _target.Position;
            _b.Origin = Vector3.ProjectOnPlane(origin, p.normal);
            _b.TargetPos = Vector3.ProjectOnPlane(tpos, p.normal);
            return _b.OnUpdate(context);
        }
    }
}
