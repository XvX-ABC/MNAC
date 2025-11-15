using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    public interface ILockTarget
    {
        LockType LockType { get; set; }
        GameObject Obj { get; set; }
    }
}
