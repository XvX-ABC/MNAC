using System;
using UnityEngine;

namespace MNAC.States
{
    public abstract class AnimationStateBase<T> : StateBase<T>
    {
        protected Animator animator;

        protected AnimationStateBase(string name, Animator animator) : base($"{name}_animation")
        {
            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }
        public virtual void OnAnimationIK(int layerIndex) { }
    }
}
