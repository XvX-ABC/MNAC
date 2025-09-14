using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Animations;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Utilities.Timeline;
using ArmAim = Tests.BodyBehaviour.Arms.ArmAim;
using Fields = Tests.Characters.CharacterBlackboardFields;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    [Obsolete]
    public partial class ArmedLauncherArmBehaviour_Obsolete : ArmedWeaponArmBehaviourBase_Obsolete
    {
        protected internal class AimingAnimator : IDynamicPlayablePart
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            IArmedLauncherArmBehaviourDefinitions _definitions;
            public AimingAnimator(AnimationClip clip, IArmedLauncherArmBehaviourDefinitions definitions)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(_clip));
                _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
            }

            public IOutputSetting OutputSetting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public Playable GetPlayablePart(PlayableGraph graph)
            {
                if (_playable.IsNull())
                {
                    _playable = AnimationClipPlayable.Create(graph, _clip);
                }
                return _playable;
            }
        }
        protected internal class ReloadAnimator : IDynamicPlayablePart
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            float _speed;
            public ITimeline ReloadTimeline
            {
                set
                {
                    if (value == null)
                        throw new NullReferenceException(nameof(value));
                    _speed = value.Length == 0 ? 1 : _clip.length / value.Length;
                    if (!_playable.Equals(default))
                        _playable.SetSpeed(_speed);
                }
            }

            public IOutputSetting OutputSetting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public ReloadAnimator(AnimationClip clip)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            }

            public Playable GetPlayablePart(PlayableGraph graph)
            {
                if (_playable.IsNull())
                {
                    _playable = AnimationClipPlayable.Create(graph, _clip);
                    _playable.SetSpeed(_speed);

                }
                return _playable;
            }
            public void Play()
            {
                _playable.SetTime(0);
                _playable.Play();
            }
            public void Stop()
            {
                _playable.Pause();
            }
        }
        ILauncher _launcher;
        PlayableStateMachine _stateMachine;
        ArmIdle _idle;
        ArmAim _aim;
        AmmoLoad _ammoLoad;
        internal ArmedLauncherArmAnimator banimator;
        [SerializeField]
        TargetsCatcher_Obsolete _targetsCatcher;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IInput _input;
        public override IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _ammoLoad.Launcher = launcher;
                    banimator.reload.ReloadTimeline = launcher.ReloadTimeline;
                }
                else
                    throw new Exception("Weapon");
            }
        }
        public IOutputSetting OutputSetting
        {
            get => banimator.outputSetting;
            set => banimator.outputSetting = value;
        }
        public override IArmedWeaponArmAnimationPlayablePart Animator { get => banimator; }
        public override Blackboard Blackboard
        {
            get => base.Blackboard;
            set
            {
                if (value != null)
                {
                    if (value.Contains(Fields.TargetsCatcher))
                        value.TryWriteValue(Fields.TargetsCatcher, _targetsCatcher);
                    else
                        value.TryRegisterField(Fields.TargetsCatcher, _targetsCatcher);

                    if (value.TryReadValue<IInput>(Fields.Input, out var input))
                    {
                        _input = input;
                    }
                }
                base.Blackboard = value;
            }
        }

        public override IPlayableState<object> StateNode => this;

        public override Func<bool> EntryFunc { get => Enter; set => throw new NotImplementedException(); }
        public override Func<bool> ExitFunc { get => Exit; set => throw new NotImplementedException(); }

        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<IArmedLauncherArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmedLauncherArmBehaviourDefinitions));


        }

        protected virtual bool Enter()
        {
            var targets = _targetsCatcher.Targets;
            return targets.Count > 0;
        }
        protected virtual bool Exit()
        {
            return _stateMachine.CurrentState == _aim && _targetsCatcher.Targets.Count == 0;
        }
        public override void OnEnter()
        {
            _stateMachine?.OnEnter();
        }

        public override void OnExit()
        {
            _stateMachine?.OnExit();
        }

        public override void OnUpdate()
        {
            _stateMachine?.OnUpdate();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            //var targetsCount = _targetsCatcher.Targets.Count;
            _stateMachine.ChangeStateTo(_aim);
            //_stateMachine.OnEnter();
            //Debug.Log("armed launcher state machine entered");
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = v;
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            banimator.IdleWeight = 1 - v;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            //if (_stateMachine.CurrentState == _aim)
            //{
            //    _aim.Weight = 1 - v;
            //    _aim.TransitionRunningWhichToNextState(currentTransition);
            //}
            //banimator.IdleWeight = v;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = 1 - v;
                //Debug.Log("amred launcher aiming weight: " + (1 - v));
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            banimator.IdleWeight = v;
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _stateMachine.OnExit();
        }
        private void Update()
        {

            _targetsCatcher.OnUpdate();
        }

        void InitializeArmAim()
        {
            var aimIK = GetComponent<AimIK>() ?? throw new ComponentCantFindException(gameObject, typeof(AimIK));
            _aim = new(aimIK);
        }
        void InitializeAmmoReload()
        {
            _ammoLoad = new(banimator.reload);
            _ammoLoad.ExitWhenEnd = true;
        }
        void InitializeStateMachine()
        {
            _stateMachine = new(name + "_statemachine");
            //_stateMachine.AddState(_idle);
            _stateMachine.AddState(_aim);
            _stateMachine.AddState(_ammoLoad);

            //_stateMachine.AddTransitionFor(_idle, _aim, _definitions.IdleAndAimTransitionLength, () => _aim.Target != null, (s, d, t) =>
            //{
            //    if (statusNum != 1)
            //        statusNum = 1;
            //    (d as ArmAim).Weight = t;
            //    banimator.IdleWeight = 1 - t;
            //});
            //_stateMachine.AddTransitionFor(_aim, _idle, _definitions.IdleAndAimTransitionLength, () => _aim.Target == null, (s, d, t) =>
            //{
            //    if (statusNum != 0)
            //        statusNum = 0;
            //    (s as ArmAim).Weight = 1 - t;
            //    banimator.IdleWeight = t;
            //});
            _stateMachine.AddTransitionFor(_aim, _ammoLoad, _definitions.AimAndReloadTransitionLength, () => _input.Reload, (s, d, t) =>
            {
                if (statusNum != 2)
                    statusNum = 2;
                (s as ArmAim).Weight = 1 - t;
            });


            _stateMachine.AddTransitionFor(_ammoLoad, _aim, _definitions.AimAndReloadTransitionLength, (s, d, t) =>
            {
                if (statusNum != 1)
                    statusNum = 1;
                (d as ArmAim).Weight = t;
            });
        }
        public override void Initialize(Blackboard blackboard)
        {
            Debug.Log(GetType().Name + " initializd");
            base.Initialize(blackboard);

            if (blackboard.Contains(Fields.TargetsCatcher))
                blackboard.TryWriteValue(Fields.TargetsCatcher, _targetsCatcher);
            else
                blackboard.TryRegisterField(Fields.TargetsCatcher, _targetsCatcher);

            if (blackboard.TryReadValue<IInput>(Fields.Input, out var input))
            {
                _input = input;
            }


            InitializeArmAim();

            banimator = new(_definitions, _aim);
            _idle = new ArmIdle(banimator);

            InitializeAmmoReload();
            InitializeStateMachine();


            _aim.weightChangedAction += v =>
            {
                banimator.AimingWeight = v;
            };
            _targetsCatcher.OnAwake();
            _targetsCatcher.TargetsChangedAction += targets =>
            {
                //var target = targets.Count > 0 ? targets[0] : null;
                //_aim.Target = target;
            };
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(Fields.TargetsCatcher);
        }

    }
}

