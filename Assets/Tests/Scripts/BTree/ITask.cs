namespace Tests.BT
{
    public interface ITask
    {
        public TaskState State { get; }
        public void OnStart() { }
        public void OnStop() { }
        public TaskState Work();
    }
    public interface ITaskComponent
    {
        public ITask Node { get; }
    }
}
