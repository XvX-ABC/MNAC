using System;
using System.Linq;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher.Animations
{
    [Serializable]
    internal class ArmedLauncherArmAnimationDefinitions : IArmedLauncherArmAnimationDefinitions
    {
        [SerializeField]
        RuntimeAnimatorController _animator;
        [SerializeField]
        string _velocity_x;
        [SerializeField]
        string _velocity_y;
        [SerializeField]
        string _aiming;
        [SerializeField]
        float _reloadClipLength;
        [SerializeField]
        string _reloadTrigger;
        [SerializeField]
        string _reloadMultiplier;
        [SerializeField]
        string _mirrorSwitch;
        [SerializeField]
        StateTransitionOptions[] _transitionOptions;
        public RuntimeAnimatorController Animator => _animator;

        public string Velocity_X => _velocity_x;

        public string Velocity_Y => _velocity_y;

        public string Aiming => _aiming;

        public float ReloadClipLength => _reloadClipLength;

        public string ReloadTrigger => _reloadTrigger;

        public string ReloadMultiplier => _reloadMultiplier;

        public string MirrorSwitch { get => _mirrorSwitch; }

        public StateTransitionOptions GetStateTransitionOption(IArmedLauncherArmAnimationDefinitions.Transition transition)
        {
            return _transitionOptions.FirstOrDefault(t => t.Transition == transition);
        }
    }
}
