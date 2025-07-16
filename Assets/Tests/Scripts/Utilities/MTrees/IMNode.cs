using System;
using System.Collections.Generic;

namespace Tests.Utilities.MTrees
{
    internal interface IMNode<T>
    {
        public IMNode<T> Parent { get; set; }
        public IList<IMNode<T>> Children { get; set; }
        public Guid ID { get; }
        public int Level { get; set; }
        public void AddChild(IMNode<T> node);
        public void RemoveChild(IMNode<T> node);
    }
}
