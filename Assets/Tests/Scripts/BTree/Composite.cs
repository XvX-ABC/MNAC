using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace Tests.BTree
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }
    public interface INode
    {
        public bool Enabled { get; set; }
        public NodeState Update();
    }
    public class BTree : Node
    {
        INode _rootNode;
        public override NodeState Update()
        {
            if (_rootNode.Enabled)
                return _rootNode.Update();
            return NodeState.Failure;
        }
    }
    public abstract class Node : ScriptableObject, INode
    {
        protected bool enabled;

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value)
                    OnStart();
                else
                    OnStop();
                enabled = value;
            }
        }

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual NodeState OnUpdate() { return NodeState.Failure; }
        public virtual NodeState Update()
        {
            if (!enabled)
                Enabled = true;
            var state = OnUpdate();
            if (state == NodeState.Failure || state == NodeState.Success)
                Enabled = false;
            return state;
        }
    }
    public abstract class CompositeNode : Node
    {
        protected List<INode> children;
        protected CompositeNode()
        {
            children = new List<INode>();
        }
    }
    public class SequencerNode : CompositeNode
    {
        protected override NodeState OnUpdate()
        {
            foreach (var node in children)
            {
                var state = node.Update();
                switch (state)
                {
                    case NodeState.Failure:
                        return NodeState.Failure;
                    case NodeState.Running:
                        return NodeState.Running;
                }
            }
            return NodeState.Success;
        }
    }
    public class FallbackNode : CompositeNode
    {
        protected override NodeState OnUpdate()
        {
            foreach (var node in children)
            {
                var state = node.Update();
                switch (state)
                {
                    case NodeState.Success:
                        return NodeState.Success;
                    case NodeState.Running:
                        return NodeState.Running;
                }
            }
            return NodeState.Failure;
        }
    }
}
