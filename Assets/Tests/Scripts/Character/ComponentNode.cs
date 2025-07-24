using System;
using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    internal class ComponentNode : MTContainerNode<ICharacterComponent>, ICharacterComponentNode
    {
        public ComponentNode()
        {
        }

        public ComponentNode(Guid id, ICharacterComponent component) : base(id, component)
        {
        }
        internal void UpdateBlackboardForChildren()
        {
            foreach (var c in children)
            {
                var node = (ICharacterComponentNode)c;
                if (node.Value != null)
                    node.Value.Blackboard = this.value.Blackboard;
            }
        }
        public override IMTNode Parent
        {
            get => base.Parent;
            set
            {
                if (value is not ICharacterComponentNode cnode)
                    throw new InvalidCastException(nameof(value));
                parent = cnode;
                if(cnode.Value!=null)
                    this.value.Blackboard= cnode.Value.Blackboard;
                UpdateBlackboardForChildren();
            }
        }
        public override void AddChild(IMTNode node)
        {
            if (node is not ICharacterComponentNode cnode)
                throw new InvalidCastException(nameof(node));
            if (cnode.Value != null)
                cnode.Value.Blackboard = this.value.Blackboard;
            base.AddChild(cnode);
        }
        public override void RemoveChild(IMTNode node)
        {
            if (node is not ICharacterComponentNode cnode)
                throw new InvalidCastException(nameof(node));
            if (cnode.Value != null)
                cnode.Value.Blackboard = null;
            base.RemoveChild(node);
        }

    }

}
