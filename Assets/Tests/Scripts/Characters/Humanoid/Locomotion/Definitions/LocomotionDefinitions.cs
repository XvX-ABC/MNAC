using Locomotion;
using System;
using Tests.Characters.Humanoid.Locomotion.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    [Serializable]
    internal class LocomotionDefinitions : ILocomotionDefinitions
    {
        [SerializeField]
        ushort _postureEvaluationFramesAmount;
        [SerializeField]
        WalkingDefinitions _walking;
        [SerializeField]
        BoostingDefinitions _boosting;
        [SerializeField]
        QuickBoostingDefinitions _quickBoosting;
        [SerializeField]
        JumpDefinitions _jump;
        [SerializeField]
        LocomotionAnimatorDefinitions _animation;
        public ushort PostureEvaluationFramesAmount => _postureEvaluationFramesAmount;

        public IWalkingDefinitions Walking => _walking;

        public IQuickBoostingDefinitions QuickBoosting => _quickBoosting;

        public ILocomotionAnimatorDefinitions Animation => _animation;

        public IJumpDefinitions Jump => _jump;

        public IBoostingDefinitions Boosting => _boosting;
    }
}
