using System;
using System.Globalization;
using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    internal class ComponentNode : MTContainerNode<ICharacterComponent>, ICharacterComponentNode
    {
        public ComponentNode(ICharacterComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            this.id = component.ID;
            this.value = component;
        }
        public override IMTNode Parent
        {
            get => base.Parent;
            set
            {
                if (value == null)
                {
                    this.value.Dispose();
                    this.parent = null;
                }
                else
                {
                    if (value is not ICharacterComponentNode pnode)
                        throw new InvalidCastException(nameof(value));
                    parent = pnode;


                    if (pnode.Value != null)
                        this.value.Initialize(pnode.Value.Blackboard);
                }
            }
        }
        public override void AddChild(IMTNode node)
        {
            if (node is not ICharacterComponentNode cnode)
                throw new InvalidCastException(nameof(node));
            base.AddChild(cnode);
        }
        public override void RemoveChild(IMTNode node)
        {
            if (node is not ICharacterComponentNode cnode)
                throw new InvalidCastException(nameof(node));
            base.RemoveChild(node);
        }

    }

}
