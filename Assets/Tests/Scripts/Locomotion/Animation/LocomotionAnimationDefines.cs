using UnityEngine;

namespace Tests.Locomotion.Animation
{
    class LocomotionAnimationDefines : MonoBehaviour, ILocomotionAnimationDefines
    {
        [SerializeField]
        HorizontalLocomotionAnimationDefines _horizontal;
        [SerializeField]
        JumpLocomotionAnimationDefines _jump;
        public IHorizontalLocomotionAnimationDefines Horizontal => _horizontal;
        public IJumpLocomotionAnimationDefines Jump => _jump;
    }
}