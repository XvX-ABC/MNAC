using Tests.Characters.Humanoid.Locomotion;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public interface IArmedSwordArmBehaviourDefinitions : Behaviours.Arms.Weapons.Sword.IArmedSwordArmBehaviourDefinitions
    {
        public LayerMask ExcludeLayers { get; }
        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions);
    }
}
