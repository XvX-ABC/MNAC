using Tests.Behaviours.Arms.Weapons.Sword;

namespace Tests.Characters.Arms.Weapons.Sword
{
    internal class SlashHelper : Behaviours.Arms.Weapons.Sword.SlashHelper
    {
        internal bool slashing;
        public SlashHelper(TPhysics.Locomotion.LocomotionCore locomotionCore, IRotationLocker rotationLocker, float slashDuration) : base(locomotionCore, rotationLocker, slashDuration)
        {
        }
        public override bool EntryEvent => slashing && base.EntryEvent;
        public override bool ExitEvent => !slashing;
        internal bool originalEntryEvent => base.EntryEvent;
    }
}
