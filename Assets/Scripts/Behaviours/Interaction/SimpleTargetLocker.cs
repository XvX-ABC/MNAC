using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Behaviours
{
    internal class SimpleTargetLocker : SimpleTargetLocker<ILockTarget>, ITargetLocker
    {
        GameObjTarget _currentObjTarget;
        GameObject _ownerObj;
        Action<GameObject, GameObject> _mainObjChangedAction;
        public SimpleTargetLocker(GameObjsInRadiusCatcher objsCatcher, Func<GameObject, ILockTarget> getTargetFunc, Action<ILockTarget> releaseAction, ObstacleDetector obstacleDetector = null) : base(objsCatcher, getTargetFunc, releaseAction, obstacleDetector)
        {
            MainTargetChangedAction += WhenTargetChangedAction;
        }

        public Action<GameObject, GameObject> MainObjChangedAction { get => _mainObjChangedAction; set => _mainObjChangedAction = value; }
        public GameObject OwnerObj { get => _ownerObj; set => _ownerObj = value; }

        void WhenTargetChangedAction(ILockTarget oldTarget, ILockTarget newTarget)
        {
            _mainObjChangedAction?.Invoke(oldTarget?.Obj, newTarget?.Obj);
        }
        protected override void WhenCaughtItem(GameObject obj)
        {
            if (obj == _ownerObj)
                return;
            base.WhenCaughtItem(obj);
        }
    }
}
