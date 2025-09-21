using System;
using Tests.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.Characters
{
    internal class ControllerPlayable : AnimationPlayablePartBase
    {
        [Obsolete]
        Animator _animator;
        RuntimeAnimatorController _controller;
        AnimatorControllerPlayable controller;
        public override IOutputSetting OutputSetting
        {
            get => base.OutputSetting;
            set
            {
                base.OutputSetting = value;
            }
        }
        public ControllerPlayable(PlayableGraph graph, Animator animator) : this(graph, animator.runtimeAnimatorController)
        {
        }
        public ControllerPlayable(PlayableGraph graph, RuntimeAnimatorController controller) : base(graph)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.controller = AnimatorControllerPlayable.Create(graph, _controller);
            playablePart = this.controller;
        }
        public float GetFloat(string name)
        {
            if (playablePart.IsNull())
                throw new NullReferenceException(nameof(playablePart));
            return controller.GetFloat(name);
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
