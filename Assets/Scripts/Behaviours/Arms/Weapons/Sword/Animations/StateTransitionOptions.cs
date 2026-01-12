using BehaviorDesigner.Runtime.Tasks;
using System;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    [Serializable]
    public class StateTransitionOptions : BlendingTransitionOptions
    {
        [SerializeField]
        IArmedSwordArmAnimationDefinitions.Transition _transition;

        public IArmedSwordArmAnimationDefinitions.Transition Transition { get => _transition; set => _transition = value; }
    }
}
