using Locomotion;
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
        public ushort PostureEvaluationFramesQuantity => _postureEvaluationFramesQuantity;

        public IWalkingDefinitions Walking => _walking;

        public IQuickBoostingDefinitions QuickBoosting => _quickBoosting;

        IBoostingDefinitions ILocomotionDefinitions.Boosting => _boosting;
    }
}
