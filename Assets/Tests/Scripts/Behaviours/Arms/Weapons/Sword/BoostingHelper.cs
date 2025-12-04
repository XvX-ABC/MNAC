using System;
using Tests.Behaviours.Input;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class BoostingHelper
    {
        internal Boosting state;
        internal BoostingLocomotion locomotion;
        ITimeline _cdTimeline;
        IWeaponControlInput _input;
        public BoostingHelper(LocomotionCore locomotionCore, Camera camera, ITargetLocker targetLocker, IBaseInput baseInput, IWeaponControlInput weaponControlInput, IBoostingDefinitions definitions)
        {
            locomotion = new BoostingLocomotion(definitions.MaxSpeed, 0);
            state = new(locomotion, locomotionCore, camera, targetLocker, baseInput, definitions.MaxDuration);
            _input = weaponControlInput;
            var cd = definitions.ColdDownDuration;
            if (cd > 0)
            {
                _cdTimeline = new Timeline_V1(definitions.ColdDownDuration);
                state.ExitAction += () => _cdTimeline.Restart();
                _cdTimeline.SetNormalizedTime(1);
            }
        }
        public bool IsColdDowned
        {
            get => _cdTimeline == null ? true : _cdTimeline.NormalizedTime >= 1;
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
            _cdTimeline?.OnUpdate(Time.deltaTime);
        }
    }
}
