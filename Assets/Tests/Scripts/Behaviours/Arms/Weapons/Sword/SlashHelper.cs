using Tests.TPhysics.Locomotion;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class SlashHelper
    {
        internal Slash state;
        internal GameObject target;
        internal float slashDuration;
        internal float recoveryDuration;
        public SlashHelper(LocomotionCore locomotionCore, IRotationLocker rotationLocker, float slashDuration, float recoveryDuration)
        {
            this.slashDuration = slashDuration;
            this.recoveryDuration = recoveryDuration;
            state = new(locomotionCore, rotationLocker, slashDuration + recoveryDuration);
            state.EntryAction += () => target = null;
        }
        public virtual bool EntryEvent { get => target != null; }
        public virtual bool ExitEvent { get => state.Timeline.NormalizedTime >= 1; }
        public GameObject Target
        {
            get => target;
            set => target = value;
        }
    }
}
