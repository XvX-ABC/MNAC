using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.States
{
    internal interface IInternalState<T> 
    {

    }
    internal interface IInternalDurationTimeState<T> : IInternalState<T>
    {
        internal new DurationTimeTransition<T>[] Transitions { get; set; }
    }
}
