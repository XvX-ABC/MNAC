using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    public class SimpleTargetLocker<T> : TargetLockerBase<T>, ITargetLocker<T>, IDisposable where T : class, ILockTarget
    {
        List<T> _targets;
        T _mainLockTarget;
        FilterCollection _filters;
        Action<T, T> _mainLockTargetChangedAction;

        Func<GameObject, T> _getTargetFunc;
        Action<T> _releaseTargetAction;

        GameObjsInRadiusCatcher _objsCatcher;
        ObstacleDetector _obstacleDetector;
        public SimpleTargetLocker(
            GameObjsInRadiusCatcher objsCatcher,
            Func<GameObject, T> getTargetFunc,
            Action<T> releaseAction,
            ObstacleDetector obstacleDetector = null)
        {
            _targets = new();
            _objsCatcher = objsCatcher ?? throw new ArgumentNullException(nameof(objsCatcher));
            _getTargetFunc = getTargetFunc ?? throw new ArgumentNullException(nameof(getTargetFunc));
            _releaseTargetAction = releaseAction ?? throw new ArgumentNullException(nameof(releaseAction));
            _obstacleDetector = obstacleDetector;

            _objsCatcher.ItemCaughtAction += WhenCaughtItem;
            _objsCatcher.ItemReleaseAction += WhenReleaseItem;
            _objsCatcher.CatchCompletedAction += WhenCatchCompleted;
        }

        public override T MainLockTarget { get => _mainLockTarget; set => _mainLockTarget = value; }
        public override Action<T, T> MainTargetChangedAction { get => _mainLockTargetChangedAction; set => _mainLockTargetChangedAction = value; }
        public override ObstacleDetector ObstacleDetector { get => _obstacleDetector; set => _obstacleDetector = value; }
        public override float TargetChangeDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddFilter(ICaughtItemFilter<GameObject> filter)
        {
            _filters.AddFilter(filter);
        }
        public void RemoveFilter(ICaughtItemFilter<GameObject> filter)
        {
            _filters.RemoveFilter(filter);
        }
        void WhenCaughtItem(GameObject obj)
        {
            if (!_filters.CanCatch(obj))
                return;
            _targets.Add(_getTargetFunc(obj));
        }
        void WhenReleaseItem(GameObject obj)
        {
            RemoveTargetBy(obj);
        }
        T RemoveTargetBy(GameObject obj)
        {
            var idx = _targets.FindIndex(t => t.Obj == obj);
            if (idx > -1)
            {
                if (obj == _mainLockTarget?.Obj)
                    MainLockTarget = null;
                var t = _targets[idx];
                _targets.RemoveAt(idx);
                _releaseTargetAction(t);
                return t;
            }
            return null;
        }
        void WhenCatchCompleted(List<GameObject> objs)
        {

        }
        public override void OnFixedUpdate()
        {
        }

        public override void OnLateUpdate()
        {
        }

        public void Dispose()
        {
            _objsCatcher.ItemCaughtAction -= WhenCaughtItem;
            _objsCatcher.ItemReleaseAction -= WhenReleaseItem;
            _objsCatcher.CatchCompletedAction += WhenCatchCompleted;
            _objsCatcher = null;
        }
        ~SimpleTargetLocker()
        {
            Dispose();
        }
    }
}
