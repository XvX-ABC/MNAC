using System;
using Tests.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmControllerPlayable : ControllerPlayable
    {
        string _velocityName_x;
        string _velocityName_y;

        public ArmControllerPlayable(PlayableGraph graph, string velocityName_x, string velocityName_y, Animator animator) : base(graph, animator)
        {
            _velocityName_x = velocityName_x ?? throw new ArgumentNullException(nameof(velocityName_x));
            _velocityName_y = velocityName_y ?? throw new ArgumentNullException(nameof(velocityName_y));
        }

        public ArmControllerPlayable(PlayableGraph graph, string velocityName_x, string velocityName_y, RuntimeAnimatorController controller) : base(graph, controller)
        {
            _velocityName_x = velocityName_x ?? throw new ArgumentNullException(nameof(velocityName_x));
            _velocityName_y = velocityName_y ?? throw new ArgumentNullException(nameof(velocityName_y));
        }
        public void SetVelocity(Vector2 velocity)
        {
            this.SetFloat(_velocityName_x, velocity.x);
            this.SetFloat(_velocityName_y, velocity.y);
        }
    }
}
