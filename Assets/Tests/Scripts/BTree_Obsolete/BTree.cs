using System;
using Tests.States;

namespace Tests.BT
{

    public class BTree : Task
    {
        bool _enabled;
        ITask _rootNode;
        public bool RestartWhenComplete;
        public bool StartWhenEnabled;
        public BTree(ITask rootNode)
        {
            _rootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }
        public override TaskState Work()
        {
            if (StartWhenEnabled && !_enabled)
                _enabled = true;
            if (!_enabled)
                return TaskState.Failure;
            var state = _rootNode.Work();
            if (state == TaskState.Running)
                return state;
            else if (!RestartWhenComplete)
                _enabled = false;
            return state;
        }
    }
}
