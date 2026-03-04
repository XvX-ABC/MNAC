using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using MNAC.Behaviours.Foots;
using MNAC.TPhysics;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Legs
{
    [RequiredComponent(typeof(LegIK))]
    public class LegController : ComponentBase_MonoComponent
    {
        [SerializeField]
        GameObject _footObj;
        [SerializeField]
        LayerMask _groundLayerMask;
        [SerializeField]
        Vector3 _offset;
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
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue(CharacterBlackboardFields.World, out _world))
                throw new Exception();
            _footIk = new(_footObj, _legIk, _groundLayerMask, _offset, World.DefaultUp);
        }
        private void LateUpdate()
        {
            _footIk.WorldUpward = worldUp;
            _footIk.OnLateUpdate();
        }
    }
}
