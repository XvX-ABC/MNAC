using Tests.Characters.Humanoid.Locomotion;

namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public interface IArmedSwordArmBehaviourDefinitions : Behaviours.Arms.Weapons.Sword.IArmedSwordArmBehaviourDefinitions
    {

        TriggerDefinitions Trigger { get; }

        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions);
    }
}
