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
    public class AnimationPlayableNode : MTContainerNode<IPlayablePart>, IPlayablePartNode
    {
        protected PlayableGraph graph;
        protected ushort connectedCount;
        internal AnimationPlayableNode(PlayableGraph graph)
        {
            this.graph = graph;
        }
        public AnimationPlayableNode(IPlayablePart part)
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
                var oldParent = (IPlayablePartNode)this.parent;
                if (oldParent != null && !IsRoot(oldParent))
                {
                    oldParent.DisconnectChild(this);
                    this.value.Dispose();
                }


                if (value != null)
                {
                    if (value is not IPlayablePartNode pnode)
                        throw new InvalidCastException(nameof(value));
                    var newParent = pnode;
                    if (this.value.Initialize(newParent.Graph))
                    {
                        if (!IsRoot(newParent) && newParent.Value != null)
                            newParent.ConnectChild(this);
                    }
                    graph = newParent.Graph;
                }

                parent = value;

            }
        }
        public PlayableGraph Graph { get => graph; set => graph = value; }

        bool NodeCheck()
        {
            if (this.value == null)
                throw new NullReferenceException(nameof(this.value));
            if (this.value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Current PlayablePart is null, cannot connect to parent.");
                return false;
            }
            return true;
        }
        void ChildNodeValidityCheck(IPlayablePartNode childNode)
        {
            if (childNode == null)
                throw new ArgumentNullException(nameof(childNode));
            if (childNode.Value == null)
                throw new NullReferenceException(nameof(childNode.Value));
        }
        bool ChildNodePlayablePartCheck(IPlayablePartNode childNode)
        {
            if (childNode.Value.PlayablePart.IsNull())
            {
                Debug.LogWarning("Child node's PlayablePart is null, cannot connect to parent.");
                return false;
            }
            return true;
        }
        public virtual void ConnectChild(IPlayablePartNode childNode)
        {
            //if (this.value == null)
            //    throw new NullReferenceException(nameof(this.value));
            //if (childNode == null)
            //    throw new ArgumentNullException(nameof(childNode));
            //var p = this.value.PlayablePart;
            //if (p.IsNull())
            //{
            //    Debug.LogWarning("Current PlayablePart is null, cannot connect to it.");
            //    return;
            //}

            //if (childNode.Value.PlayablePart.IsNull())
            //{
            //    Debug.LogWarning("Child node's PlayablePart is null, cannot connect to parent.");
            //    return;
            //}
            if (!NodeCheck())
                return;

            var p = this.value.PlayablePart;
            var idx = this.children == null ? 0 : connectedCount;
            ChildNodeValidityCheck(childNode);
            var outputSetting = new OutputSetting(p, idx);
            childNode.Value.OutputSetting = outputSetting;
            if (!ChildNodePlayablePartCheck(childNode))
                return;


            p.ConnectInput(idx, childNode.Value.PlayablePart, 0);
            connectedCount++;
        }
        public virtual void DisconnectChild(IPlayablePartNode childNode)
        {
            //if (this.value == null)
            //    throw new NullReferenceException(nameof(this.value));
            //if (childNode == null)
            //    throw new ArgumentNullException(nameof(childNode));
            //var p = this.value.PlayablePart;
            //if (p.IsNull())
            //{
            //    Debug.LogWarning("Current PlayablePart is null, cannot disconnect from it.");
            //    return;
            //}
            //if (childNode.Value.PlayablePart.IsNull())
            //{
            //    Debug.LogWarning("Child node's PlayablePart is null, cannot disconnect from parent.");
            //    return;
            //}
            if (!NodeCheck())
                return;

            var p = this.value.PlayablePart;

            ChildNodeValidityCheck(childNode);

            var setting = (OutputSetting)childNode.Value.OutputSetting;
            var idx = setting.portNum;
            childNode.Value.OutputSetting = null;
            if (!ChildNodePlayablePartCheck(childNode))
                return;


            p.DisconnectInput(idx);
            connectedCount--;
        }
        internal virtual void ConnectToParent(IPlayablePartNode parentNode)
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
        internal virtual void DisconnectFromParent(IPlayablePartNode parentNode)
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


    }
}
