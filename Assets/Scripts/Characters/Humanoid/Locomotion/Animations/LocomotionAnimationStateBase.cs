using System;
using MNAC.States;
using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion.Animations
{
    internal abstract class LocomotionAnimationStateBase : WithCallbackPlayableState<object>
    {
        protected LocomotionAnimationStateBase(string name, float duration, bool enabled = true) : base(name == null ? "locomotion_animation_state" : $"locomotion_animation_state_{name}", duration, enabled)
        {

        }
        protected new LocomotionAnimationStateContext context => (LocomotionAnimationStateContext)base.context;
    }
}
