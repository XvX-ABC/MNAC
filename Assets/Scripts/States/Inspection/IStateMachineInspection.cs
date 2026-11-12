using System.Collections.Generic;

namespace MNAC.States
{
    /// <summary>状态机只读快照，供 Editor Inspector / 调试工具使用，不参与运行时逻辑。</summary>
    public interface IStateMachineInspection
    {
        string Name { get; }
        bool Enabled { get; }
        string CurrentStateName { get; }
        IReadOnlyList<StateSnapshot> StateSnapshots { get; }
        IReadOnlyList<TransitionSnapshot> TransitionSnapshots { get; }

        bool IsTransitioning { get; }
        string ActiveTransitionSource { get; }
        string ActiveTransitionDestination { get; }
        float ActiveTransitionProgress { get; }
    }

    public readonly struct StateSnapshot
    {
        public StateSnapshot(string name, string typeName, bool enabled, bool isCurrent)
        {
            Name = name;
            TypeName = typeName;
            Enabled = enabled;
            IsCurrent = isCurrent;
        }

        public string Name { get; }
        public string TypeName { get; }
        public bool Enabled { get; }
        public bool IsCurrent { get; }
    }

    public readonly struct TransitionSnapshot
    {
        public TransitionSnapshot(
            string sourceName,
            string destinationName,
            float duration,
            InterruptionMode interruptionMode,
            int triggerCount,
            bool hasTriggerResult,
            bool lastTriggerResult,
            bool isActive,
            float progress)
        {
            SourceName = sourceName;
            DestinationName = destinationName;
            Duration = duration;
            InterruptionMode = interruptionMode;
            TriggerCount = triggerCount;
            HasTriggerResult = hasTriggerResult;
            LastTriggerResult = lastTriggerResult;
            IsActive = isActive;
            Progress = progress;
        }

        public string SourceName { get; }
        public string DestinationName { get; }
        public float Duration { get; }
        public InterruptionMode InterruptionMode { get; }
        public int TriggerCount { get; }
        public bool HasTriggerResult { get; }
        public bool LastTriggerResult { get; }
        public bool IsActive { get; }
        public float Progress { get; }

        public bool IsPlayable => Duration > 0f || InterruptionMode != InterruptionMode.None;
    }
}
