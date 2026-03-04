using System;
using MNAC.Behaviours;
using MNAC.Behaviours.Arms.Weapons.Sword;
using MNAC.Behaviours.Input;
using MNAC.Characters.Interaction;
using MNAC.Characters.Interaction.Input;
using MNAC.Input;
using UnityEngine;
using ISwordBoostingDefinitions = MNAC.Behaviours.Arms.Weapons.Sword.IBoostingDefinitions;
namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class BoostingHelper : Behaviours.Arms.Weapons.Sword.BoostingHelper
    {
        internal bool inBoosting;
        public BoostingHelper(
                TPhysics.Locomotion.LocomotionCore locomotionCore,
                IRotationLocker rotationLocker,
                ITargetLocker targetLocker,
                IBaseInput baseInput,
                IWeaponControlInput weaponControlInput,
                ISwordBoostingDefinitions definitions) : base(
                     locomotionCore,
                     rotationLocker,
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
