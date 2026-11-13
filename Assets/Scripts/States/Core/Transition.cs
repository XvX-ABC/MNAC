using System;
using System.Collections.Generic;

namespace MNAC.States
{
    /// <summary>
    /// 转移。由状态机创建并持有（状态自身不存转移表）。
    /// 触发条件分两类，两类之间是与关系：
    /// - 旧式 <see cref="Func{TResult}"/>（无上下文，闭包捕获）；
    /// - 新式 <see cref="ITransitionCondition{T}"/>（显式接收 Context）。
    /// 无任何触发条件 = 无条件转移（恒触发）。
    /// </summary>
    public class Transition<T>
    {
        readonly IState<T> source;
        readonly IState<T> destination;
        readonly List<Func<bool>> triggers;
        readonly List<ITransitionCondition<T>> conditions;

        public Transition(IState<T> source, IState<T> destination, Func<bool> trigger = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
            triggers = new List<Func<bool>>();
            conditions = new List<ITransitionCondition<T>>();
            AddTrigger(trigger);
        }

        public IState<T> Source => source;
        public IState<T> Destination => destination;

        /// <summary>触发成功时回调（携带 Context），在状态切换前调用。</summary>
        public Action<T> OnTriggered { get; set; }

        /// <summary>上次求值是否有结果（仅调试/可视化读取，不参与状态机逻辑）。</summary>
        public bool HasTriggerResult { get; private set; }

        /// <summary>上次 <see cref="Evaluate(T)"/> 的求值结果，供调试器展示。</summary>
        public bool LastTriggerResult { get; private set; }

        /// <summary>旧式无上下文触发条件（只读，运行时不要修改）。</summary>
        public IReadOnlyList<Func<bool>> Triggers => triggers;

        /// <summary>带上下文的条件（只读，运行时不要修改）。</summary>
        public IReadOnlyList<ITransitionCondition<T>> Conditions => conditions;

        /// <summary>条件总数 = 旧式触发器 + 带上下文条件。</summary>
        public int TriggerCount => triggers.Count + conditions.Count;

        /// <summary>是否存在任何触发条件。过渡中断用它判断“有条件的出边”。</summary>
        public bool HasTriggerConditions => TriggerCount > 0;

        /// <summary>
        /// 是否满足触发条件。空条件列表 = 恒 true；所有条件 AND 短路求值。
        /// 等价于 <see cref="Evaluate(T)"/> 传 <c>default(T)</c>。
        /// 依赖 Context 的条件必须走带参重载，否则拿到的是 default。
        /// </summary>
        public bool IsTriggered => Evaluate(default);

        /// <summary>
        /// 是否满足触发条件。结果会记录到 <see cref="LastTriggerResult"/>，
        /// 但每次调用都会重新求值，调试器读取缓存不会跳过未来帧的计算。
        /// </summary>
        public bool Evaluate(T context)
        {
            var result = true;
            foreach (var trigger in triggers)
            {
                if (trigger())
                    continue;
                result = false;
                break;
            }

            if (result)
            {
                foreach (var condition in conditions)
                {
                    if (condition.Evaluate(context))
                        continue;
                    result = false;
                    break;
                }
            }

            LastTriggerResult = result;
            HasTriggerResult = true;
            return result;
        }

        public void AddTrigger(Func<bool> trigger)
        {
            if (trigger == null)
                return;
            triggers.Add(trigger);
            HasTriggerResult = false;
        }

        public bool RemoveTrigger(Func<bool> trigger)
        {
            if (trigger == null || !triggers.Remove(trigger))
                return false;
            HasTriggerResult = false;
            return true;
        }

        public bool ContainsTrigger(Func<bool> trigger)
        {
            return trigger != null && triggers.Contains(trigger);
        }

        public void ClearTriggers()
        {
            if (triggers.Count == 0)
                return;
            triggers.Clear();
            HasTriggerResult = false;
        }

        public void AddCondition(ITransitionCondition<T> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            conditions.Add(condition);
            HasTriggerResult = false;
        }

        public bool RemoveCondition(ITransitionCondition<T> condition)
        {
            if (condition == null || !conditions.Remove(condition))
                return false;
            HasTriggerResult = false;
            return true;
        }

        public bool ContainsCondition(ITransitionCondition<T> condition)
        {
            return condition != null && conditions.Contains(condition);
        }

        public void ClearConditions()
        {
            if (conditions.Count == 0)
                return;
            conditions.Clear();
            HasTriggerResult = false;
        }
    }
}
