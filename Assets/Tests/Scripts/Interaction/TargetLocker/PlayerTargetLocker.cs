using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tests.States;
using Tests.Utilities;
using Tests.Utilities.Timeline;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Tests.Interaction
{
    public partial class PlayerTargetLocker<T> : PlayerTargetLockerBase<T>, IPlayerTargetLocker<T> where T : class, ILockTarget
    {
        #region internal classes
        internal abstract class TargetLockerState : WithCallbackPlayableState
        {
            StateLifeCycleWatcher<object> _lifeWatcher;
            protected PlayerTargetLocker<T> locker;
            LifeCycleState _lifeCycle { get => _lifeWatcher.CurrentState; }
            public TargetLockerState(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
            {
                this.locker = locker ?? throw new ArgumentNullException(nameof(locker));
                _lifeWatcher = new(this);
            }
            protected abstract void OnExecute(List<GameObject> caughtObjs);
            public void Execute(List<GameObject> caughtObjs)
            {
                if (_lifeCycle == LifeCycleState.Ready || _lifeCycle == LifeCycleState.Exited)
                    return;
                OnExecute(caughtObjs);
            }
        }
        internal class Unlock : TargetLockerState
        {
            public Unlock(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
            {
            }

            protected override void OnExecute(List<GameObject> caughtObjs)
            {
                throw new NotImplementedException();
            }
        }
        internal class Locked : TargetLockerState
        {
            public Locked(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
            {
            }

            protected override void OnExecute(List<GameObject> caughtObjs)
            {
                throw new NotImplementedException();
            }
        }
        internal class FindClosestTargetByMainTarget : TargetLockerState
        {
            public FindClosestTargetByMainTarget(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
            {
            }

            protected override void OnExecute(List<GameObject> caughtObjs)
            {
                var target = locker.FindClosestObjByMainObj(caughtObjs);
                if (target != null)
                    locker.MainLockTarget = target;
            }
        }
        internal class FindClosestTargetByOriginalPosition : TargetLockerState
        {
            public FindClosestTargetByOriginalPosition(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
            {
            }

            protected override void OnExecute(List<GameObject> caughtObjs)
            {
                var target = locker.FindClosestObj(caughtObjs);
                if (target != null)
                    locker.MainLockTarget = target;
            }
        }

        internal class ReceiveCursorInput : TargetLockerState
        {
            List<Vector3> _posList;
            public ReceiveCursorInput(PlayerTargetLocker<T> locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
            {
                _posList = new();
            }
            protected override ITimeline NewTimeline(float duration)
            {
                return new Timeline_V1(duration);
            }
            public override void OnEnter()
            {
                base.OnEnter();
                _posList.Clear();
                timeline.Restart();
            }

            public override void OnUpdate()
            {
                base.OnUpdate();
                _posList.Add(locker.CursorPositionDelta);
                timeline.OnUpdate(Time.deltaTime);
            }
            public override void OnExit()
            {
                base.OnExit();
                timeline.End();
                var pos = Vector3.zero;
                for (int i = 0; i < _posList.Count; i++)
                {
                    pos += _posList[i];
                }
                locker.cursorPositionDeltaCache = pos / _posList.Count;
            }
            protected override void OnExecute(List<GameObject> caughtObjs)
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        List<T> _targets;
        List<GameObject> _targetObjs;
        FilterCollection _filters;
        Func<GameObject, LockType, T> _getTargetFunc;
        Action<T> _releaseTargetAction;

        GameObjsInScreenCatcher _screenObjsCatcher;
        Camera _camera;
        ObstacleDetector _obstacleDetector;


        //IndicatorsManager _indicatorManager;
        //RingCatcher _ringCatcher;
        ICursorController _cursorController;

        [Obsolete]
        GameObject _mainTargetObj;
        T _mainLockTarget;
        Action<T, T> _mainLockTargetChangedAction;
        [Obsolete]
        Action<GameObject, GameObject> _mainTargetChangedAction;


        Vector3 _originWorldPosition;
        Vector3 _originScreenPosition;
        Vector3 _cursorPosition;
        Vector3 _cursorPositionDelta;
        float _catchAngle;
        internal Vector3 cursorPositionDeltaCache;
        float _targetChangeDuration;
        Tween _targetChangeTween;

        [Obsolete]
        internal byte _num;


        internal TargetLockerStatemachine statemachine;
        Unlock _unlock;
        Locked _lockedState;
        FindClosestTargetByOriginalPosition _findClosestTargetState_OP;
        FindClosestTargetByMainTarget _findClosestTargetState_MT;
        ReceiveCursorInput _receiveState;

        public override bool ObjsCatchEnable
        {
            get => _screenObjsCatcher.enabled;
            set => _screenObjsCatcher.enabled = value;
        }
        public override bool CursorEnable
        {
            get => _cursorController.Enabled;
            set => _cursorController.Enabled = value;
        }
        //TODO：激活逻辑不应该与捕获器耦合
        public override bool Enabled
        {
            get => _screenObjsCatcher.Enabled;
            set
            {
                _screenObjsCatcher.Enabled = value;
                _cursorController.Enabled = value;
            }
        }

        public override Vector3 OriginWorldPosition { get => _originWorldPosition; set => _originWorldPosition = value; }
        public override Vector3 CursorPosition { get => _cursorPosition; set => _cursorPosition = value; }
        public override Vector3 CursorPositionDelta { get => _cursorPositionDelta; set => _cursorPositionDelta = value; }
        [Obsolete]
        public GameObject MainTargetObj
        {
            get => _mainTargetObj;
            set
            {
                var ov = _mainTargetObj;
                _mainTargetObj = value;

                _mainTargetChangedAction?.Invoke(ov, value);
            }
        }
        public override T MainLockTarget
        {
            get => _mainLockTarget;
            set
            {
                var ov = _mainLockTarget;
                if (value != null)
                {
                    value.LockType = LockType.Lock_Confirmed;
                    _targetChangeTween = DOTween.To(() => _cursorController.CursorPosition, pos => _cursorController.CursorPosition = pos, _camera.WorldToScreenPoint(value.Position), _targetChangeDuration);
                }
                else
                {
                    _targetChangeTween = DOTween.To(() => _cursorController.CursorPosition, pos => _cursorController.CursorPosition = pos, _cursorPosition, _targetChangeDuration);
                }
                _mainLockTarget = value;
                _mainLockTargetChangedAction?.Invoke(ov, _mainLockTarget);
            }
        }
        public override float CatchAngle { get => _catchAngle * 2; set => _catchAngle = value / 2; }
        public override Action<T, T> MainTargetChangedAction { get => _mainLockTargetChangedAction; set => _mainLockTargetChangedAction = value; }
        public override ObstacleDetector ObstacleDetector { get => _obstacleDetector; set => _obstacleDetector = value; }
        public Camera Camera { get => _camera; set => _camera = value; }
        public override float TargetChangeDuration { get => _targetChangeDuration; set => _targetChangeDuration = value; }

        public PlayerTargetLocker(
            GameObjsInScreenCatcher screenObjsCatcher,
            Func<GameObject, LockType, T> getTargetFunc,
            Action<T> releaseTargetAction,
            Camera camera,
            ICursorController cursorController,
            ObstacleDetector obstacleDetector = null,
            ushort handleAmountInCoroutine = 30,//TODO：冗余参数
            float catchAngle = 60,
            float targetChangedDuration = 0.2f,
            float receiveInputDuration = 0.05f,
            bool enabled = true)
        {
            _targets = new();
            _targetObjs = new();
            _filters = new();
            _getTargetFunc = getTargetFunc ?? throw new ArgumentNullException(nameof(getTargetFunc));
            _releaseTargetAction = releaseTargetAction ?? throw new ArgumentNullException(nameof(releaseTargetAction));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _cursorController = cursorController ?? throw new ArgumentNullException(nameof(cursorController));
            _screenObjsCatcher = screenObjsCatcher ?? throw new ArgumentNullException(nameof(screenObjsCatcher));
            _obstacleDetector = obstacleDetector;

            CatchAngle = catchAngle;

            _screenObjsCatcher.ItemCaughtAction += WhenCaughtItem;
            _screenObjsCatcher.ItemReleaseAction += WhenReleaseItem;
            _screenObjsCatcher.CatchCompletedAction += WhenCatchCompleted;

            _targetChangeDuration = Mathf.Max(0, targetChangedDuration);

            Enabled = enabled;


            InitializeStatemachine(receiveInputDuration);
        }
        void InitializeStatemachine(float receiveInputDuration)
        {
            _unlock = new(this, "unlock");
            _lockedState = new(this, "locked");
            _receiveState = new(this, "receive_input", receiveInputDuration);
            _findClosestTargetState_OP = new(this, "find_target_op");
            _findClosestTargetState_MT = new(this, "find_target_mt");

            statemachine = new();
            statemachine.AddState(_findClosestTargetState_OP);
            statemachine.AddState(_lockedState);
            statemachine.AddState(_receiveState);
            statemachine.AddState(_findClosestTargetState_MT);

            statemachine.AddTransitionFor(_findClosestTargetState_OP, _lockedState, () => _mainLockTarget != null);

            statemachine.AddTransitionFor(_lockedState, _receiveState, () => _cursorPositionDelta != Vector3.zero);
            statemachine.AddTransitionFor(_lockedState, _findClosestTargetState_OP, () => _mainLockTarget == null);

            statemachine.AddTransitionFor(_receiveState, _lockedState, () => _cursorPositionDelta == Vector3.zero && _receiveState.Timeline.NormalizedTime < 1);

            statemachine.AddTransitionFor(_receiveState, _findClosestTargetState_MT, () => _receiveState.Timeline.NormalizedTime >= 1);

            statemachine.AddTransitionFor(_findClosestTargetState_MT, _lockedState, () => _mainLockTarget != null);
            statemachine.AddTransitionFor(_findClosestTargetState_MT, _findClosestTargetState_OP, () => _mainLockTarget == null);


        }
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
            AddTargetBy(obj, LockType.Lock_Unconfirm);
            _targetObjs.Add(obj);
        }
        void WhenReleaseItem(GameObject obj)
        {
            if (obj == _mainTargetObj)
            {
                MainTargetObj = null;
            }
            RemoveTargetBy(obj);
            _targetObjs.Remove(obj);
        }
        void AddTargetBy(GameObject obj, LockType type)
        {
            _targets.Add(_getTargetFunc(obj, type));
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
        void WhenCatchCompleted(List<GameObject> caughtObjs)
        {
            _originScreenPosition = _camera.WorldToScreenPoint(_originWorldPosition);
            UpdateTargetsLockType();
            if (_mainLockTarget != null && _mainLockTarget.LockType == LockType.CantLock)
                MainLockTarget = null;
            _findClosestTargetState_OP.Execute(_targetObjs);
            _findClosestTargetState_MT.Execute(_targetObjs);
        }
        internal T FindClosestObj(List<GameObject> objs)
        {
            if (objs.Count == 0)
                return null;
            var minDistance = float.MaxValue;
            var closestTarget = default(T);
            var screenPos = (Vector2)_originScreenPosition;
            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                var obj = target.Obj;
                if (target == _mainLockTarget || target.LockType <= LockType.CantLock)
                    continue;
                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var distance = Vector3.Distance(pos, screenPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }
            return closestTarget;
        }
        void UpdateCursorReceiver()
        {
            if (_targetChangeTween == null || !_targetChangeTween.IsActive() || !_targetChangeTween.IsPlaying())
                _cursorController.CursorPosition = _mainLockTarget != null ? _camera.WorldToScreenPoint(_mainLockTarget.Position) : _cursorPosition;
        }
        bool IsBehindObstacle(T target)
        {
            return _obstacleDetector.TryDetect(_originWorldPosition, target.Position, out _);
        }
        internal T FindClosestObjByMainObj(List<GameObject> objs)
        {
            var direction = (Vector2)cursorPositionDeltaCache.normalized;
            if (direction == Vector2.zero || _mainLockTarget == null)
                return null;
            var originalPos = (Vector2)_camera.WorldToScreenPoint(_mainLockTarget.Obj.transform.position);
            var minDistance = float.MaxValue;
            var closestTarget = default(T);
            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                var obj = target.Obj;
                if (target == _mainLockTarget || target.LockType <= LockType.CantLock)
                    continue;

                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var tv = pos - originalPos;
                //var tv = originalPos - pos;
                var angle = Vector2.Angle(direction, tv);
                if (angle > _catchAngle)
                    continue;
                var distance = Vector2.Distance(pos, originalPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }
            return closestTarget;
        }
        void UpdateTargetsLockType()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                UpdateTargetLockType(_targets[i]);
            }
        }
        void UpdateTargetLockType(T target)
        {
            var obj = target.Obj;
            if (_obstacleDetector != null && IsBehindObstacle(target))
                target.LockType = LockType.CantLock;
            else if (target != _mainLockTarget)
                target.LockType = LockType.Lock_Unconfirm;
        }
        public override void OnFixedUpdate()
        {
            statemachine.OnUpdate();
        }
        public override void OnLateUpdate()
        {
            UpdateCursorReceiver();
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this.GetType().Name);
            sb.AppendLine("caught items: ");
            foreach (var obj in _targetObjs)
            {
                sb.AppendLine(obj.name);
            }
            return sb.ToString();
        }
    }
}
