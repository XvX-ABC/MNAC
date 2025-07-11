using Assets.Tests.Scripts.BDExtensions.Variables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.BDExtensions.Variables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Tests.BodyBehaviour.Arm.Weapons
{
    public class ArmAimer_New : Action
    {
        [SerializeField]
        SharedArmIK _armIK;
        [SerializeField]
        SharedTarget _target;
        public override void OnStart()
        {
            _armIK.Value.solver.IKPositionWeight = 1f;
        }
        public override void OnEnd()
        {
            _armIK.Value.solver.IKPositionWeight = 1f;
        }
        public override TaskStatus OnUpdate()
        {
            if (_target.Value == null)
            {
                _armIK.Value.solver.IKPositionWeight = 0f;
                return TaskStatus.Failure;
            }
            _armIK.Value.solver.IKPosition = _target.Value.Position;
            return TaskStatus.Running;
        }

    }
}
