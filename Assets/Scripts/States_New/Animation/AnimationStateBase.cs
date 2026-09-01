using System;
using UnityEngine;

namespace MNAC.StatesNew
{
    /// <summary>动画状态基类：持 Animator 引用，暴露 IK 钩子。仅此一层薄封装，动画播放仍需自行用 Animator/Playables 完成。</summary>
    public abstract class AnimationStateBase<T> : StateBase<T>
    {
        protected readonly Animator animator;

        protected AnimationStateBase(string name, Animator animator) : base($"{name}_animation")
        {
            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public virtual void OnAnimationIK(int layerIndex)
        {
        }
    }
}
