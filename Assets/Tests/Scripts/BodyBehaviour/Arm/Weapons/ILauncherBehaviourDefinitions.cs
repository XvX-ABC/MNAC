using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public interface ILauncherBehaviourDefinitions
    {
        public AnimationClip ReloadClip { get; }
        public float IdleAndAimTransitionLength { get; }
        public float AimAndReloadTransitionLength { get; }
    }
}
