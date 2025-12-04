using System;
using Tests.Characters;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.UI;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Attributes;
using Tests.Utilities.Blackboards;
using UnityEngine;
using IndicatedTarget = Tests.Characters.UI.IndicatedTarget;

namespace Tests.Characters.Weapons
{
    [PlayerComponent(DontDestroyOnLoad = true)]
    internal class PlayerTargetLocker : CharacterComponent, IPlayerTargetLocker<ILockTarget>
    {
        public static implicit operator Behaviours.Arms.Weapons.PlayerTargetLocker(PlayerTargetLocker obj) => obj._locker;
        [SerializeField]
        ushort _processingAmountInCoroutine = 30;
        [SerializeField]
        ObstacleDetector _obstacleDetector;
        [SerializeField]
        float _catchAngle = 60;
        [SerializeField]
        float _targetChangeDuration = 0.2f;
        [SerializeField]
        float _receiveInputDuration = 0.05f;


        Interaction.GameObjsInScreenCatcher _screenCatcher;
        Behaviours.Arms.Weapons.PlayerTargetLocker _locker;
        IndicatorsManager _indicatorsManager;

        public float CatchAngle { get => _locker.CatchAngle; set => _catchAngle = _locker.CatchAngle = value; }
        public Vector3 CursorPosition { get => _locker.CursorPosition; set => _locker.CursorPosition = value; }
        public Vector3 CursorPositionDelta { get => _locker.CursorPositionDelta; set => _locker.CursorPositionDelta = value; }
        public ILockTarget MainLockTarget { get => _locker.MainLockTarget; set => _locker.MainLockTarget = value; }
        public Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => _locker.MainTargetChangedAction; set => _locker.MainTargetChangedAction = value; }
        public ObstacleDetector ObstacleDetector { get => _locker.ObstacleDetector; set => _obstacleDetector = _locker.ObstacleDetector = value; }
        public Vector3 OriginWorldPosition { get => _locker.OriginWorldPosition; set => _locker.OriginWorldPosition = value; }
        void Start()
        {
            StartCoroutine(_screenCatcher.UpdateWithCoroutine());
        }
        void OnEnable()
        {
            if (_locker != null)
                _locker.Enabled = true;
        }
        void OnDisable()
        {
            if (_locker != null)
                _locker.Enabled = false;
        }
        void FixedUpdate()
        {
            _locker.OnFixedUpdate();
        }
        void LateUpdate()
        {
            _locker.OnLateUpdate();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Player_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadUIValueOrThrowException<ICursorIndicator>(CharacterUIBlackboardFields.Character_Actor_Cursor_Indicator, out var cursorIndicator);
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Indicators_Manager, out _indicatorsManager);

            _screenCatcher = new(camera, _processingAmountInCoroutine);
            _locker = new(input, _screenCatcher, CreateLockTarget, ReleaseLockTarget, camera, cursorIndicator, _obstacleDetector, _processingAmountInCoroutine, _catchAngle, _targetChangeDuration, _receiveInputDuration);

            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Component_TargetLocker, this);
            Debug.Log("target locker initialized");
        }

        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Component_TargetLocker);
            base.Dispose();
        }
        LockTarget CreateLockTarget(GameObject obj, LockType lockType)
        {
            var result = LockTarget.GetInstance(obj, lockType);
            var it = _indicatorsManager.AddTargetFor<IndicatedTarget>(obj);
            result.indicatedTarget = it;
            return result;
        }
        void ReleaseLockTarget(ILockTarget target)
        {
            _indicatorsManager.RemoveTargetFor<IndicatedTarget>(target.Obj);
            LockTarget.ReleaseInstance(target as LockTarget);
        }

        public void OnFixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void OnLateUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
