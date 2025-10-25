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
        IInput _input;
        public BoostingHelper(LocomotionCore locomotionCore, Camera camera, IInput input, IBoostingDefinitions definitions)
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

        public bool IsColdDowned
        {
            get => _cdTimeline == null ? true : _cdTimeline.NormalizedTime >= 1;
        }
        public virtual bool EntryEvent
        {
            get => _input == null ? false : _input.Fire && IsColdDowned;
        }
        public virtual bool ExitEvent { get => state.Timeline.NormalizedTime >= 1; }
        public IInput Input { get => _input; set => _input = value; }
        public virtual void Update()
        {
            _cdTimeline?.OnUpdate(Time.deltaTime);
        }
    }
}
