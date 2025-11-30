using System;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Interaction;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    internal class TargetLocker : TargetLocker<ILockTarget>
    {
        IHumanInput _input;
        GameObjTarget _currentObjTarget;
        Action<GameObject, GameObject> _mainObjChangedAction;
        Action<IGameObjTarget, IGameObjTarget> _mainObjTargetChangedAction;
        public TargetLocker(
            IHumanInput input,
            GameObjsInScreenCatcher_New screenObjsCatcher,
            Func<GameObject, LockType, ILockTarget> getTargetFunc,
            Action<ILockTarget> releaseTargetAction,
            Camera camera,
            ICursorController cursorReceiver,
            ObstacleDetector obstacleDetector = null,
            ushort handleAmountInCoroutine = 30,
            float catchAngle = 60,
            float targetChangedDuration = 0.2F,
            float receiveInputDuration = 0.05F,
            bool enabled = true) : base(
                screenObjsCatcher,
                getTargetFunc,
                releaseTargetAction,
                camera,
                cursorReceiver,
                obstacleDetector,
                handleAmountInCoroutine,
                catchAngle,
                targetChangedDuration,
                receiveInputDuration,
                enabled)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            MainTargetChangedAction += WhenTargetChangedAction;
        }

        public Action<GameObject, GameObject> MainObjChangedAction
        {
            get => _mainObjChangedAction;
            set => _mainObjChangedAction = value;
        }
        public Action<IGameObjTarget, IGameObjTarget> MainObjTargetChangedAction { get => _mainObjTargetChangedAction; set => _mainObjTargetChangedAction = value; }

        void WhenTargetChangedAction(ILockTarget oldTarget, ILockTarget newTarget)
        {
            _mainObjChangedAction?.Invoke(oldTarget?.Obj, newTarget?.Obj);

            var ov = _currentObjTarget;

            if (newTarget != null)
                _currentObjTarget = GameObjTarget.GetInstance(newTarget.Obj);
            else
                _currentObjTarget = null;


            _mainObjTargetChangedAction?.Invoke(ov, _currentObjTarget);

            if (ov != null && oldTarget?.Obj == ov.Obj)
                GameObjTarget.ReleaseInstance(ov);
        }
        public override void OnFixedUpdate()
        {
            CursorPosition = _input.MousePosition;
            CursorPositionDelta = _input.MousePositionDelta;
            base.OnFixedUpdate();
        }
    }
}
