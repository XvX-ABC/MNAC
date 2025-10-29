using System;
using Tests.Behaviours.Input;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using UnityEngine;
using ISwordBoostingDefinitions = Tests.Behaviours.Arms.Weapons.Sword.IBoostingDefinitions;
namespace Tests.Characters.Arms.Weapons.Sword
{
    internal class BoostingHelper : Behaviours.Arms.Weapons.Sword.BoostingHelper
    {
        internal bool inBoosting;
        public BoostingHelper(
                TPhysics.Locomotion.LocomotionCore locomotionCore,
                Camera camera,
                IBaseInput baseInput,
                IWeaponControlInput weaponControlInput,
                ISwordBoostingDefinitions definitions) : base(
                     locomotionCore,
                     camera,
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
