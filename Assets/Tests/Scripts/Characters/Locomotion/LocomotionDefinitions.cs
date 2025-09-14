using Locomotion;
using Tests.Characters.Locomotion.Animations;
using UnityEngine;

namespace Tests.Characters.Locomotion
{
    public class LocomotionDefinitions : LocomotionDefinitionsBase, ILocomotionDefinitions
    {
        [SerializeField]
        ushort _postureEvaluationFramesQuantity;
        [SerializeField]
        WalkingDefinitions _walking;
        [SerializeField]
        BoostingDefinitions _boosting;
        [SerializeField]
        QuickBoostingDefinitions _quickBoosting;
        [SerializeField]
        LocomotionAnimatorDefinitions _animation;
        public ushort PostureEvaluationFramesQuantity => _postureEvaluationFramesQuantity;

        public IWalkingDefinitions Walking => _walking;

        public IQuickBoostingDefinitions QuickBoosting => _quickBoosting;

        public ILocomotionAnimatorDefinitions Animation => _animation;

        IBoostingDefinitions ILocomotionDefinitions.Boosting => _boosting;
    }
}
