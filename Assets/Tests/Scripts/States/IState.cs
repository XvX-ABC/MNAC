using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.States
{
    internal interface IState<T>
    {
        public string Name { get; }
        public Guid ID { get; }
        public Transition<T>[] Transitions { get; set; }
        public bool Enabled { get; set; }
        public T Context { set; }
        public void OnEnter();
        public void OnUpdate();
        public void OnExit();
    }
}
