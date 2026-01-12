using Tests.Interaction;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class MoveTowardTargetLocomotion : LocomotionModuleBase
    {
        HorizontalLocomotion _base;
        ITarget_Obsolete _target;
        public MoveTowardTargetLocomotion(float maxSpeed, float acceleratedSpeed)
        {
            _base = new(maxSpeed, acceleratedSpeed);
        }
        public float MaxSpeed
        {
            get => _base.MaxSpeed;
            set => _base.MaxSpeed = value;
        }
        public float AcceleratedSpeed
        {
            get => _base.AcceleratedSpeed;
            set => _base.AcceleratedSpeed = value;
        }
        public Vector3 HorizontalVectorWhenNoTarget
        {
            get => _base.DirectionVector;
            set => _base.DirectionVector = value;
        }
        public ITarget_Obsolete Target
        {
            get => _target;
            set => _target = value;
        }
        public override Context OnEnd(Context context)
        {
            return OnUpdate(context);
        }

        public override Context OnStart(Context context)
        {
            return OnUpdate(context);
        }

        public override Context OnUpdate(Context context)
        {
            if (_target != null)
            {
                var tpos = _target.Position;
                var cpos = context.CurrentPosition;
                var tv = tpos - cpos;
                _base.DirectionVector = Vector3.ProjectOnPlane(tv, Vector3.up).normalized;
            }
                return _base.OnUpdate(context);
        }
    }
}