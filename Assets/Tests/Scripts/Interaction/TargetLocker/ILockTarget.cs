using UnityEngine;

namespace Tests.Interaction
{
    public interface ILockTarget
    {
        LockType LockType { get; set; }
        GameObject Obj { get; set; }
    }
}
