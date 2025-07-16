using System;
using System.Collections.Generic;

namespace Tests.Utilities.MTrees
{
    internal abstract class MNodeBase<T> : IMNode<T>
    {
        protected IMNode<T> parent;
        protected List<IMNode<T>> children;
        protected Guid id;
        protected int level;

        public IMNode<T> Parent { get => parent; set => parent = value; }
        public IList<IMNode<T>> Children { get => children; set => children = (List<IMNode<T>>)value; }
        public Guid ID { get => id; }
        public int Level { get => level; set => level = value; }

        protected MNodeBase()
        {
            id = Guid.NewGuid();
        }

        public void AddChild(IMNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.Level = level + 1;
            node.Parent = this;
            children.Add(node);
        }

        public void RemoveChild(IMNode<T> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.Parent = null;
            children.Remove(node);
        }
    }
}
