using UnityEngine;

namespace Tests.BT
{
    public class Sequencer : Composite
    {
        protected override TaskState OnWork()
        {
            foreach (var node in children)
            {
                state = node.Work();
                switch (state)
                {
                    case TaskState.Failure:
                        return TaskState.Failure;
                    case TaskState.Running:
                        return TaskState.Running;
                }
            }
            return TaskState.Success;
        }
    }
}
