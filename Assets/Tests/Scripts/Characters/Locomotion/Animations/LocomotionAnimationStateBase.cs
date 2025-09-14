using System;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using UnityEngine;

namespace Tests.Characters.Locomotion.Animations
{
    internal abstract class LocomotionAnimationStateBase : WithCallbackPlayableState<LocomotionAnimationStateContext>
    {
        protected ILocomotionAnimatorDefinitions definitions;
        protected ControllerPlayable controller;
        protected World world;
        protected Rigidbody rbody;
        protected IGroundDetector groundDetector;
        protected LocomotionAnimationStateBase(string name, float duration, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector ground, bool enabled = true) : base(name == null ? "locomotion_animation_state" : $"locomotion_animation_state_{name}", duration, enabled)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.world = world ?? throw new ArgumentNullException(nameof(world));
            this.rbody = rigidbody ?? throw new ArgumentNullException(nameof(rigidbody));
            this.groundDetector = groundDetector ?? throw new ArgumentNullException(nameof(groundDetector));
        }
        protected LocomotionAnimationStateBase(string name, float duration, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, Rigidbody rigidbody, IGroundDetector ground, bool enabled = true) : this(name, duration, definitions, controller, World.Default, rigidbody, ground, enabled)
        {
        }
    }
}
