using System;

namespace MNAC.Weapons
{
    public class WeaponObjGetFailedByName : Exception
    {
        public WeaponObjGetFailedByName(string name) : base($"Get a weapon obj by the name '{name}'  failed.")
        {

        }
    }
}
