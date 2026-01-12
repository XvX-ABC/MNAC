using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class BoostingLocomotion : HorizontalLocomotion
    {
        public BoostingLocomotion(float maxSpeed, float acceleratedSpeed) : base(maxSpeed, acceleratedSpeed)
        {
        }

        public BoostingLocomotion(float maxSpeed, float acceleratedSpeed, Vector3 horizontalVector) : base(maxSpeed, acceleratedSpeed, horizontalVector)
        {
        }
        public override Context OnStart(Context context)
        {
            context.CurrentVelocity = Vector3.zero;
            return base.OnStart(context);
        }
        public override Context OnUpdate(Context context)
        {
            context = base.OnUpdate(context);
            var worldUp = world.Up;
            var currentVelocity = context.CurrentVelocity;
            context.CurrentVelocity = Vector3.ProjectOnPlane(currentVelocity, worldUp);
            return context;
        }
        public override Context OnEnd(Context context)
        {
            return base.OnEnd(context);
        }
    }
}
