using UnityEngine;

namespace Tests.TPhysics.Locomotion
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
            context.CurrentVelocity = Vector3.zero;
            return context;
        }
    }
}