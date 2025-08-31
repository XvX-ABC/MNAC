namespace Tests.BT
{
    public class Selector : Composite
    {
        protected override TaskState OnWork()
        {
            foreach (var node in children)
            {
                state = node.Work();
                switch (state)
                {
                    case TaskState.Success:
                        return TaskState.Success;
                    case TaskState.Running:
                        return TaskState.Running;
                }
            }
            return TaskState.Failure;
        }
    }
}
