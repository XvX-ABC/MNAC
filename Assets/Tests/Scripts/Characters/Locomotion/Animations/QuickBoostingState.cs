using System;
using Tests.States;
using UnityEngine;
using UnityEngine.Animations;
using Utilities.Timeline;

namespace Tests.Characters.Locomotion.Animations
{
    //TODO: quick boostring 和 jump 可以向上提取一个共同基类
    internal class QuickBoostingState : MovementState
    {
        protected ILocomotionAnimatorDefinitions definitions;
        protected ControllerPlayable controller;
        public QuickBoostingState(string name, ITimeline quickBoostingTimeline, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, MovementAnimator animator, bool enabled = true) : base(name == null ? "quick_boosting" : $"quick_boosting_{name}", 0, animator, enabled)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.timeline = quickBoostingTimeline ?? throw new ArgumentNullException(nameof(quickBoostingTimeline));
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            var clipLength = definitions.Boosting_Clip_Length;
            var m = clipLength / (timeline.Length <= 0 ? 1 : timeline.Length);
            controller.SetFloat(definitions.Boosting_Multiplier, m);
            controller.SetTrigger(definitions.Boosting_Trigger);
            base.FromPreviousStateTransitionBegin(currentTransition);
        }
    }
}
