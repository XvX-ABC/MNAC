using Tests.Behaviours.Arms.Weapons;
using Unity.VisualScripting;
using UnityEngine;

namespace Tests.Behaviours.Arms
{
    public interface IArmDefinitions
    {
        public HumanPartDof Part { get; }
        public IArmWeaponDefinitions Weapon { get; }
    }
}
