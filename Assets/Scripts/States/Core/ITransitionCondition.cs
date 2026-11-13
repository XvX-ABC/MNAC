namespace MNAC.States
{
    /// <summary>
    /// 带上下文的转移条件。与旧的 <c>Func&lt;bool&gt;</c> 触发器并存，
    /// 适合读取构建时注入的 Context（距离、血量、输入等）。
    /// </summary>
    public interface ITransitionCondition<T>
    {
        /// <summary>是否满足条件。多个条件之间为 AND 关系。</summary>
        bool Evaluate(T context);
    }
}
