using System;
using Tests.Utilities.MTrees;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Animations
{
    public class AnimationPlayableNode : MTContainerNode<IAnimationPlayablePart>, IAnimationPlayablePartNode
    {

        [Obsolete]
        protected PlayableGraph graph;
        protected ushort connectedCount;
        [Obsolete]
        internal AnimationPlayableNode(PlayableGraph graph)
        {
            this.graph = graph;
        }
        public AnimationPlayableNode(IAnimationPlayablePart part)
        {
            value = part ?? throw new ArgumentNullException(nameof(part));
        }
        bool IsRoot(IMTNode node)
        {
            return node.Parent == null;
        }
        public IAnimationPlayablePartNode PlayableParent
        {
            get => (IAnimationPlayablePartNode)parent;
        }

        [Obsolete]
        public PlayableGraph Graph { get => graph; set => graph = value; }
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
            AddChild(pnode);
        }
        public virtual void AddChild(IAnimationPlayablePartNode node)
        {
            NodeValidityCheck(node);
            base.AddChild(node);
            if (!IsRoot(this) && value != null)
            {
                SetOutputSettingForNode(node);
                if (NodePlayablePartCheck(node))
                    ConnectChild(node);
            }
            node.Graph = graph;
        }
        public override void RemoveChild(IMTNode node)
        {
            if (node is not IAnimationPlayablePartNode pnode)
                throw new InvalidCastException(nameof(node));
            RemoveChild(pnode);
        }
        public virtual void RemoveChild(IAnimationPlayablePartNode node)
        {
            NodeValidityCheck(node);
            if (!children.Contains(node))
                return;
            if (NodePlayablePartCheck(node))
            {
                DisconnectChild(node);
                //node.Value.Dispose();
            }
            base.RemoveChild(node);
            node.Graph = default;
        }
        protected virtual void SetOutputSettingForNode(IAnimationPlayablePartNode node)
        {
            var p = value.PlayablePart;
            var idx = children == null ? 0 : connectedCount;
            var outputSetting = node.Value.OutputSetting ?? throw new NullReferenceException(nameof(node.Value.OutputSetting));
            outputSetting.PortNum = idx;
            outputSetting.Parent = value;
        }
        protected virtual void ConnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = value.PlayablePart;
            //var idx = children == null ? 0 : connectedCount;
            var outputSetting = childNode.Value.OutputSetting;
            var idx = outputSetting.PortNum;

            p.ConnectInput(idx, childNode.Value.PlayablePart, 0, outputSetting.Weight);
            connectedCount++;
        }
        protected virtual void DisconnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = value.PlayablePart;

            var setting = childNode.Value.OutputSetting;
            var idx = setting.PortNum;

            p.DisconnectInput(idx);
            connectedCount--;
        }

    }
}
