using System;
using UnityEngine;

namespace Tests.BT
{
    public abstract class Task : ITask
    {
        protected bool started;
        protected Guid guid;
        protected TaskState state;
        public TaskState State { get => state; }
        protected Task()
        {
            guid = Guid.NewGuid();
        }
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        protected virtual TaskState OnWork() { return TaskState.Failure; }
        public virtual TaskState Work()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }
            state = OnWork();
            if (state == TaskState.Failure || state == TaskState.Success)
            {
                started = false;
                OnStop();
            }
            return state;
        }
        public override string ToString()
        {
            return $"type: {this.GetType().Name}, state: {state}";
        }
    }
}
