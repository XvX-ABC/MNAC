using System;
using System.Collections.Generic;
using Tests.Extensions;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;
using UnityEngine.Playables;
using IAnimationPlayablePart = Tests.Animations.IAnimationPlayablePart;

namespace Tests.Behaviours.Arms.Weapons.Animations
{

    internal class ArmedWeaponArmAnimator<T> : IArmedWeaponArmAnimator where T : IArmedWeaponArmBehaviour
    {
        Dictionary<string, IArmedWeaponArmAnimationPlayablePart> _animators;
        IArmedWeaponArmAnimationPlayablePart[] _activatedAnimators;
        IArmedWeaponArmAnimationPlayablePart _activatedAnimator;
        IArmedWeaponArmBehavioursController<T> _controller;
        ArmedWeaponPlayablePart _playablePart;
        Action<bool, IAnimationPlayablePart> _stateAction;
        [Obsolete]
        internal Action<bool, IAnimationPlayablePart> stateAction { get => _stateAction; set => _stateAction = value; }
        public ArmedWeaponPlayablePart PlayablePart { get => _playablePart; /*set => _playablePart = value;*/ }

        public ArmedWeaponArmAnimator(PlayableGraph graph, IArmedWeaponArmBehavioursController<T> controller)
        {
            _animators = new();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _controller.ActivatedAction += ActivatedAnimator;
            _controller.UnactivatedAction += UnactivatedAnimator;
            _playablePart = new(graph);
        }
        [Obsolete]
        void ActivatedAnimator_Obsolete(IWeapon weapon, T behaviour)
        {
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                if (_activatedAnimators == null)
                    _activatedAnimators = new IArmedWeaponArmAnimationPlayablePart[] { animator };
                else
                    ArrayExtensions.Append(ref _activatedAnimators, animator);
                _playablePart.animator = animator;
            }
            else
            {
                Debug.LogWarning($"No animator found for weapon '{name}'");
            }
        }
        void ActivatedAnimator(IWeapon weapon, T behaviour)
        {
            _activatedAnimator = behaviour.Animator;
            _playablePart.animator = _activatedAnimator;
        }
        void UnactivatedAnimator(IWeapon weapon, T behaviour)
        {
            if (behaviour.Animator != _activatedAnimator)
                return;
            _activatedAnimator = null;
            _playablePart.animator = null;
        }
        public void OnUpdate()
        {
            if (_activatedAnimators == null)
                return;

        }
    }
}
