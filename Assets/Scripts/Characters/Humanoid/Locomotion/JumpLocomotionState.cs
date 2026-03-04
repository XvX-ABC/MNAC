using System;
using MNAC.TPhysics.Locomotion;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    internal class JumpLocomotionState : LocomotionStateBase
    {
        internal JumpLocomotion locomotion;
        public float Height
        {
            get => locomotion.Height;
            set => locomotion.Height = value;
        }
        public JumpLocomotionState(float height, bool enable = true) : base("jump", 0, enable)
        {
            locomotion = new(height);
            timeline = locomotion.Timeline;
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
