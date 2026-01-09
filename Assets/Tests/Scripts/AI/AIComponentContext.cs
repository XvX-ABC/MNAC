using System;
using System.Diagnostics;
using Tess.AI;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Blackboards;
using UnityEngine.AI;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;

namespace Tests.AI
{
    internal class AIComponentContext
    {
        AICore _core;
        NavMeshAgent _navAgent;
        Blackboard _characterBlackboard;
        AIHumanoidInput _input;
        AINavigation _navigation;
        AITargetLocker _targetLocker;
        Blackboard _blackboard;
        LocomotionCore _locomotionCore;

        internal Blackboard blackboard { get => _blackboard; set => _blackboard = value; }
        internal NavMeshAgent navAgent { get => _navAgent; set => _navAgent = value; }
        internal Blackboard characterBlackboard { get => _core.characterBlackboard; set => _core.characterBlackboard = value; }
        internal AICore core { get => _core; set => _core = value; }
        internal ITarget target { get => _core.interactableTarget; set => _core.interactableTarget = value; }
        internal AINavigation navigation { get => _navigation; set => _navigation = value; }
        internal AITargetLocker targetLocker { get => _targetLocker; set => _targetLocker = value; }
        internal LocomotionCore locomotionCore
        {
            get
            {
                if (_locomotionCore == null)
                {
                    if (!characterBlackboard.TryReadValue(AIBlackboardFields.Character_LocomotionCore, out _locomotionCore))
                    {
                        characterBlackboard.RegisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
                    }
                }
                return _locomotionCore;
            }
            set => _locomotionCore = value;
        }

        internal AIHumanoidInput Input { get => _input; set => _input = value; }

        void WhenLocomotionCoreChange(FieldEventType type, LocomotionCore ov, LocomotionCore nv)
        {
            if (type == FieldEventType.Reading)
                return;
            _locomotionCore = nv;
        }
    }
}
