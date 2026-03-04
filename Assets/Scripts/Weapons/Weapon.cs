using UnityEngine;

namespace MNAC.Weapons
{
    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        public string Name => this.name;

        public GameObject Obj => this.gameObject;

        public abstract WeaponType Type { get; }
    }
}
