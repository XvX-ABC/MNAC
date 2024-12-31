using UnityEngine;

namespace Tests.Locomotion.Animation
{
    class LocomotionAnimationDefines : MonoBehaviour, ILocomotionAnimationDefines
    {
        [SerializeField]
        HorizontalLocomotionAnimationDefines _horizontal;
        [SerializeField]
        JumpLocomotionAnimationDefines _jump;
        [SerializeField]
        AirLocomotionAnimationDefines _air;
        public IHorizontalLocomotionAnimationDefines Horizontal => _horizontal;
        public IJumpLocomotionAnimationDefines Jump => _jump;

        public IAirLocomotionAnimationDefines Air => _air;
    }
}