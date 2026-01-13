using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    public class SimpleTargetLocker<T> : TargetLockerBase<T>, ITargetLocker<T>, IDisposable where T : class, ILockTarget
    {
        struct Closest
        {
            internal T target;
            internal float distance;
        }
        T _mainLockTarget;
        FilterCollection_New<T> _filters;
        Action<T, T> _mainLockTargetChangedAction;

        Func<GameObject, T> _getTargetFunc;
        Action<T> _releaseTargetAction;

        GameObjsInRadiusCatcher _objsCatcher;
        ObstacleDetector _obstacleDetector;
        Vector3 _origin;
        Closest _closest;
        public SimpleTargetLocker(
            GameObjsInRadiusCatcher objsCatcher,
            Func<GameObject, T> getTargetFunc,
            Action<T> releaseAction,
            ObstacleDetector obstacleDetector = null)
        {
            _filters = new();
            _objsCatcher = objsCatcher ?? throw new ArgumentNullException(nameof(objsCatcher));
            _getTargetFunc = getTargetFunc ?? throw new ArgumentNullException(nameof(getTargetFunc));
            _releaseTargetAction = releaseAction ?? throw new ArgumentNullException(nameof(releaseAction));
            _obstacleDetector = obstacleDetector;

            _objsCatcher.ItemCaughtAction += WhenCaughtItem;
            _objsCatcher.ItemReleaseAction += WhenReleaseItem;
            _objsCatcher.CatchCompletedAction += WhenCatchCompleted;

            _closest = new() { target = null, distance = float.MaxValue };
        }

        public override T MainLockTarget
        {
            get => _mainLockTarget;
            set
            {
                _mainLockTargetChangedAction?.Invoke(_mainLockTarget, value);
                _mainLockTarget = value;
            }
        }
        public override Action<T, T> MainTargetChangedAction { get => _mainLockTargetChangedAction; set => _mainLockTargetChangedAction = value; }
        public override ObstacleDetector ObstacleDetector { get => _obstacleDetector; set => _obstacleDetector = value; }
        public override float TargetChangeDuration { get; set; }
        public Vector3 Origin { get => _objsCatcher.Origin; set => _objsCatcher.Origin = value; }

        public void AddFilter(ICaughtItemFilter<T> filter)
        {
            _filters.AddFilter(filter);
        }
        public void RemoveFilter(ICaughtItemFilter<T> filter)
        {
            _filters.RemoveFilter(filter);
        }
        protected virtual void WhenCaughtItem(GameObject obj)
        {
            //if (!_filters.CanCatch(obj))
            //    return;
            var t = _getTargetFunc(obj);
            //if (!_filters.CanCatch(t))
            //{
            //    _releaseTargetAction(t);
            //    return;
            //}
            //if (!IsClosest(t))
            //{
            //    _releaseTargetAction(t);
            //    return;
            //}
            if (_filters.CanCatch(t) && IsClosest(t))
                MainLockTarget = t;
            else
                _releaseTargetAction(t);
        }
        protected virtual void WhenReleaseItem(GameObject obj)
        {
            RemoveTargetBy(obj);
        }
        void RemoveTargetBy(GameObject obj)
        {
            if (obj == _mainLockTarget?.Obj)
            {
                _releaseTargetAction(_mainLockTarget);
                MainLockTarget = null;
                _closest = new() { target = null, distance = float.MaxValue };
            }
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
        bool IsClosest(T target)
        {
            var c = _closest;
            var pos = target.Position;
            var distance = Vector3.Distance(Origin, pos);
            if (c.distance > distance)
            {
                _closest = new() { target = target, distance = distance };
                return true;
            }
            return false;
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
