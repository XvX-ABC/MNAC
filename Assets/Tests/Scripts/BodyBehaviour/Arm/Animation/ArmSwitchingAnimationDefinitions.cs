using System;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animations
{
    [Serializable]
    public class ArmSwitchingAnimationDefinitions : IArmWeaponSwitchingAnimationDefinitions
    {
        [SerializeField]
        AnimationClip _clip;
        public AnimationClip Clip { get => _clip; }
    }
}
