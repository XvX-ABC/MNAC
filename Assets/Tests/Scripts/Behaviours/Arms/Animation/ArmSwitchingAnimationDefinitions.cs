using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Animations
{
    [Serializable]
    public class ArmSwitchingAnimationDefinitions : IArmWeaponSwitchingAnimationDefinitions
    {
        [SerializeField]
        AnimationClip _clip;
        public AnimationClip Clip { get => _clip; }
    }
}
