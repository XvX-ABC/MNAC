using BehaviorDesigner.Runtime.Tasks;
using System;
using MNAC.Behaviours.Arms.Weapons.Sword.Animations;
using MNAC.States;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Sword
{
    [Serializable]
    public class StateTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        IArmedSwordArmAnimationDefinitions.Transition _transition;

        public IArmedSwordArmAnimationDefinitions.Transition Transition { get => _transition; set => _transition = value; }
    }
}
