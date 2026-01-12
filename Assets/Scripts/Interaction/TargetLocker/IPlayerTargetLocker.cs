using System;
using UnityEngine;

namespace Tests.Interaction
{
    public interface IPlayerTargetLocker<T> : ITargetLocker<T> where T : class, ILockTarget
    {
        public bool ObjsCatchEnable { get; set; }
        public bool CursorEnable { get; set; }
        float CatchAngle { get; set; }
        Vector3 CursorPosition { get; set; }
        Vector3 CursorPositionDelta { get; set; }
        Vector3 OriginWorldPosition { get; set; }
    }
}