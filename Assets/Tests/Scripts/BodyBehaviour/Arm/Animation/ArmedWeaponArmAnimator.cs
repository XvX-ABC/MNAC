using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.Character;
using Tests.Extensions;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static Tests.BodyBehaviour.Arm.Animations.ArmedWeaponArmAnimator;
using static UnityEngine.Rendering.DebugUI;
using IAnimationPlayablePart = Tests.Character.IAnimationPlayablePart;

namespace Tests.BodyBehaviour.Arm.Animations
{
    internal class ArmedWeaponArmAnimator_New
    {
        Dictionary<string, IArmedWeaponArmAnimator> _animators;
        IArmedWeaponArmAnimator[] _activatedAnimators;
        ArmedWeaponArmBehavioursController _controller;
        internal ArmedWeaponPlayablePart playablePart;
        Action<bool, IAnimationPlayablePart> _stateAction;

        internal Action<bool, IAnimationPlayablePart> stateAction { get => _stateAction; set => _stateAction = value; }
        internal class ArmedWeaponPlayablePart : AnimationPlayablePartBase
        {
            IArmedWeaponArmAnimator _animator;

            internal IArmedWeaponArmAnimator animator
            {
                get => _animator;
                set
                {

                    _animator = value;
                }
            }
            public override bool Enabled => _animator.Enabled;
            public override IOutputSetting OutputSetting { get => _animator.OutputSetting; set => _animator.OutputSetting = value; }
            public ArmedWeaponPlayablePart()
            {
            }
            public override bool Initialize(PlayableGraph graph)
            {
                playablePart = _animator.GetPlayablePart(graph);
                return true;
            }
            public override void Dispose()
            {
            }
        }
        public ArmedWeaponArmAnimator_New(ArmedWeaponArmBehavioursController controller)
        {
            _animators = new();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            foreach (var kv in _controller.weaponBehavioursMapping)
            {
                var name = kv.Key;
                var animator = kv.Value.Animator;
                _animators.Add(name, animator);
            }
            _controller.ActivatedAction += ActivatedAnimator;
            _controller.UnactivatedAction += UnactivatedAnimator;
            playablePart = new();
        }
        void ActivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour behaviour)
        {
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                if (_activatedAnimators == null)
                    _activatedAnimators = new IArmedWeaponArmAnimator[] { animator };
                else
                    _activatedAnimators.Append(animator);
                playablePart.animator = animator;
            }
        }
        void UnactivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour behaviour)
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
            //for (int i = 0; i < _activatedAnimators.Length; i++)
            //{
            //    var a = _activatedAnimators[i];
            //    var oldState = a.OldState;
            //    var currentState = a.Animator.State;
            //    _activatedAnimators[i].OldState = currentState;
            //    if (oldState != currentState)
            //    {
            //        playablePart.animator = a.Animator;
            //        _stateAction?.Invoke(currentState > 0, playablePart);
            //        break;
            //    }
            //}
        }
    }
    internal class ArmedWeaponArmAnimator : IDynamicPlayablePart
    {
        Dictionary<string, IArmedWeaponArmAnimator> _animators;
        IArmedWeaponArmAnimator[] _activatedAnimators;
        ArmedWeaponArmBehavioursController _controller;
        //AnimationMixerPlayable playablePart;
        PlayableGraph _graph;
        Action<Playable> _playablePartUpdateAction;

        IOutputSetting _outputSetting;
        //public Playable PlayablePart
        //{
        //    get => playablePart;
        //}
        public bool Enabled
        {
            get => _controller.Enabled;
            set => _controller.Enabled = value;
        }
        public IOutputSetting OutputSetting
        {
            get => _outputSetting;
            set => _outputSetting = value;
        }
        public Action<Playable> UpdateAction { get => _playablePartUpdateAction; set => _playablePartUpdateAction = value; }

        public ArmedWeaponArmAnimator(ArmedWeaponArmBehavioursController controller)
        {
            _animators = new();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            foreach (var kv in _controller.weaponBehavioursMapping)
            {
                var name = kv.Key;
                var animator = kv.Value.Animator;
                _animators.Add(name, animator);
            }
            _controller.ActivatedAction += ActivatedAnimator;
            _controller.UnactivatedAction += UnactivatedAnimator;

        }
        void InitializePlayablePart(PlayableGraph graph)
        {
            //if (playablePart.IsNull())
            //{
            //    var count = _controller.behaviours.Length;

            //    playablePart = AnimationMixerPlayable.Create(graph, count);
            //    var idx = 0;
            //    foreach (var b in _controller.behaviours)
            //    {
            //        var animator = b.Animator;
            //        var p = animator.GetPlayablePart(graph);
            //        var outputSetting = new OutputSetting(playablePart, idx);
            //        animator.OutputSetting = outputSetting;

            //        graph.Connect(p, 0, playablePart, idx);
            //        playablePart.SetInputWeight(idx++, 0);
            //    }
            //}
            _graph = graph;
        }
        void ActivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour behaviour)
        {
            var name = weapon.Name;
            if (_animators.TryGetValue(name, out var animator))
            {
                if (_activatedAnimators == null)
                    _activatedAnimators = new IArmedWeaponArmAnimator[] { animator };
                else
                    _activatedAnimators.Append(animator);
            }
        }
        void UnactivatedAnimator(IWeapon weapon, IArmedWeaponArmBehaviour behaviour)
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
                {
                    _activatedAnimators.Remove(animator);
                }
            }
        }
        public Playable GetPlayablePart(PlayableGraph graph)
        {
            //if (playablePart.IsNull())
            //    InitializePlayablePart(graph);
            //return playablePart;
            _graph = graph;
            return Playable.Null;
        }
        Playable CreatePlayablePart(PlayableGraph graph)
        {
            //var w = _activatedAnimators[0];
            //w.Animator.OutputSetting = _outputSetting;
            //return w.Animator.GetPlayablePart(graph);
            return default;
        }
        public void OnUpdate()
        {
            //if (_activatedAnimators == null)
            //    return;
            //foreach (var a in _activatedAnimators)
            //{
            //    var oldState = a.OldState;
            //    var currentState = a.Animator.State;
            //    if (oldState != currentState)
            //    {
            //        var p = Playable.Null;
            //        if (currentState > 0)
            //        {
            //            p = CreatePlayablePart(_graph);
            //        }
            //        _playablePartUpdateAction?.Invoke(p);
            //        break;
            //    }
            //}
        }
    }
}
