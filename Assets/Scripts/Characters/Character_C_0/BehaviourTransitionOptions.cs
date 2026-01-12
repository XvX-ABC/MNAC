using System;
using Tests.States;
using UnityEngine;

namespace Tests.Characters.C_0
{
    [Serializable]
    internal class BehaviourTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        internal BehavioursTransition transition;
    }
}
