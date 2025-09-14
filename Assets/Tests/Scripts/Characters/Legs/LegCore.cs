using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Foots;
using Tests.TPhysics;
using UnityEngine;

namespace Tests.Characters.Legs
{
    public class LegsCore : CharacterComponentBase_MonoComponent
    {
        [SerializeField]
        LegCore _leftLeg;
        [SerializeField]
        LegCore _rightLeg;
        public float Weight
        {
            get => _leftLeg.Weight;
            set
            {
                _leftLeg.Weight = value;
                _rightLeg.Weight = value;
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Legs_Core, this);
        }
    }
    [RequiredComponent(typeof(LegIK))]
    public class LegCore : CharacterComponentBase_MonoComponent
    {
        [SerializeField]
        GameObject _footObj;
        ILegDefinitions _definitions;
        LegIK _legIk;
        SimpleFootIK _footIk;
        World _world;
        public float Weight
        {
            get => _footIk.Weight;
            set => _footIk.Weight = value;
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
            _footIk.WorldUpward = _world.Up;
            _footIk.OnLateUpdate();
        }
    }
}
