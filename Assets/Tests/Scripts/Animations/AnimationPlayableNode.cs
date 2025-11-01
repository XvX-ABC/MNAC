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
        //public override IMTNode Parent
        //{
        //    get => parent;
        //    set
        //    {
        //        parent = value;
        //    }
        //}
        public IAnimationPlayablePartNode PlayableParent
        {
            get => (IAnimationPlayablePartNode)parent;
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
        }
        public virtual void AddChild(IAnimationPlayablePartNode node)
        {
            NodeValidityCheck(node);
            base.AddChild(node);
            //if (node.Value.Initialize(graph))
            //{
            if (!IsRoot(this) && value != null)
            {
                SetOutputSettingForNode(node);
                if (NodePlayablePartCheck(node))
                    ConnectChild(node);
            }
            //}
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
        //void SetOutputSettingForNode(IAnimationPlayablePartNode node)
        //{
        //    var p = value.PlayablePart;
        //    var idx = children == null ? 0 : connectedCount;
        //    //var outputSetting = new OutputSetting(p, idx);
        //    var outputSetting = new OutputSetting(value, idx);
        //    node.Value.OutputSetting = outputSetting;
        //}
        void SetOutputSettingForNode(IAnimationPlayablePartNode node)
        {
            var p = value.PlayablePart;
            var idx = children == null ? 0 : connectedCount;
            var outputSetting = node.Value.OutputSetting ?? throw new NullReferenceException(nameof(node.Value.OutputSetting));
            //var outputSetting = new OutputSetting(p, idx);
            //var outputSetting = new OutputSetting(value, idx);
            //node.Value.OutputSetting = outputSetting;
            outputSetting.PortNum = idx;
            outputSetting.Parent = value;
        }
        protected virtual void ConnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = value.PlayablePart;
            var idx = children == null ? 0 : connectedCount;
            var outputSetting = childNode.Value.OutputSetting;

            p.ConnectInput(idx, childNode.Value.PlayablePart, 0, outputSetting.Weight);
            connectedCount++;
        }
        protected virtual void DisconnectChild(IAnimationPlayablePartNode childNode)
        {
            var p = value.PlayablePart;

            var setting = childNode.Value.OutputSetting;
            var idx = setting.PortNum;
            //childNode.Value.OutputSetting = null;

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
            if (value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot connect to parent.");
                return;
            }
            var idx = parentNode.Children == null ? 0 : parentNode.Children.Count;
            //var outputSetting = new OutputSetting(p, idx);
            var outputSetting = new OutputSetting(v, idx);
            p.ConnectInput(idx, value.PlayablePart, 0);
            value.OutputSetting = outputSetting;
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
            if (value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot disconnect from parent.");
                return;
            }
            var idx = parentNode.Children.IndexOf(this);
            p.DisconnectInput(idx);
            value.OutputSetting = null;
        }
        [Obsolete]
        public virtual void RemoveAllChildren()
        {
            if (children == null || children.Count == 0)
                return;
            foreach (var node in children)
            {
                RemoveChild(node);
            }
        }

    }
}
