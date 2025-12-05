using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Behaviours
{
    internal abstract class TargetLocker : TargetLockerBase<ILockTarget>, ITargetLocker
    {
        Action<GameObject, GameObject> _mainObjChangedAction;

        protected TargetLocker()
        {
            MainTargetChangedAction += WhenTargetChangedAction;
        }

        public Action<GameObject, GameObject> MainObjChangedAction { get => _mainObjChangedAction; set => _mainObjChangedAction = value; }
        void WhenTargetChangedAction(ILockTarget oldTarget, ILockTarget newTarget)
        {
            _mainObjChangedAction?.Invoke(oldTarget?.Obj, newTarget?.Obj);
        }
    }
}
