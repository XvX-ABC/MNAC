using System;
using Tests.BodyBehaviour.Arm.Animations;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animations
{
    [Serializable]
    public class ArmWeaponAnimationDefinitions : IArmWeaponAnimationDefinitions
    {
        [SerializeField]
        ArmSwitchingAnimationDefinitions _switching;

        public IArmWeaponSwitchingAnimationDefinitions Switching => _switching;
    }
}
