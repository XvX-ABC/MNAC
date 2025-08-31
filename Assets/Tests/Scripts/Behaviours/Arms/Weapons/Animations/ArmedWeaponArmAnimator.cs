using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Extensions;
using Tests.Weapons;
using IAnimationPlayablePart = Tests.Behaviours.Animations.IAnimationPlayablePart;

namespace Tests.Behaviours.Arms.Weapons.Animations
{

    internal class ArmedWeaponArmAnimator<T> : IArmedWeaponArmAnimator where T : IArmedWeaponArmBehaviour
    {
        Dictionary<string, IArmedWeaponArmAnimationPlayablePart> _animators;
        IArmedWeaponArmAnimationPlayablePart[] _activatedAnimators;
        IArmedWeaponArmBehavioursController<T> _controller;
        ArmedWeaponPlayablePart _playablePart;
        Action<bool, IAnimationPlayablePart> _stateAction;

        internal Action<bool, IAnimationPlayablePart> stateAction { get => _stateAction; set => _stateAction = value; }
        public ArmedWeaponPlayablePart PlayablePart { get => _playablePart; set => _playablePart = value; }

        public ArmedWeaponArmAnimator(IArmedWeaponArmBehavioursController<T> controller)
        {
            _animators = new();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            foreach (var kv in _controller.Behaviours)
            {
                var name = kv.Key;
                var animator = kv.Value.Animator;
                _animators.Add(name, animator);
            }
            _controller.ActivatedAction += ActivatedAnimator;
            _controller.UnactivatedAction += UnactivatedAnimator;
            _playablePart = new();
        }
        void ActivatedAnimator(IWeapon weapon, T behaviour)
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
        }
        void UnactivatedAnimator(IWeapon weapon, T behaviour)
        {
            if (_activatedAnimators == null)
                return;
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                if (_activatedAnimators.Length == 1)
                    _activatedAnimators = null;
                else
                    _activatedAnimators.Remove(animator);
                _playablePart.animator = null;
            }
        }
        public void OnUpdate()
        {
            if (_activatedAnimators == null)
                return;

        }
    }
}
