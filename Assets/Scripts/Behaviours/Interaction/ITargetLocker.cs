using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Behaviours
{
    internal interface ITargetLocker : ITargetLocker<ILockTarget>
    {
        Action<GameObject, GameObject> MainObjChangedAction { get; set; }

    }
}