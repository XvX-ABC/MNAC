using System;

namespace Tests.Weapons_New.Sword
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
