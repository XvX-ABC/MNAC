using Locomotion;
using Tests.TPhysics.Locomotion;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class JumpLocomotionState : LocomotionStateBase
    {
        internal JumpLocomotion locomotion;
        public JumpLocomotionState(IJumpDefinitions definitions, bool enabled = true) : base("jump", 0, enabled)
        {
            locomotion = new(definitions);
            timeline = locomotion.timeline;
        }

        protected override ILocomotionModule module => locomotion;
        public override void OnEnter()
        {
            context.Core.EnableModule(locomotion);
        }
        public override void OnExit()
        {
            context.Core.DisableModule(locomotion);
        }
    }
}
