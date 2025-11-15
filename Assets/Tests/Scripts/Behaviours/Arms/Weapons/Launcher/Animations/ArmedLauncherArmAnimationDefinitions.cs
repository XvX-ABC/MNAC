using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
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
        public RuntimeAnimatorController Animator => _animator;

        public string Velocity_X => _velocity_x;

        public string Velocity_Y => _velocity_y;

        public string Aiming => _aiming;

        public float ReloadClipLength => _reloadClipLength;

        public string ReloadTrigger => _reloadTrigger;

        public string ReloadMultiplier => _reloadMultiplier;
    }
}
