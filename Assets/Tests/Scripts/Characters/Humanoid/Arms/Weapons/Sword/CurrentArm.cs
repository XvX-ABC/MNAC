using System;
using Tests.States;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    class CurrentArm : WithCallbackPlayableState
    {
        ArmController _armCore;
        public CurrentArm(ArmController anotherArmCore, string name, bool enabled = true) : base(name, 0, enabled)
        {
            _armCore = anotherArmCore ?? throw new ArgumentNullException(nameof(anotherArmCore));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _armCore.enabled = false;
        }
        public override void OnExit()
        {
            _armCore.enabled = true;
            base.OnExit();
        }
    }
}
