using UnityEngine;

namespace Tests.Interaction
{
    public interface ILockTarget : IGameObjTarget_New, IPositionTarget
    {
        LockType LockType { get; set; }
    }
}
