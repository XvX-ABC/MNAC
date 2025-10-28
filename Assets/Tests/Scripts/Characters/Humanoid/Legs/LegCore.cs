using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Foots;
using Tests.TPhysics;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Legs
{
    [RequiredComponent(typeof(LegIK))]
    public class LegCore : ComponentBase_MonoComponent
    {
        [SerializeField]
        GameObject _footObj;
        ILegDefinitions _definitions;
        LegIK _legIk;
        SimpleFootIK _footIk;
        float _weight;
        World _world;
        public float Weight
        {
            get => _weight;
            set
            {
                if (_footIk != null)
                    _footIk.Weight = value;
                _weight = value;
            }
        }
        internal SimpleFootIK footIK
        {
            get => _footIk;
            set
            {
                _footIk = value;
                _footIk.Weight = _weight;
            }
        }
        protected Vector3 worldUp
        {
            get => _world == null ? World.DefaultUp : _world.Up;
        }

        protected override void Awake()
        {
            base.Awake();
            _legIk = GetComponent<LegIK>();
            _definitions = GetComponent<ILegDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILegDefinitions));
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out _world))
                throw new Exception();
            _footIk = new(_footObj, _legIk, _definitions.FootIKLayer, _definitions.FootIKPositionOffset, World.DefaultUp);
        }
        private void LateUpdate()
        {
            _footIk.WorldUpward = worldUp;
            _footIk.OnLateUpdate();
        }
    }
}
