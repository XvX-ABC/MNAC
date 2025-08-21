using Assets.Tests.Scripts.BDExtensions.Variables;
using BehaviorDesigner.Runtime;
using RootMotion.FinalIK;
using System;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Tests.Behaviours.Arms
{
    public class ArmAimerAction : Action
    {
        AimIK _ik;
        [SerializeField]
        SharedTarget _target;
        [SerializeField]
        SharedGameObject _owner;
        public float Weight
        {
            get => _ik.solver.IKPositionWeight;
            set => _ik.solver.IKPositionWeight = value;
        }
        public override void OnAwake()
        {
            var obj = (GameObject)_owner.Value;
            _ik = GetComponent<AimIK>();
        }
        public bool BEnd()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 0f;
            return true;
        }

        public bool BStart()
        {
            if (_target == null)
            {
                return false;
            }
            _ik.solver.IKPositionWeight = 1f;
            //_endabled = true;
            return true;

        }
        public override void OnStart()
        {
            _ik.solver.IKPositionWeight = 1f;
        }
        public override void OnEnd()
        {
            _ik.solver.IKPositionWeight = 0f;
        }
        public override TaskStatus OnUpdate()
        {
            if (_target.Value == null)
                return TaskStatus.Failure;
            _ik.solver.IKPosition = _target.Value.Position;
            return TaskStatus.Running;

        }
        //public void OnUpdate()
        //{
        //    if (!_endabled)
        //        return;
        //    _ik.solver.IKPosition = _target.Position;
        //}
    }
}
