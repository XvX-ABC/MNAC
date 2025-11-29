using System;

namespace Tests.Weapons
{
    public class WeaponObjGetFailedByName : Exception
    {
        public WeaponObjGetFailedByName(string name) : base($"Get a weapon obj by the name '{name}'  failed.")
        {

        }
    }
}
