using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal interface IArmedLauncherArmAnimationDefinitions
    {
        public enum Transition
        {
            Idle_Aiming,
            Aiming_Reload,
            Reload_Aiming,

        }
        RuntimeAnimatorController Animator { get; }
        string Velocity_X { get; }
        string Velocity_Y { get; }
        string Aiming { get; }
        float ReloadClipLength { get; }
        string ReloadTrigger { get; }
        string ReloadMultiplier { get; }

        public StateTransitionOptions GetStateTransitionOption(Transition transition);
    }
}
