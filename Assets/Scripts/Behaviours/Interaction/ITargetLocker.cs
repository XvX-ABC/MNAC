using System;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Behaviours
{
    internal interface ITargetLocker : ITargetLocker<ILockTarget>
    {
        Action<GameObject, GameObject> MainObjChangedAction { get; set; }

    }
}