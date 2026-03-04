using MNAC.Characters.Humanoid.Locomotion;

namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    public interface IArmedSwordArmBehaviourDefinitions : Behaviours.Arms.Weapons.Sword.IArmedSwordArmBehaviourDefinitions
    {

        TriggerDefinitions Trigger { get; }

        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions);
    }
}
