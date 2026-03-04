using System;
using MNAC.States;
using UnityEngine;

namespace MNAC.Characters.C_0
{
    [Serializable]
    internal class BehaviourTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        internal BehavioursTransition transition;
    }
}
