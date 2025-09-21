using Tests.Behaviours.Arm.Weapons;
using Tests.Input;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Behaviours.Arms
{
    internal class ArmAiming : ArmedArmStateBase
    {
        IInput _input;
        ILauncher _controlledWeapon;
        public ArmAiming() : base("aiming", 0)
        {
        }

        public IInput Input { get => _input; set => _input = value; }
        public ILauncher ControlledWeapon
        {
            get => _controlledWeapon;
            //set => _controlledWeapon = value;
            set
            {
                if (_controlledWeapon != null && value != _controlledWeapon && _controlledWeapon.LaunchDurationTimeline.IsRunning)
                    _controlledWeapon.EndLaunch();
                _controlledWeapon = value;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_input != null && _input.Fire)
            {
                _controlledWeapon.StartLaunch();
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (_input != null)
                _controlledWeapon.EndLaunch();
        }
    }
}
