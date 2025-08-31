using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.Extensions;
using Tests.Weapons;
using UnityEngine.Playables;
using IAnimationPlayablePart = Tests.Behaviours.Animations.IAnimationPlayablePart;

namespace Tests.Behaviours.Arms.Weapons.Animations
{
    [Obsolete]
    internal class ArmedWeaponArmAnimator_Obsolete
    {
        Dictionary<string, IArmedWeaponArmAnimationPlayablePart> _animators;
        IArmedWeaponArmAnimationPlayablePart[] _activatedAnimators;
        ArmedWeaponArmBehavioursController_Obsolete _controller;
        internal ArmedWeaponPlayablePart playablePart;
        Action<bool, IAnimationPlayablePart> _stateAction;

        internal Action<bool, IAnimationPlayablePart> stateAction { get => _stateAction; set => _stateAction = value; }
        //internal class ArmedWeaponPlayablePart : AnimationPlayablePartBase
        //{
        //    IArmedWeaponArmAnimator_Obsolete _animator;

        //    internal IArmedWeaponArmAnimator_Obsolete animator
        //    {
        //        get => _animator;
        //        set
        //        {

        //            _animator = value;
        //        }
        //    }
        //    public override bool Enabled => _animator.Enabled;
        //    public override IOutputSetting OutputSetting { get => _animator.OutputSetting; set => _animator.OutputSetting = value; }
        //    public ArmedWeaponPlayablePart()
        //    {
        //    }
        //    public override bool Initialize(PlayableGraph graph)
        //    {
        //        playablePart = _animator.GetPlayablePart(graph);
        //        return true;
        //    }
        //    public override void Dispose()
        //    {
        //    }
        //}
        public ArmedWeaponArmAnimator_Obsolete(ArmedWeaponArmBehavioursController_Obsolete controller)
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
            playablePart = new();
        }
        void ActivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour_Obsolete behaviour)
        {
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                if (_activatedAnimators == null)
                    _activatedAnimators = new IArmedWeaponArmAnimationPlayablePart[] { animator };
                else
                    //_activatedAnimators.Append(animator);
                    ArrayExtensions.Append(ref _activatedAnimators, animator);
                playablePart.animator = animator;
            }
        }
        void UnactivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour_Obsolete behaviour)
        {
            if (_activatedAnimators == null)
                return;
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                animator.OutputSetting = null;
                if (_activatedAnimators.Length == 1)
                    _activatedAnimators = null;
                else
                    _activatedAnimators.Remove(animator);
                playablePart.animator = null;
            }
        }
        public void OnUpdate()
        {
            if (_activatedAnimators == null)
                return;

        }
    }
}
