using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Assertions.Must;

namespace Tests.States
{
    public class StateMachine<T> : StateMachineBase<StateBase<T>, T>
    {
        public StateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

}
