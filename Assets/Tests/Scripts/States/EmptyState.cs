using System;

namespace Tests.States
{
    public class EmptyState<T> : StateBase<T>
    {

        public EmptyState() : base("Empty")
        {
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnUpdate()
        {

        }
    }
}
