using System;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Interaction;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Behaviours
{
    internal class PlayerTargetLocker : PlayerTargetLocker<ILockTarget>, ITargetLocker
    {
        IHumanInput _input;
        GameObjTarget _currentObjTarget;
        Action<GameObject, GameObject> _mainObjChangedAction;
        public PlayerTargetLocker(
            IHumanInput input,
            GameObjsInScreenCatcher screenObjsCatcher,
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

        void WhenTargetChangedAction(ILockTarget oldTarget, ILockTarget newTarget)
        {
            _mainObjChangedAction?.Invoke(oldTarget?.Obj, newTarget?.Obj);
        }
        public override void OnFixedUpdate()
        {
            CursorPosition = _input.MousePosition;
            CursorPositionDelta = _input.MousePositionDelta;
            base.OnFixedUpdate();
        }
    }
}
