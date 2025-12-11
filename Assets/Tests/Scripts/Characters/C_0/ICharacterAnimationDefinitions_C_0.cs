using System;
using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Animations;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal interface ICharacterAnimationDefinitions_C_0
    {
        IHumanAnimationDefinitions HumanoidDefinitions { get; }
        IStunningAnimationDefinitions Stunning { get; }
        IDeathAnimationDefinitions Death { get; }
    }
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
