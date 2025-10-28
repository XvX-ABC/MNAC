using System;
using Tests.Characters.Humanoid.Locomotion;
using UnityEngine;
using LBoostingDefinitions = Tests.Characters.Humanoid.Locomotion.BoostingDefinitions;
using SwordBoostingDefinitioins = Tests.Behaviours.Arms.Weapons.Sword.BoostingDefinitions;
using SwordSlashDefinitions = Tests.Behaviours.Arms.Weapons.Sword.SlashDefinitions;
namespace Tests.Characters.Arms.Weapons.Sword
{
    public class ArmedSwordArmBehaviourDefinitions : MonoBehaviour, IArmedSwordArmBehaviourDefinitions
    {
        [SerializeField]
        LBoostingDefinitions _boosting;
        [SerializeField]
        float _duration;
        [SerializeField]
        float _slashDuration;
        [SerializeField]
        LayerMask _targetsMask;
        SwordBoostingDefinitioins _sboosting;
        SwordSlashDefinitions _sslash;
        public Behaviours.Arms.Weapons.Sword.IBoostingDefinitions Boosting => _sboosting;

        public Behaviours.Arms.Weapons.Sword.ISlashDefinitions Slash => _sslash;

        public LayerMask ExcludeLayers => _targetsMask;

        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions)
        {
            if (locomotionDefinitions == null || locomotionDefinitions.Walking == null)
                throw new ArgumentNullException(nameof(locomotionDefinitions));
            var b = locomotionDefinitions.Walking;
            _sboosting = new SwordBoostingDefinitioins(_boosting.MaxSpeedPower * b.MaxSpeed, _boosting.AcceleratedSpeedPower * b.AcceleratedSpeed, _duration);
            _sslash = new SwordSlashDefinitions(_slashDuration);
        }
    }
}
