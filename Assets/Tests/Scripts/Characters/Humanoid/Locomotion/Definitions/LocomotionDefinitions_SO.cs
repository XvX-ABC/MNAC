using Tests.Characters.Humanoid.Locomotion.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    [CreateAssetMenu(fileName = "LocomotionDefinitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/LocomotionDefinitions")]
    internal class LocomotionDefinitions_SO : ScriptableObject, ILocomotionDefinitions
    {
        [SerializeField]
        LocomotionDefinitions _definitions;

        public IWalkingDefinitions Walking => _definitions.Walking;

        public IBoostingDefinitions Boosting => _definitions.Boosting;

        public IJumpDefinitions Jump => _definitions.Jump;

        public IQuickBoostingDefinitions QuickBoosting => _definitions.QuickBoosting;

        public ushort PostureEvaluationFramesAmount => _definitions.PostureEvaluationFramesAmount;

        public ILocomotionAnimatorDefinitions Animation => _definitions.Animation;

        public IMutativeDragDefinitions MutativeDrag => ((ILocomotionDefinitions)_definitions).MutativeDrag;
    }
}
