using Unity.VisualScripting;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public interface IArmDefinitions
    {
        public HumanPartDof Part { get; }
        public IArmWeaponDefinitions Weapon { get; }
    }
}
