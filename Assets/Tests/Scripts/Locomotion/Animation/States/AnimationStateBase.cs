using Tests.States;
using UnityEngine;

namespace Tests.Locomotion.Animation.States
{
    public class AnimationStateBase : AnimationStateBase<Context>
    {
        public AnimationStateBase(string name, Animator animator) : base(name, animator)
        {
        }
    }
}
