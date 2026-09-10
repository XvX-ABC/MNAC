using System;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 把速度收敛到支撑面平面（去掉垂直分量），用于停止时保持贴地。
    [Serializable]
    public class StopLocomotion : LocomotionModuleBase
    {
        public override Context OnUpdate(Context context)
        {
            context.CurrentVelocity = Vector3.Project(context.CurrentVelocity, context.groundNormal);
            return context;
        }
    }
}
