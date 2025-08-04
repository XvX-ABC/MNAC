using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Utilities.MTrees;
using Unity.VisualScripting;
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
        internal AnimationPlayableNode(PlayableGraph graph)
        {
            this.graph = graph;
        }
        public AnimationPlayableNode(IPlayablePart part)
        {
            this.value = part ?? throw new ArgumentNullException(nameof(part));
        }
        public override IMTNode Parent
        {
            get => this.parent;
            set
            {


                var oldParent = (IPlayablePartNode)this.parent;
                if (oldParent != null)
                {
                    DisconnectFromParent(oldParent);
                    this.value.Dispose();
                }


                if (value != null)
                {
                    if (value is not IPlayablePartNode pnode)
                        throw new InvalidCastException(nameof(value));
                    var newParent = pnode;
                    if (this.value.Initialize(newParent.Graph))
                    {
                        if (newParent.Value != null)
                            ConnectToParent(newParent);
                    }
                    else
                        throw new Exception();
                    graph = newParent.Graph;
                }

                parent = value;
            }
        }
        public PlayableGraph Graph { get => graph; set => graph = value; }
        internal virtual void ConnectToParent(IPlayablePartNode parentNode)
        {
            var p = parentNode.Value.PlayablePart;
            var idx = parentNode.Children == null ? 0 : parentNode.Children.Count;
            var outputSetting = new OutputSetting(p, idx);
            p.ConnectInput(idx, this.value.PlayablePart, 0);
            this.value.OutputSetting = outputSetting;
        }
        internal virtual void DisconnectFromParent(IPlayablePartNode parentNode)
        {
            var p = parentNode.Value.PlayablePart;
            var idx = parentNode.Children.IndexOf(this);
            p.DisconnectInput(idx);
            this.value.OutputSetting = null;
        }


    }
}
