using Tests.Characters.Humanoid.Locomotion;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    [CreateAssetMenu(fileName = "ArmedSwordArmBehaviourDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Sword/ArmedSwordArmBehaviourDefinitions")]
    public class ArmedSwordArmBehaviourDefinitions_SO : ScriptableObject, IArmedSwordArmBehaviourDefinitions
    {
        [SerializeField]
        ArmedSwordArmBehaviourDefinitions _definitions;

        public LayerMask ExcludeLayerMask => _definitions.ExcludeLayerMask;

        public Behaviours.Arms.Weapons.Sword.IBoostingDefinitions Boosting => _definitions.Boosting;

        public Behaviours.Arms.Weapons.Sword.ISlashDefinitions Slash => _definitions.Slash;

        public LayerMask IncludeLayerMask => _definitions.IncludeLayerMask;


        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions)
        {
            _definitions.InitializeBy(locomotionDefinitions);
        }
    }
}
