using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;

namespace Tests.Utilities.Composable
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
