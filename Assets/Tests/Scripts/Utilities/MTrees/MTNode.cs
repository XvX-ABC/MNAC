using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Tests.Utilities.MTrees
{
    public abstract class MTContainerNode<T> : MTNode, IMTContainerNode<T>
    {
        protected T value;
        public T Value { get => value; set => this.value = value; }
        public T ParentValue { get => Parent is IMTContainerNode<T> parentNode ? parentNode.Value : default; }
        //public new virtual IMTContainerNode<T> Parent { get => (IMTContainerNode<T>)base.Parent; set => base.Parent = value; }
        public IMTContainerNode<T> GetChild(int index)
        {
            return children[index] as IMTContainerNode<T>;
        }
        protected MTContainerNode()
        {
        }

        protected MTContainerNode(Guid id, T value) : base(id)
        {
            Value = value;
        }


    }
    public abstract class MTNode : IMTNode
    {
        protected IMTNode parent;
        protected List<IMTNode> children;
        protected Guid id;
        protected int level;

        public virtual IMTNode Parent { get => parent; set => parent = value; }
        public IList<IMTNode> Children { get => children; set => children = (List<IMTNode>)value; }
        public Guid ID { get => id; }
        public int Level { get => level; set => level = value; }

        public MTNode()
        {
            id = Guid.NewGuid();
            children = new List<IMTNode>();
        }
        public MTNode(Guid id)
        {
            this.id = id;
            children = new List<IMTNode>();
        }

        public virtual void AddChild(IMTNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.Level = level + 1;
            node.Parent = this;
            children.Add(node);
        }

        public virtual void RemoveChild(IMTNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.Parent = null;
            children.Remove(node);
        }
    }
}
