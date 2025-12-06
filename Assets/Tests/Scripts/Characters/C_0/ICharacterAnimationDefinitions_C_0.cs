using System;
using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal interface ICharacterAnimationDefinitions_C_0
    {
        IStunningAnimationDefinitions Stunning { get; }
        IDeathAnimationDefinitions Death { get; }
    }
    [Serializable]
    internal class C_0AnimationDefinitions : ICharacterAnimationDefinitions_C_0
    {
        [SerializeField]
        StunningAnimationDefinitions _stunning;
        [SerializeField]
        DeathAnimationDefinitions _death;
        public IStunningAnimationDefinitions Stunning => _stunning;

        public IDeathAnimationDefinitions Death => _death;
    }
}
