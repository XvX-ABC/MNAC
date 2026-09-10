using System;

namespace MNAC.TPhysics.Locomotion
{
    [Serializable]
    public class StopRotation : LocomotionModuleBase
    {
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
            context.CurrentRotation = context.Original.Rotation;
            return context;
        }
    }
}
