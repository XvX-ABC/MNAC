using System;
using Tests.Behaviours.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Characters
{
    internal class ControllerPlayable : AnimationPlayablePartBase
    {
        Animator _animator;
        AnimatorControllerPlayable controller;
        public override IOutputSetting OutputSetting
        {
            get => base.OutputSetting;
            set
            {
                base.OutputSetting = value;
                outputSetting.Weight = 1;
            }
        }
        public ControllerPlayable(Animator animator)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public override bool Initialize(PlayableGraph graph)
        {
            controller = AnimatorControllerPlayable.Create(graph, _animator.runtimeAnimatorController);
            playablePart = controller;
            return true;
        }
        public void SetFloat(string name, float value)
        {
            if (playablePart.IsNull())
                throw new NullReferenceException(nameof(playablePart));
            controller.SetFloat(name, value);
        }
        public void SetBool(string name, bool value)
        {
            if (playablePart.IsNull())
                throw new NullReferenceException(nameof(playablePart));
            controller.SetBool(name, value);
        }
        public void SetTrigger(string name)
        {
            if (playablePart.IsNull())
                throw new NullReferenceException(nameof(playablePart));
            controller.SetTrigger(name);
        }
    }
}
