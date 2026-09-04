using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 冲刺：启动时清空速度，期间水平移动并把垂直分量压平。
    public class BoostingLocomotion : HorizontalLocomotion
    {
        public BoostingLocomotion(float maxSpeed, float acceleratedSpeed) : base(maxSpeed, acceleratedSpeed)
        {
        }

        public BoostingLocomotion(float maxSpeed, float acceleratedSpeed, Vector3 horizontalVector)
            : base(maxSpeed, acceleratedSpeed, horizontalVector)
        {
        }

        public override Context OnStart(Context context)
        {
            context.CurrentVelocity = Vector3.zero;
            return context;
        }

        public override Context OnUpdate(Context context)
        {
            context = base.OnUpdate(context);
            context.CurrentVelocity = Vector3.ProjectOnPlane(context.CurrentVelocity, context.world.Up);
            return context;
        }
    }
}
