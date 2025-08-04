using Assets.Scripts.Utilities.Timeline;
using RootMotion.FinalIK;
using System;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static UnityEngine.Rendering.DebugUI;
using ArmAim = Tests.BodyBehaviour.Arm.Weapons.Launcher.ArmAim;
using Fields = Tests.Characters.CharacterBlackboardFields;

namespace Tests.Behaviours.Arm.Weapons
{
    public class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        protected internal class AimingAnimator : IDynamicPlayablePart
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            ILauncherBehaviourDefinitions _definitions;
            public AimingAnimator(AnimationClip clip, ILauncherBehaviourDefinitions definitions)
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
        protected internal class BAnimator : IArmedWeaponArmAnimator
        {
            internal AimingAnimator aiming;
            internal ReloadAnimator reload;
            AnimationMixerPlayable _playable;
            float _aimingWeight;
            float _idleWeight;
            internal IOutputSetting outputSetting;
            internal bool enabled;
            internal byte state;
            public float IdleWeight
            {
                get => _idleWeight;
                set
                {
                    var v = Mathf.Clamp01(value);
                    _idleWeight = v;
                    UpdateWeight();
                    if (outputSetting != null)
                        outputSetting.Weight = 1 - v;
                }
            }
            public float AimingWeight
            {
                get => _aimingWeight;
                set
                {
                    var v = Mathf.Clamp01(value);
                    _aimingWeight = value;
                    UpdateWeight();
                }
            }

            public IOutputSetting OutputSetting
            {
                get => outputSetting;
                set
                {
                    if (value != null)
                        value.Weight = _idleWeight;
                    outputSetting = value;
                }
            }

            public bool Enabled { get => enabled; set => enabled = value; }
            public byte State { get => state; }
            void UpdateWeight()
            {
                if (!_playable.Equals(default))
                {
                    _playable.SetInputWeight(0, _aimingWeight);
                    _playable.SetInputWeight(1, 1 - _aimingWeight);
                }
            }
            public BAnimator(ILauncherBehaviourDefinitions definitions)
            {
                aiming = new(definitions?.AimingClip, definitions);
                reload = new(definitions?.ReloadClip);
            }
            public Playable GetPlayablePart(PlayableGraph graph)
            {
                var ap = aiming.GetPlayablePart(graph);
                var rp = reload.GetPlayablePart(graph);
                _playable = AnimationMixerPlayable.Create(graph, 2);
                graph.Connect(ap, 0, _playable, 0);
                graph.Connect(rp, 0, _playable, 1);
                AimingWeight = _aimingWeight;
                return _playable;
            }
        }
        ILauncher _launcher;
        PlayableStateMachine _stateMachine;
        ArmIdle _idle;
        ArmAim _aim;
        AmmoLoad _ammoLoad;
        internal BAnimator banimator;
        [SerializeField]
        TargetsCatcher_Debug _targetsCatcher;
        ILauncherBehaviourDefinitions _definitions;
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
        public override IArmedWeaponArmAnimator Animator { get => banimator; }
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


        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ILauncherBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherBehaviourDefinitions));


        }
        private void Start()
        {
            if (banimator != null)
                banimator.AimingWeight = 1;
        }
        void InitializeArmAim()
        {
            var aimIK = this.GetComponent<AimIK>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AimIK));
            _aim = new(aimIK);
        }
        void InitializeAmmoReload()
        {
            _ammoLoad = new(this.banimator.reload);
            _ammoLoad.ExitWhenEnd = true;
        }
        void InitializeStateMachine()
        {
            _stateMachine = new(this.name + "_statemachine");
            _stateMachine.AddState(_idle);
            _stateMachine.AddState(_aim);
            _stateMachine.AddState(_ammoLoad);

            _stateMachine.AddTransitionFor(_idle, _aim, _definitions.IdleAndAimTransitionLength, () => _aim.Target != null, (s, d, t) =>
            {

                (d as ArmAim).Weight = t;
                banimator.IdleWeight = 1 - t;
            });
            _stateMachine.AddTransitionFor(_aim, _idle, _definitions.IdleAndAimTransitionLength, () => _aim.Target == null, (s, d, t) =>
            {
                (s as ArmAim).Weight = 1 - t;
                banimator.IdleWeight = t;
            });
            _stateMachine.AddTransitionFor(_aim, _ammoLoad, _definitions.AimAndReloadTransitionLength, () => _input.Reload, (s, d, t) =>
            {
                (s as ArmAim).Weight = 1 - t;
            });


            _stateMachine.AddTransitionFor(_ammoLoad, _aim, _definitions.AimAndReloadTransitionLength, (s, d, t) =>
              {
                  (d as ArmAim).Weight = t;
              });
        }
        public override void OnEnter()
        {
            _stateMachine.OnEnter();
        }

        public override void OnExit()
        {
            _stateMachine.OnExit();
        }

        public override void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }
        private void Update()
        {
            this.OnUpdate();
            _targetsCatcher.OnUpdate();
            Debug.Log("launcher statemachine : " + _stateMachine);
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);

            if (blackboard.Contains(Fields.TargetsCatcher))
                blackboard.TryWriteValue(Fields.TargetsCatcher, _targetsCatcher);
            else
                blackboard.TryRegisterField(Fields.TargetsCatcher, _targetsCatcher);

            if (blackboard.TryReadValue<IInput>(Fields.Input, out var input))
            {
                _input = input;
            }


            banimator = new(_definitions);
            _idle = new ArmIdle(this);

            InitializeArmAim();
            InitializeAmmoReload();
            InitializeStateMachine();

            _aim.weightChangedAction += v =>
            {
                banimator.AimingWeight = v;
            };
            _targetsCatcher.OnAwake();
            _targetsCatcher.TargetsChangedAction += targets =>
            {
                var target = targets.Count > 0 ? targets[0] : null;
                _aim.Target = target;
            };
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(Fields.TargetsCatcher);
        }
    }
}

