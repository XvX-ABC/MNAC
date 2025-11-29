using UnityEngine;

namespace Tests.Weapons_New
{
    public abstract class WeaponSource : MonoBehaviour, IWeaponSource
    {
        public abstract IWeapon Weapon { get; }
        public abstract string Name { get; }

        public abstract void Dispose();
        public abstract void Initialize();
    }
}
