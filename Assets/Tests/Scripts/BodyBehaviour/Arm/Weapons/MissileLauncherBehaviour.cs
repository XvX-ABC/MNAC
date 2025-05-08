using System;
using Tests.Weapons;
using Tests.Weapons.MissileLauncher;


namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    public class MissileLauncherBehaviour : LauncherBehaviour
    {
        IMissileLauncher _launcher;
        public override IWeapon Weapon
        {
            get => base.Weapon;
            set
            {
                base.Weapon = value;
                if (value is IMissileLauncher launcher)
                {
                    _launcher = launcher;
                }
                else
                {
                    throw new InvalidCastException($"This weapon '{value.Name}' is not a missile launcher.");
                }
            }
        }
        public override ITarget Target
        {
            get => base.Target;
            set
            {
                base.Target = value;
                _launcher.Target = value;
            }
        }
    }
}
