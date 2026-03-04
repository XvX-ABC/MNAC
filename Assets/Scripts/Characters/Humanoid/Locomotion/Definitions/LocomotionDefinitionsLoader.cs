using MNAC.Characters.Humanoid.Locomotion.Animations;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    public class LocomotionDefinitionsLoader : MonoBehaviour, ILocomotionDefinitions
    {
        [SerializeField]
        LocomotionDefinitions_SO _definitions;

        public IWalkingDefinitions Walking => _definitions.Walking;

        public IBoostingDefinitions Boosting => _definitions.Boosting;

        public IJumpDefinitions Jump => _definitions.Jump;

        public IQuickBoostingDefinitions QuickBoosting => _definitions.QuickBoosting;

        public ushort PostureEvaluationFramesAmount => _definitions.PostureEvaluationFramesAmount;

        public ILocomotionAnimatorDefinitions Animation => ((ILocomotionDefinitions)_definitions).Animation;

        public IMutativeDragDefinitions MutativeDrag => ((ILocomotionDefinitions)_definitions).MutativeDrag;
    }
}
