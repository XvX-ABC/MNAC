using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.AI;
using Tests.Behaviours;
using Tests.Interaction;
using UnityEngine;

namespace Tess.AI
{
    internal class AITargetLocker : AIComponent, ITargetLocker
    {
        SimpleTargetLocker _locker;
        [SerializeField]
        GameObjsInRadiusCatcher _objsCatcher;
        AINavigation _navigation;
        ILockTarget _mainLockTarget;

        public Action<GameObject, GameObject> MainObjChangedAction { get => _locker.MainObjChangedAction; set => _locker.MainObjChangedAction = value; }
        public ILockTarget MainLockTarget { get => _locker.MainLockTarget; set => _locker.MainLockTarget = value; }
        public Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => _locker.MainTargetChangedAction; set => _locker.MainTargetChangedAction = value; }
        public ObstacleDetector ObstacleDetector { get => _locker.ObstacleDetector; set => _locker.ObstacleDetector = value; }
        public float TargetChangeDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);

            _locker.MainTargetChangedAction += WhenTargetChanged;
        }
        public override void Dispose()
        {
            _locker.MainTargetChangedAction -= WhenTargetChanged;
            base.Dispose();
        }
        void OnDestroy()
        {
            Dispose();
        }
        void WhenTargetChanged(ILockTarget oldTarget, ILockTarget newTarget)
        {

        }
        public void OnFixedUpdate()
        {
            _locker.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            _locker.OnLateUpdate();
        }
    }
}
