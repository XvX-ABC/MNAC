using UnityEngine;

namespace Tests.Locomotion.Animation
{
public   class LocomotionAnimationDefinitions : MonoBehaviour, ILocomotionAnimationDefinitions
    {
        [SerializeField]
        GroundLocomotionAnimatorDefinitions _ground;
        [SerializeField]
        JumpLocomotionAnimationDefinitions _jump;
        [SerializeField]
        AirLocomotionAnimationDefinitions _air;
        [SerializeField]
        BoostingLocomotionAnimatorDefinitions _boosting;
        public IGroundLocomotionAnimatorDefinitions Ground => _ground;
        public IJumpLocomotionAnimationDefinitions Jump => _jump;

        public IAirLocomotionAnimationDefinitions Air => _air;
        public IBoostingLocomotionAnimatorDefinitions Boosting => _boosting;
    }
}