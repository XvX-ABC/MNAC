using Tests.Behaviours.Arm.Weapons;
using Tests.Characters.Interaction.Input;
using Tests.States;
using Tests.Weapons_New.Launcher;

namespace Tests.Behaviours.Arms
{
    internal class ArmAiming : ArmedArmStateBase
    {
        IWeaponControlInput _input;
        ILauncher _controlledWeapon;
        StateLifeCycleWatcher<object> _lifeWatcher;
        public ArmAiming() : base("aiming", 0)
        {
            _lifeWatcher = new(this);
        }

        public IWeaponControlInput Input { get => _input; set => _input = value; }
        public ILauncher ControlledWeapon
        {
            get => _controlledWeapon;
            //set => _controlledWeapon = value;
            set
            {
                if (_controlledWeapon != null)
                    _controlledWeapon.FireTrigger -= FireTrigger;
                if (value != null)
                    value.FireTrigger += FireTrigger;
                _controlledWeapon = value;
            }
        }
        bool FireTrigger()
        {
            return _input != null && _lifeWatcher.CurrentState == LifeCycleState.Update && _input.Fire;
        }
        //public override void OnUpdate()
        //{
        //    base.OnUpdate();
        //    //if (_input != null && _input.Fire)
        //    if (_input != null && _input.Fire)
        //    {
        //        _controlledWeapon.StartLaunch();
        //    }
        //}
        //public override void OnExit()
        //{
        //    base.OnExit();
        //    if (_input != null)
        //        _controlledWeapon.EndLaunch();
        //}
    }
}
