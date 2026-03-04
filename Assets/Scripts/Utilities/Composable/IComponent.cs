using System;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;

namespace MNAC.Utilities.Composable
{
    public interface IComponent<T> : IComponentDescriptions, IDisposable
    {
        public bool Enabled { get; set; }
        public T Context { get; set; }
        public IComponentNode<T> Node { get; }
        public void Initialize(T context);
    }
    public interface IComponent : IComponentDescriptions, IDisposable
    {
        public bool Enabled { get; set; }
        public Blackboard Blackboard { get; set; }
        public IComponentNode Node { get; }
        public void Initialize(Blackboard blackboard);

    }
}
