using System;

namespace MNAC.States
{
    /// <summary>把 <see cref="Func{T,TResult}"/> 适配为 <see cref="ITransitionCondition{T}"/>。</summary>
    public sealed class DelegateTransitionCondition<T> : ITransitionCondition<T>
    {
        readonly Func<T, bool> predicate;

        public DelegateTransitionCondition(Func<T, bool> predicate)
        {
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public bool Evaluate(T context) => predicate(context);
    }
}
