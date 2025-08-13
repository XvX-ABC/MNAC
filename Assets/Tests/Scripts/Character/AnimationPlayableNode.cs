using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Utilities.MTrees;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Character
{
    public class AnimationPlayablePartTree : MTree
    {
        public AnimationPlayablePartTree(PlayableGraph graph)
        {
            this.root = new AnimationPlayableNode(graph);
            this.enumerator = new(this.root);
        }
    }
    public class AnimationPlayableNode : MTContainerNode<IAnimationPlayablePart>, IAnimationPlayablePartNode
    {
        protected PlayableGraph graph;
        protected ushort connectedCount;
        internal AnimationPlayableNode(PlayableGraph graph)
        {
            this.graph = graph;
        }
        public AnimationPlayableNode(IAnimationPlayablePart part)
        {
            this.value = part ?? throw new ArgumentNullException(nameof(part));
        }
        bool IsRoot(IMTNode node)
        {
            return node.Parent == null;
        }
        public override IMTNode Parent
        {
            get => this.parent;
            set
            {
                parent = value;
            }
        }
        public IAnimationPlayablePartNode PlayableParent
        {
            get => (IAnimationPlayablePartNode)this.parent;
        }
        public PlayableGraph Graph { get => graph; set => graph = value; }
        [Obsolete]
        bool NodeCheck(IAnimationPlayablePartNode node)
        {
            if (node.Value == null)
                throw new NullReferenceException(nameof(node.Value));
            if (node.Value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot connect to parent.");
                return false;
            }
            return true;
        }
        void NodeValidityCheck(IAnimationPlayablePartNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (node.Value == null)
                throw new NullReferenceException(nameof(node.Value));
        }
        bool NodePlayablePartCheck(IAnimationPlayablePartNode node)
        {
            if (node.Value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Child node's PlayablePart is null, cannot connect to parent.");
                return false;
            }
            return true;
        }
        public override void AddChild(IMTNode node)
        {
            if (node is not IAnimationPlayablePartNode pnode)
                throw new InvalidCastException(nameof(node));

            NodeValidityCheck(pnode);
            base.AddChild(pnode);
            if (pnode.Value.Initialize(this.graph))
            {
                if (!IsRoot(this) && this.value != null)
                {
                    SetOutputSettingForNode(pnode);
                    if (NodePlayablePartCheck(pnode))
                        ConnectChild(pnode);
                }
            }
            pnode.Graph = this.graph;

        }
        public override void RemoveChild(IMTNode node)
        {
            if (node is not IAnimationPlayablePartNode pnode)
                throw new InvalidCastException(nameof(node));
            if (!children.Contains(node))
                return;
            NodeValidityCheck(pnode);
            if (NodePlayablePartCheck(pnode))
            {
                this.DisconnectChild(pnode);
                pnode.Value.Dispose();
            }
            base.RemoveChild(pnode);
            pnode.Graph = default;

        }
        void SetOutputSettingForNode(IAnimationPlayablePartNode node)
        {
            var p = this.value.PlayablePart;
            var idx = this.children == null ? 0 : connectedCount;
            var outputSetting = new OutputSetting(p, idx);
            node.Value.OutputSetting = outputSetting;
        }
        protected virtual void ConnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = this.value.PlayablePart;
            var idx = this.children == null ? 0 : connectedCount;

            p.ConnectInput(idx, childNode.Value.PlayablePart, 0);
            connectedCount++;
        }
        protected virtual void DisconnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = this.value.PlayablePart;

            var setting = (OutputSetting)childNode.Value.OutputSetting;
            var idx = setting.portNum;
            childNode.Value.OutputSetting = null;

            p.DisconnectInput(idx);
            connectedCount--;
        }
        [Obsolete]
        internal virtual void ConnectToParent(IAnimationPlayablePartNode parentNode)
        {
            var v = parentNode.Value;
            if (v == null)
                throw new NullReferenceException(nameof(parentNode.Value));
            var p = v.PlayablePart;
            if (p.IsNull())
            {
                Debug.LogWarning("Parent PlayablePart is null, cannot connect to it.");
                return;
            }
            if (this.value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot connect to parent.");
                return;
            }
            var idx = parentNode.Children == null ? 0 : parentNode.Children.Count;
            var outputSetting = new OutputSetting(p, idx);
            p.ConnectInput(idx, this.value.PlayablePart, 0);
            this.value.OutputSetting = outputSetting;
        }
        [Obsolete]
        internal virtual void DisconnectFromParent(IAnimationPlayablePartNode parentNode)
        {
            var v = parentNode.Value;
            if (v == null)
                throw new NullReferenceException(nameof(parentNode.Value));
            var p = v.PlayablePart;
            if (p.IsNull())
            {
                Debug.LogWarning("Parent PlayablePart is null, cannot connect to it.");
                return;
            }
            if (this.value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot disconnect from parent.");
                return;
            }
            var idx = parentNode.Children.IndexOf(this);
            p.DisconnectInput(idx);
            this.value.OutputSetting = null;
        }
        [Obsolete]
        public virtual void RemoveAllChildren()
        {
            if (this.children == null || this.children.Count == 0)
                return;
            foreach (var node in children)
            {
                this.RemoveChild(node);
            }
        }

    }
}
