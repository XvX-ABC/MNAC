using System;
using Tests.States;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    class Occupited : WithCallbackPlayableState
    {
        ArmController _armController;
        public Occupited(ArmController anotherArmCore, string name, bool enabled = true) : base(name, 0, enabled)
        {
            _armController = anotherArmCore ?? throw new ArgumentNullException(nameof(anotherArmCore));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _armController.stateMachine.OnExit();
            _armController.enabled = false;
        }
        public override void OnExit()
        {
            _armController.enabled = true;
            _armController.stateMachine.OnEnter();
            base.OnExit();
        }
    }
}
