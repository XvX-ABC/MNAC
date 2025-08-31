using System;
using Tests.States;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Locomotion
{
    internal abstract class LocomotionStateBase : WithCallbackPlayableState<LocomotionStateContext>
    {


        public LocomotionStateBase(string name, float duration = 0, bool enabled = true) : base($"locomotion_{name}", duration, enabled)
        {

        }
        protected abstract ILocomotionModule module { get; }
        public override LocomotionStateContext Context
        {
            get => base.Context;
            set
            {
                var core = default(Core);
                if (base.Context != null)
                {
                    core = base.Context.Core;
                    core.RemoveModule(this.module);
                }

                if (value == null)
                    throw new NullReferenceException(nameof(value));
                core = value.Core;
                core.AddModule(this.module);

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
            timeline.End();
            base.OnExit();
        }
    }
}
