using Assets.Tests.Scripts.BodyBehaviour.Arm.Animation;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Characters;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.BodyBehaviour.Arm.Animations
{
    internal class ArmWeaponAnimationCore
    {
        GameObject _armObj;
        Animator _animator;
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;
        AnimationMixerPlayable _playable;



        ArmWeaponHoldingBehavioursAnimator _holdingBehavioursAnimator;
        public float SwitchingWeight
        {
            get => _playable.GetInputWeight(0);
            set
            {
                var v = Mathf.Clamp01(value);
                _playable.SetInputWeight(0, v);
                _playable.SetInputWeight(1, 1 - v);
            }
        }
        public ArmWeaponAnimationCore(ArmCore core, PlayableGraph graph)
        {
            _armObj = core.gameObject;
            _definitions = core.definitions.Weapon;
            _animationDefinitions = core.animationDefinitions.Weapon;
            _animator = _armObj.GetComponent<Animator>();
            _holdingBehavioursAnimator = new(core.holdingBehaviours, graph);

            InitializePlayableGraph(graph);
        }
        void InitializePlayableGraph(PlayableGraph graph)
        {
            _playable = AnimationMixerPlayable.Create(graph, 2);
            var clip = AnimationClipPlayable.Create(graph, _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip"));
            clip.SetDuration(_definitions.SwitchingDurationTime);
            graph.Connect(clip, 0, _playable, 0);
            graph.Connect(_holdingBehavioursAnimator.playablePart, 0, _playable, 1);
        }
    }
}
