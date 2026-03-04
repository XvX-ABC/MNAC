using System;
using MNAC.Characters.Animations;
using MNAC.Characters.Humanoid.Animations;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    [Serializable]
    internal class C_0AnimationDefinitions : ICharacterAnimationDefinitions_C_0
    {
        [SerializeField]
        HumanoidAnimationDefinitions _humanoidDefinitions;
        [SerializeField]
        StunningAnimationDefinitions _stunning;
        [SerializeField]
        DeathAnimationDefinitions _death;
        public IStunningAnimationDefinitions Stunning => _stunning;

        public IDeathAnimationDefinitions Death => _death;

        public IHumanAnimationDefinitions HumanoidDefinitions => _humanoidDefinitions;
    }
}
