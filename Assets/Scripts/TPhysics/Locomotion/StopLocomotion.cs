using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    public class StopLocomotion : LocomotionModuleBase
    {
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
            var v = context.CurrentVelocity;
            var normal = context.groundNormal;
            context.CurrentVelocity = Vector3.Project(v, normal);
            return context;
        }
    }
}