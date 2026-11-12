using System;
using System.Collections.Generic;

namespace MNAC.States
{
    /// <summary>
    /// 转移。由状态机创建并持有（状态自身不存转移表）。
    /// 触发条件：多个 <see cref="Func{TResult}"/> 是与关系，遇 false 短路；无触发条件 = 无条件转移（恒触发）。
    /// </summary>
    public class Transition<T>
    {
        readonly IState<T> source;
        readonly IState<T> destination;
        readonly List<Func<bool>> triggers;

        public Transition(IState<T> source, IState<T> destination, Func<bool> trigger = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
            triggers = new List<Func<bool>>();
            AddTrigger(trigger);
        }

        public IState<T> Source => source;
        public IState<T> Destination => destination;

        /// <summary>触发成功时回调（携带 Context），在状态切换前调用。</summary>
        public Action<T> OnTriggered { get; set; }

        /// <summary>上次求值是否有结果（仅调试/可视化读取，不参与状态机逻辑）。</summary>
        public bool HasTriggerResult { get; private set; }

        /// <summary>上次 <see cref="IsTriggered"/> 的求值结果，供调试器展示，避免重复调用委托。</summary>
        public bool LastTriggerResult { get; private set; }

        /// <summary>只读触发条件列表。</summary>
        public IReadOnlyList<Func<bool>> Triggers => triggers;

        /// <summary>
        /// 是否满足触发条件。空条件列表 = 恒 true；多个条件 AND 短路求值。
        /// 结果会缓存到 <see cref="LastTriggerResult"/>，调试器直接读取缓存，不重复调用委托。
        /// </summary>
        public bool IsTriggered
        {
            get
            {
                var result = true;
                foreach (var trigger in triggers)
                {
                    if (trigger())
                        continue;
                    result = false;
                    break;
                }
                LastTriggerResult = result;
                HasTriggerResult = true;
                return result;
            }
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
    }
}
