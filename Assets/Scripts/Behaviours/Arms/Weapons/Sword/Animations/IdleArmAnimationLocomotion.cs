using System;
using Tests.TPhysics.Locomotion;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class IdleArmAnimationLocomotion : EvaluationModuleBase
    {
        Idle _idle;
        public IdleArmAnimationLocomotion(Idle idle)
        {
            _idle = idle ?? throw new ArgumentNullException(nameof(idle));
        }
        public override Context Update(Context context)
        {
            _idle.SetVelocity(context);
            return base.Update(context);
        }
    }
}
