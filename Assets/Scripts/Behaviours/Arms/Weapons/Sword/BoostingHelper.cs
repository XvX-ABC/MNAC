using System;
using MNAC.Behaviours.Input;
using MNAC.Characters.Interaction.Input;
using MNAC.TPhysics.Locomotion;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Sword
{
    internal class BoostingHelper
    {
        internal Boosting state;
        internal BoostingLocomotion locomotion;
        internal ITimeline cdTimeline;
        IWeaponControlInput _input;

        public BoostingHelper(LocomotionCore locomotionCore, IRotationLocker rotationLocker, ITargetLocker targetLocker, IBaseInput baseInput, IWeaponControlInput weaponControlInput, IBoostingDefinitions definitions)
        {
            locomotion = new BoostingLocomotion(definitions.MaxSpeed, 0);
            state = new(locomotion, locomotionCore, rotationLocker, targetLocker, baseInput, definitions.MaxDuration);
            _input = weaponControlInput;
            var cd = definitions.ColdDownDuration;
            cdTimeline = new Timeline(definitions.ColdDownDuration);
            if (cd > 0)
            {

                state.ExitAction += () => cdTimeline.Restart();
                cdTimeline.SetNormalizedTime(1);
            }
        }
        public bool IsColdDowned
        {
            get => cdTimeline == null ? true : cdTimeline.NormalizedTime >= 1;
        }
        public virtual bool EntryEvent
        {
            //get => _input == null ? false : _input.Fire && IsColdDowned;
            get => _input == null ? false : _input.Fire && IsColdDowned;
        }
        public virtual bool ExitEvent { get => state.Timeline.NormalizedTime >= 1; }
        public IWeaponControlInput Input { get => _input; set => _input = value; }
        public virtual void Update()
        {
            cdTimeline?.OnUpdate(Time.deltaTime);
        }
    }
}
