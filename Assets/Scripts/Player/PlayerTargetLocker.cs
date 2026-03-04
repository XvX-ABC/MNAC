using System;
using MNAC.Behaviours;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Input;
using MNAC.Characters.Interaction;
using MNAC.Characters.UI;
using MNAC.Interaction;
using MNAC.UI;
using MNAC.Utilities.Attributes;
using MNAC.Utilities.Blackboards;
using TMPro;
using UnityEngine;
using IndicatedTarget = MNAC.Characters.UI.IndicatedTarget;
using PlayerCursorIndicator = MNAC.Characters.UI.PlayerCursorIndicator;

namespace MNAC.Characters.Weapons
{
    [PlayerComponent(DontDestroyOnLoad = true)]
    internal class PlayerTargetLocker : TargetLockerBase, IPlayerTargetLocker
    {

        internal class EnemyFilter : ICaughtItemFilter<GameObject>
        {
            TeamMask _teamMask;

            public EnemyFilter(TeamMask teamMask)
            {
                _teamMask = teamMask;
            }

            public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

            public bool CanCatch(GameObject item)
            {
                if (item.TryGetComponent<ITeamMember>(out var member))
                {
                    return !member.CheckFriendlyBy(_teamMask);
                }
                return false;
            }

            public bool CanRelease(GameObject item)
            {
                throw new NotImplementedException();
            }
        }
        internal class HealthFilter : ICaughtItemFilter<GameObject>
        {
            public bool CanCatch(GameObject item)
            {
                if (item.TryGetComponent<IDamageable>(out var d))
                {
                    return d.HP.IsAlive;
                }
                return false;
            }

            public bool CanRelease(GameObject item)
            {
                throw new NotImplementedException();
            }
        }
        public static implicit operator Behaviours.Interaction.PlayerTargetLocker(PlayerTargetLocker obj) => obj._locker;
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


        MNAC.Interaction.GameObjsInScreenCatcher _screenCatcher;
        Behaviours.Interaction.PlayerTargetLocker _locker;
        PlayerCursorIndicator _cursorIndicator;
        IndicatorsManager _indicatorsManager;
        HumanoidBodyParts _bodyParts;
        GameObject _chestObj;

        public float CatchAngle { get => _locker.CatchAngle; set => _catchAngle = _locker.CatchAngle = value; }
        public Vector3 CursorPosition { get => _locker.CursorPosition; set => _locker.CursorPosition = value; }
        public Vector3 CursorPositionDelta { get => _locker.CursorPositionDelta; set => _locker.CursorPositionDelta = value; }
        public override ILockTarget MainLockTarget { get => _locker.MainLockTarget; set => _locker.MainLockTarget = value; }
        public override Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => _locker.MainTargetChangedAction; set => _locker.MainTargetChangedAction = value; }
        public override ObstacleDetector ObstacleDetector { get => _locker.ObstacleDetector; set => _obstacleDetector = _locker.ObstacleDetector = value; }
        public Vector3 OriginWorldPosition { get => _locker.OriginWorldPosition; set => _locker.OriginWorldPosition = value; }
        public override float TargetChangeDuration { get => _locker.TargetChangeDuration; set => _locker.TargetChangeDuration = value; }
        public bool ObjsCatchEnable { get => _locker.ObjsCatchEnable; set => _locker.ObjsCatchEnable = value; }
        public bool CursorEnable { get => _locker.CursorEnable; set => _locker.CursorEnable = value; }
        public override Action<GameObject, GameObject> MainObjChangedAction { get => _locker.MainObjChangedAction; set => _locker.MainObjChangedAction = value; }

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
            if (_locker == null)
                return;
            _locker.OnFixedUpdate();
            _locker.OriginWorldPosition = _chestObj.transform.position;
        }
        void LateUpdate()
        {
            if (_locker == null)
                return;
            _locker.OnLateUpdate();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Player_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<IHumanoidInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadUIValueOrThrowException<PlayerCursorIndicator>(CharacterUIBlackboardFields.Player_Cursor_Indicator, out _cursorIndicator);
            blackboard.TryReadUIValueOrThrowException(CharacterUIBlackboardFields.Indicators_Manager, out _indicatorsManager);
            blackboard.TryReadValueOrThrowException<TeamMask>(CharacterBlackboardFields.Character_TeamMask, out var teamMask);
            blackboard.TryReadValueOrThrowException<HumanoidBodyParts>(CharacterBlackboardFields.Character_BodyParts, out _bodyParts);


            _chestObj = _bodyParts.GetItem((uint)HumanBodyPart.Chest);
            _screenCatcher = new(camera, _processingAmountInCoroutine);
            _locker = new(
                input,
                _screenCatcher,
                CreateLockTarget,
                RemoveLockTarget,
                camera,
                _cursorIndicator,
                _obstacleDetector,
                _processingAmountInCoroutine,
                _catchAngle,
                _targetChangeDuration,
                _receiveInputDuration);
            _locker.AddFilter(new EnemyFilter(teamMask));
            _locker.AddFilter(new HealthFilter());
            _locker.MainTargetChangedAction += WhenLockTargetChange;
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Components_TargetLocker, this);
        }

        public override void Dispose()
        {
            _locker.MainTargetChangedAction -= WhenLockTargetChange;
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Components_TargetLocker);
            base.Dispose();
        }
        void WhenLockTargetChange(ILockTarget oldTarget, ILockTarget newTarget)
        {
            _cursorIndicator.LockTarget = newTarget;
        }
        LockTarget CreateLockTarget(GameObject obj, LockType lockType)
        {
            var target = obj.AddComponent<LockTarget>();
            var it = _indicatorsManager.AddTargetFor<IndicatedTarget>(obj);
            target.indicatedTarget = it;
            target.LockType = lockType;
            target.DisableCallback += t => _screenCatcher.RemoveItem(t.Obj);
            return target;
        }
        void RemoveLockTarget(ILockTarget target)
        {
            if (target == MainLockTarget)
                MainLockTarget = null;
            if (target.Obj != null)
                _indicatorsManager.RemoveTargetFor<IndicatedTarget>(target.Obj);
            GameObject.Destroy((LockTarget)target);
        }

        public override void OnFixedUpdate()
        {
            throw new NotImplementedException();
        }

        public override void OnLateUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
