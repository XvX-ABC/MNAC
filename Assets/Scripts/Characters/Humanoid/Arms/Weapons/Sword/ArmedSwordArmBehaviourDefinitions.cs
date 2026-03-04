using System;
using MNAC.Characters.Humanoid.Locomotion;
using UnityEngine;
using LBoostingDefinitions = MNAC.Characters.Humanoid.Locomotion.BoostingDefinitions;
using SwordBoostingDefinitioins = MNAC.Behaviours.Arms.Weapons.Sword.BoostingDefinitions;
using SwordSlashDefinitions = MNAC.Behaviours.Arms.Weapons.Sword.SlashDefinitions;
namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    [Serializable]
    public class ArmedSwordArmBehaviourDefinitions : IArmedSwordArmBehaviourDefinitions
    {
        [SerializeField]
        LBoostingDefinitions _boosting;
        [SerializeField]
        float _boostingDuration;
        [SerializeField]
        float _boostingCD;
        [SerializeField]
        float _slashDuration;
        [SerializeField]
        float _slashedRecoverDuration;
        [SerializeField]
        TriggerDefinitions _trigger;
        SwordBoostingDefinitioins _sboosting;
        SwordSlashDefinitions _slash;
        public Behaviours.Arms.Weapons.Sword.IBoostingDefinitions Boosting => _sboosting;

        public Behaviours.Arms.Weapons.Sword.ISlashDefinitions Slash => _slash;

        public TriggerDefinitions Trigger => _trigger;

        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions)
        {
            if (locomotionDefinitions == null || locomotionDefinitions.Walking == null)
                throw new ArgumentNullException(nameof(locomotionDefinitions));
            var b = locomotionDefinitions.Walking;
            _sboosting = new SwordBoostingDefinitioins(_boosting.MaxSpeedPower * b.MaxSpeed, _boostingDuration, _boostingCD);
            _slash = new SwordSlashDefinitions(_slashDuration, _slashedRecoverDuration);
        }
    }
}
