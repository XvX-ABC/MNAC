using System;
using MNAC.States;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher.Animations
{
    [Serializable]
    public class StateTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        IArmedLauncherArmAnimationDefinitions.Transition _transition;

        internal IArmedLauncherArmAnimationDefinitions.Transition Transition { get => _transition; set => _transition = value; }
    }
}
