using UnityEngine;

namespace Tests.Locomotion.Animation
{
    class LocomotionAnimationDefinitions : MonoBehaviour, ILocomotionAnimationDefinitions
    {
        [SerializeField]
        HorizontalLocomotionAnimationDefinitions _horizontal;
        [SerializeField]
        JumpLocomotionAnimationDefinitions _jump;
        [SerializeField]
        AirLocomotionAnimationDefinitions _air;
        [SerializeField]
        BoostingLocomotionAnimatorDefinitions _boosting;
        public IHorizontalLocomotionAnimationDefinitions Horizontal => _horizontal;
        public IJumpLocomotionAnimationDefinitions Jump => _jump;

        public IAirLocomotionAnimationDefinitions Air => _air;
        public IBoostingLocomotionAnimatorDefinitions Boosting => _boosting;
    }
}