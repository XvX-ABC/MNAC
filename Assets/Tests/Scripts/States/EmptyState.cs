using System;

namespace Tests.States
{
    public class EmptyState<T> : StateBase<T>
    {

        public EmptyState():base("Empty")
        {
        }
    }
}
