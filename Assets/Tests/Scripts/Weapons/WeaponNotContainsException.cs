using System;

namespace Tests.Weapons
{
    public class WeaponNotContainsException : Exception
    {
        public WeaponNotContainsException(WeaponCore_Obsolete core, string name) : base($"There is not exists a weapon origin which's name is '{name}' in the weapon core '{core.name}'")
        {

        }
    }
}
