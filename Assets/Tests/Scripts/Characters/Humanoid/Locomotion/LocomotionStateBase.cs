using System;
using Tests.States;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using UnityEngine;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{
    internal abstract class LocomotionStateBase : WithCallbackPlayableState<object>
    {


        public LocomotionStateBase(string name, float duration = 0, bool enabled = true) : base($"locomotion_{name}", duration, enabled)
        {
        }
        protected override ITimeline NewTimeline(float duration)
        {
            return new Timeline_V1(duration);
        }
        protected new LocomotionStateContext context { get => (LocomotionStateContext)base.context; }
        protected abstract ILocomotionModule module { get; }
        public override object Context
        {
            get => base.Context;
            set
            {
                var core = default(Core);
                if (base.Context != null)
                {
                    core = (base.Context as LocomotionStateContext).Core;
                    core.RemoveModule(module);
                }

                if (value == null)
                    throw new NullReferenceException(nameof(value));
                core = (value as LocomotionStateContext).Core;
                core.AddModule(module);

                base.Context = value;

            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();
            context.Core.EnableModule(module);
        }
        public override void OnUpdate()
        {
            timeline.OnUpdate(Time.deltaTime);
            base.OnUpdate();
        }
        public override void OnExit()
        {
            context.Core.DisableModule(module);
            timeline.Pause();
            base.OnExit();
        }
    }
}
