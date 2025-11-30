using System;
using Tests.States;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    class AnotherArm : PlayableStatemachineState<object>
    {
        ArmCore _armCore;
        public AnotherArm(ArmCore otherArmCore, string name, bool enabled = true) : base(otherArmCore.stateMachine, name, 0, enabled)
        {
            _armCore = otherArmCore ?? throw new ArgumentNullException(nameof(otherArmCore));
        }

        public ArmCore ArmCore
        {
            get => _armCore;
            set
            {
                if (_armCore != null)
                {
                    if (value != null)
                        value.enabled = _armCore.enabled;
                    _armCore.enabled = false;
                }
                _armCore = value;
            }
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            //_armCore.enabled = true;
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            //_armCore.enabled = false;
        }
    }
}
