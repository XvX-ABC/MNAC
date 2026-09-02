using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.AI;
using MNAC.Behaviours;
using MNAC.Characters;
using MNAC.Characters.Humanoid.Locomotion;
using MNAC.Interaction;
using MNAC.TPhysics.Locomotion;
using MNAC.Utilities.Blackboards;
using Unity.VisualScripting;
using UnityEngine;
using LocomotionCore = MNAC.Characters.Humanoid.Locomotion.LocomotionCore;

namespace MNAC.AI
{
    [Serializable]
    internal class AITargetLocker : AIComponent, ITargetLocker
    {
        internal class EnemyFilter : ICaughtItemFilter<ILockTarget>
        {
            TeamMask _teamMask;

            public EnemyFilter(TeamMask teamMask)
            {
                _teamMask = teamMask;
            }

            public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

            public bool CanCatch(ILockTarget target)
            {
                var obj = target.Obj;
                if (obj.TryGetComponent<ITeamMember>(out var member))
                {
                    return !member.CheckFriendlyBy(_teamMask);
                }
                return false;
            }

            public bool CanRelease(ILockTarget target)
            {
                throw new NotImplementedException();
            }
        }
        internal class ObstacleFilter : ICaughtItemFilter<ILockTarget>
        {
            Vector3 _origin;
            ObstacleDetector _detector;

            public Vector3 Origin { get => _origin; set => _origin = value; }
            public ObstacleDetector Detector { get => _detector; set => _detector = value; }

            public bool CanCatch(ILockTarget target)
            {
                return !_detector.TryDetect(_origin, target.Position, out _);
            }

            public bool CanRelease(ILockTarget target)
            {
                throw new NotImplementedException();
            }
        }
        SimpleTargetLocker _locker;
        [SerializeField]
        GameObjsInRadiusCatcher _objsCatcher;
        MonoBehaviour _coroutineOwner;
        [SerializeField]
        ObstacleDetector _detector;
        EnemyFilter _enemyFilter;
        ObstacleFilter _obstacleFilter;
        LocomotionCore _locomotionCore;
        GameObject _ownerObj;
        public Action<GameObject, GameObject> MainObjChangedAction { get => _locker.MainObjChangedAction; set => _locker.MainObjChangedAction = value; }
        public ILockTarget MainLockTarget { get => _locker.MainLockTarget; set => _locker.MainLockTarget = value; }
        public Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => _locker.MainTargetChangedAction; set => _locker.MainTargetChangedAction = value; }
        public ObstacleDetector ObstacleDetector
        {
            get => _obstacleFilter?.Detector;
            set
            {
                if (_obstacleFilter != null)
                    _obstacleFilter.Detector = value;
            }
        }
        public float TargetChangeDuration { get => _locker.TargetChangeDuration; set => _locker.TargetChangeDuration = value; }
        public float CatchRadius { get => _objsCatcher.Radius; set => _objsCatcher.Radius = value; }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                _objsCatcher.Enabled = value;
            }
        }
        internal LocomotionCore locomotionCore
        {
            get => _locomotionCore;
            set
            {
                _locomotionCore = value;
            }
        }
        internal GameObject ownerObj
        {
            get => _ownerObj;
            set
            {
                _locker.OwnerObj = value;
                _ownerObj = value;
            }
        }

        public override string Name => "ai_targetlocker";
        ILockTarget GetTarget(GameObject obj)
        {
            var t = obj.AddComponent<LockTarget>();
            return t;

        }
        void ReleaseTarget(ILockTarget target)
        {
            GameObject.Destroy(target as LockTarget);
        }
        public void StartCoroutine(MonoBehaviour owner)
        {
            _coroutineOwner = owner ?? throw new ArgumentNullException(nameof(owner));
            //owner.StartCoroutine(_objsCatcher.UpdateWithCoroutine());
        }
        public void StopCoroutine()
        {
            //_coroutineOwner.StopCoroutine(_objsCatcher.UpdateWithCoroutine());
        }
        public IEnumerator UpdateWithCoroutine()
        {
            return _objsCatcher.UpdateWithCoroutine();
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            _locker = new(_objsCatcher, GetTarget, ReleaseTarget);
            _locker.MainTargetChangedAction += WhenTargetChanged;


            _enemyFilter = new(new TeamMask { Value = 0 });
            _locker.AddFilter(_enemyFilter);

            _obstacleFilter = new();
            _locker.AddFilter(_obstacleFilter);

            var characterBlackboard = context.characterBlackboard;
            if (characterBlackboard.TryReadValue<TeamMask>(AIBlackboardFields.Character_TeamMask, out var teamMask))
            {
                _enemyFilter.TeamMask = teamMask;
            }
            else
                characterBlackboard.RegisterFieldChangeAction<TeamMask>(AIBlackboardFields.Character_TeamMask, WhenTeamMaskChange);


            context.targetLocker = this;
            ObstacleDetector = _detector;

            if (characterBlackboard.TryReadValue<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, out var lcore))
            {
                locomotionCore = lcore;
            }
            else
                characterBlackboard.RegisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);

            if (characterBlackboard.TryReadValue<GameObject>(AIBlackboardFields.Character_Obj_Main, out var obj))
            {
                ownerObj = obj;
            }
            else
                characterBlackboard.RegisterFieldChangeAction<GameObject>(AIBlackboardFields.Character_Obj_Main, WhenObjChange);
        }

        private void WhenObjChange(FieldEventType type, GameObject ov, GameObject nv)
        {
            if (type == FieldEventType.Reading)
                return;
            ownerObj = nv;
        }

        public override void Dispose()

        {
            _locker.MainTargetChangedAction -= WhenTargetChanged;
            context.targetLocker = null;
            locomotionCore = null;
            var characterBlackboard = context.characterBlackboard;
            characterBlackboard.UnregisterFieldChangeAction<LocomotionCore>(AIBlackboardFields.Character_LocomotionCore, WhenLocomotionCoreChange);
            characterBlackboard.UnregisterFieldChangeAction<GameObject>(AIBlackboardFields.Character_Obj_Main, WhenObjChange);
            base.Dispose();
        }
        void WhenTargetChanged(ILockTarget oldTarget, ILockTarget newTarget)
        {

        }
        void WhenTeamMaskChange(FieldEventType type, TeamMask ov, TeamMask nv)
        {
            if (type == FieldEventType.Reading)
                return;
            _enemyFilter.TeamMask = nv;
        }

        void WhenLocomotionCoreChange(FieldEventType type, LocomotionCore ov, LocomotionCore nv)
        {
            if (type == FieldEventType.Reading)
                return;
            locomotionCore = nv;
        }
        public void OnFixedUpdate()
        {
            _locker.OnFixedUpdate();
            if (_locomotionCore != null)
            {
                var pos = _locomotionCore.internalCore.Context.CurrentPosition;
                _locker.Origin = pos;
                //_obstacleFilter.Origin = pos;
            }
        }

        public void OnLateUpdate()
        {
            _locker.OnLateUpdate();
        }
    }
}
