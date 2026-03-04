using System;

namespace MNAC.Weapons.Sword
{
    internal class SwordComponent : WeaponComponent
    {
        static SwordComponent()
        {
            OwnerSword = Guid.NewGuid();
        }
        public readonly static Guid OwnerSword;
    }
}
