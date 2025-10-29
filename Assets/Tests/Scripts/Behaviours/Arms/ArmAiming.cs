using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Behaviours.Arms
{
    internal class ArmAiming : ArmedArmStateBase
    {
        IWeaponControlInput _input;
        ILauncher _controlledWeapon;
        public ArmAiming() : base("aiming", 0)
        {
        }

        public IWeaponControlInput Input { get => _input; set => _input = value; }
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
            //if (_input != null && _input.Fire)
            if (_input != null && _input.Fire)
            {
                Debug.Log("fire");
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
