using System;
using Tests.Behaviours.Input;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using UnityEngine;
using ISwordBoostingDefinitions = Tests.Behaviours.Arms.Weapons.Sword.IBoostingDefinitions;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class BoostingHelper : Behaviours.Arms.Weapons.Sword.BoostingHelper
    {
        internal bool inBoosting;
        public BoostingHelper(
                TPhysics.Locomotion.LocomotionCore locomotionCore,
                Camera camera,
                TargetLocker targetLocker,
                IBaseInput baseInput,
                IWeaponControlInput weaponControlInput,
                ISwordBoostingDefinitions definitions) : base(
                     locomotionCore,
                     camera,
                     targetLocker,
                     baseInput,
                     weaponControlInput,
                     definitions
        )
        {
        }
        public override bool EntryEvent => inBoosting && base.EntryEvent;
        public override bool ExitEvent => !inBoosting;
        internal bool originalEntryEvent => base.EntryEvent;
    }
}
