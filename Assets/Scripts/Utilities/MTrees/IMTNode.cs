using System;
using System.Collections.Generic;

namespace MNAC.Utilities.MTrees
{
    public interface IMTNode
    {
        public IMTNode Parent { get; set; }
        public IList<IMTNode> Children { get; }
        public Guid ID { get; }
        public int Level { get; set; }
        public void AddChild(IMTNode node);
        public void RemoveChild(IMTNode node);
    }
    public interface IMTContainerNode<T> : IMTNode
    {
        public T Value { get; set; }
    }
}
