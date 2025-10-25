using Tests.Characters.Locomotion;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Sword
{
    public interface IArmedSwordArmBehaviourDefinitions : Behaviours.Arms.Weapons.Sword.IArmedSwordArmBehaviourDefinitions
    {
        public LayerMask ExcludeLayers { get; }
        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions);
    }
}
