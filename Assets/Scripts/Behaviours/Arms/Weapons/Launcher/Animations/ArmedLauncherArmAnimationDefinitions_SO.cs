using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher.Animations
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmAnimationDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Launchers/ArmedLauncherArmAnimationDefinitions")]
    internal class ArmedLauncherArmAnimationDefinitions_SO : ScriptableObject, IArmedLauncherArmAnimationDefinitions
    {
        [SerializeField]
        ArmedLauncherArmAnimationDefinitions _definitions;

        public RuntimeAnimatorController Animator => _definitions.Animator;

        public string Velocity_X => _definitions.Velocity_X;

        public string Velocity_Y => _definitions.Velocity_Y;

        public string Aiming => _definitions.Aiming;

        public float ReloadClipLength => _definitions.ReloadClipLength;

        public string ReloadTrigger => _definitions.ReloadTrigger;

        public string ReloadMultiplier => _definitions.ReloadMultiplier;

        public string MirrorSwitch => ((IArmedLauncherArmAnimationDefinitions)_definitions).MirrorSwitch;

        public StateTransitionOptions GetStateTransitionOption(IArmedLauncherArmAnimationDefinitions.Transition transition)
        {
            return _definitions.GetStateTransitionOption(transition);
        }
    }
}
