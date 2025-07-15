using Tests.Input;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public enum BehaviourState
    {
        None,
        Ready,
        Running,
        Ended,
    }
    public interface IArmBehaviour : IState
    {
    }
}
