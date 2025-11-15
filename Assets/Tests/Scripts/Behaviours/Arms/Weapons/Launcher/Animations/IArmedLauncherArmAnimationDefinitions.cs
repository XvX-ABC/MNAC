using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal interface IArmedLauncherArmAnimationDefinitions
    {
        RuntimeAnimatorController Animator { get; }
        string Velocity_X { get; }
        string Velocity_Y { get; }
        string Aiming { get; }
        float ReloadClipLength { get; }
        string ReloadTrigger { get; }
        string ReloadMultiplier { get; }
    }
}
