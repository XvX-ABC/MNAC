using System;
using MNAC.States;
namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    class Unoccupied : PlayableStatemachineState<object>
    {
        ArmController _armController;
        public Unoccupied(ArmController anotherArmCore, string name, bool enabled = true) : base(anotherArmCore.stateMachine, name, 0, enabled)
        {
            _armController = anotherArmCore ?? throw new ArgumentNullException(nameof(anotherArmCore));
        }

        public ArmController ArmController
        {
            get => _armController;
            set
            {
                if (_armController != null)
                {
                    if (value != null)
                        value.enabled = _armController.enabled;
                }
                _armController = value;
            }
        }
        public override void OnEnter()
        {
        }
        public override void OnExit()
        {
        }
        public override void OnUpdate()
        {
        }

    }
}
