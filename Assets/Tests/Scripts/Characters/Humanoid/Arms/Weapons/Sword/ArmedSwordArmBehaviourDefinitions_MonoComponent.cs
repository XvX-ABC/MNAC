using Tests.Characters.Humanoid.Locomotion;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public class ArmedSwordArmBehaviourDefinitions_MonoComponent : MonoBehaviour, IArmedSwordArmBehaviourDefinitions
    {
        [SerializeField]
        ArmedSwordArmBehaviourDefinitions _definitions;


        public Behaviours.Arms.Weapons.Sword.IBoostingDefinitions Boosting => _definitions.Boosting;

        public Behaviours.Arms.Weapons.Sword.ISlashDefinitions Slash => _definitions.Slash;


        public TriggerDefinitions Trigger => ((IArmedSwordArmBehaviourDefinitions)_definitions).Trigger;

        public void InitializeBy(ILocomotionDefinitions locomotionDefinitions)
        {
            _definitions.InitializeBy(locomotionDefinitions);
        }
        //[SerializeField]
        //LBoostingDefinitions _boosting;
        //[SerializeField]
        //float _duration;
        //[SerializeField]
        //float _slashDuration;
        //[SerializeField]
        //LayerMask _targetsMask;
        //SwordBoostingDefinitioins _sboosting;
        //SwordSlashDefinitions _sslash;
        //public Behaviours.Arms.Weapons.Sword.IBoostingDefinitions Boosting => _sboosting;

        //public Behaviours.Arms.Weapons.Sword.ISlashDefinitions Slash => _sslash;

        //public LayerMask ExcludeLayers => _targetsMask;

        //public void InitializeBy(ILocomotionDefinitions locomotionDefinitions)
        //{
        //    if (locomotionDefinitions == null || locomotionDefinitions.Walking == null)
        //        throw new ArgumentNullException(nameof(locomotionDefinitions));
        //    var b = locomotionDefinitions.Walking;
        //    _sboosting = new SwordBoostingDefinitioins(_boosting.MaxSpeedPower * b.MaxSpeed, _boosting.AcceleratedSpeedPower * b.AcceleratedSpeed, _duration);
        //    _sslash = new SwordSlashDefinitions(_slashDuration);
        //}
    }
}
