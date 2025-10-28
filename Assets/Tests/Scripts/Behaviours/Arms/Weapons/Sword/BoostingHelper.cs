using System;
using Tests.Behaviours.Input;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class BoostingHelper
    {
        internal Boosting state;
        internal BoostingLocomotion locomotion;
        ITimeline _cdTimeline;
        [Obsolete]
        IInput_Obsolete _input;
        IWeaponControlInput _winput;
        [Obsolete]
        public BoostingHelper(LocomotionCore locomotionCore, Camera camera, IInput_Obsolete input, IBoostingDefinitions definitions)
        {
            locomotion = new BoostingLocomotion(definitions.MaxSpeed, 0);
            state = new(locomotion, locomotionCore, camera, input, definitions.MaxDuration);
            _input = input;
            var cd = definitions.ColdDownDuration;
            if (cd > 0)
            {
                _cdTimeline = new Timeline_V1(definitions.ColdDownDuration);
                state.ExitAction += () => _cdTimeline.Restart();
                _cdTimeline.SetNormalizedTime(1);
            }
        }

        public BoostingHelper(LocomotionCore locomotionCore, Camera camera, IBaseInput baseInput, IWeaponControlInput weaponControlInput, IBoostingDefinitions definitions)
        {
            locomotion = new BoostingLocomotion(definitions.MaxSpeed, 0);
            state = new(locomotion, locomotionCore, camera, baseInput, definitions.MaxDuration);
            _winput = weaponControlInput;
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
            get => _winput == null ? false : _winput.Fire && IsColdDowned;
        }
        public virtual bool ExitEvent { get => state.Timeline.NormalizedTime >= 1; }
        [Obsolete]
        public IInput_Obsolete Input_Obsolete { get => _input; set => _input = value; }
        public IWeaponControlInput Input { get => _winput; set => _winput = value; }
        public virtual void Update()
        {
            _cdTimeline?.OnUpdate(Time.deltaTime);
        }
    }
}
