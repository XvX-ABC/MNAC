using UnityEngine;

namespace MNAC.Interaction
{
    public interface ILockTarget : IGameObjTarget_New, IPositionTarget
    {
        LockType LockType { get; set; }
    }
}
