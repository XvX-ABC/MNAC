using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using BehaviorDesigner.Runtime;
using RootMotion.FinalIK;
using System;
using Tests;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.TextCore;
using ArmAim = Tests.BodyBehaviour.Arm.Weapons.Launcher.ArmAim;

namespace Tests.Behaviours.Arm.Weapons
{

    public class ArmedLauncherArmBehaviour : ArmedArmBehaviour
    {
        protected internal class ReloadAnimator : IArmWeaponHoldingBehavioursAnimator
        {
            AnimationClipPlayable _playable;
            AnimationClip _clip;
            float _length;
            public ITimeline ReloadTimeline
            {
                set
                {
                    _length = value.Length;
                    if (!_playable.Equals(default))
                        _playable.SetDuration(_length);
                }
            }
            public ReloadAnimator(AnimationClip clip)
            {
                _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            }

            public Playable GetPlayablePart(PlayableGraph graph)
            {
                if (_playable.Equals(default))
                {
                    _playable = AnimationClipPlayable.Create(graph, _clip);
                    _playable.SetDuration(_length);
                }
                return _playable;
            }
            public void Play()
            {
                _playable.Play();
                _playable.SetTime(0);
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
        ReloadAnimator _reloadAnimator;
        //IInput _input;
        [SerializeField]
        CustomPlayerInput _input;
        [SerializeField]
        Target _target;
        ILauncherBehaviourDefinitions _definitions;
        IPlayableTransition<object> _ts;
        internal IInput input { get => _input; }
        public override IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    //_ammoLoad.ReloadTimeline = launcher.ReloadTimeline;
                    _ammoLoad.Launcher = launcher;
                    _reloadAnimator.ReloadTimeline = launcher.ReloadTimeline;
                }
                else
                    throw new Exception("Weapon");
            }
        }
        public override IArmWeaponHoldingBehavioursAnimator Animator { get => _reloadAnimator; }
        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ILauncherBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILauncherBehaviourDefinitions));
            _idle = new ArmIdle();
            _reloadAnimator = new(_definitions.ReloadClip);
            InitializeArmAim();
            InitializeAmmoReload();
            InitializeStateMachine();

        }
        private void Start()
        {
            _aim.Target = _target;
        }
        void InitializeArmAim()
        {
            var aimIK = this.GetComponent<AimIK>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AimIK));
            _aim = new(aimIK);
        }
        void InitializeAmmoReload()
        {
            _ammoLoad = new(this._reloadAnimator);
            _ammoLoad.ExitWhenEnd = true;
        }
        void InitializeStateMachine()
        {
            _stateMachine = new(this.name + "_statemachine");
            _stateMachine.AddState(_idle);
            _stateMachine.AddState(_aim);
            _stateMachine.AddState(_ammoLoad);

            _stateMachine.AddTransitionFor(_idle, _aim, _definitions.IdleAndAimTransitionLength, () => _target != null, (s, d, t) =>
            {

                (d as ArmAim).Weight = t;
            });
            _stateMachine.AddTransitionFor(_aim, _idle, _definitions.IdleAndAimTransitionLength, () => _target == null, (s, d, t) =>
            {
                (s as ArmAim).Weight = 1 - t;
            });
            _ts = _stateMachine.AddTransitionFor(_aim, _ammoLoad, _definitions.AimAndReloadTransitionLength, () => input.Reload, (s, d, t) =>
                                      {
                                          //Debug.Log("t: " + t);
                                          (s as ArmAim).Weight = 1 - t;
                                      });


            _ts = _stateMachine.AddTransitionFor(_ammoLoad, _aim, _definitions.AimAndReloadTransitionLength, (s, d, t) =>
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
            //if (_stateMachine.CurrentState.Name.Contains("armed_arm_aim ->  armed_arm_ammo_load"))
            //if (_stateMachine.CurrentState == _ammoLoad)
            //if (this.input.Reload)
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Debug point");
            }
            _stateMachine.OnUpdate();
            Debug.Log(_stateMachine);
        }
        private void OnDrawGizmos()
        {
            var pos = _target.Position;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 1);
        }
    }
}
//    [RequireComponent(typeof(AimIK))]
//    public class LauncherBehaviour : MonoBehaviour, IArmWeaponBehaviour, IAimer, IArmWeaponAction
//    {
//        ArmAimer _aimer;
//        [SerializeField]
//        ArmAimingAndReloadTransition _transition;
//        [SerializeField]
//        ArmReloadAnimation _reloadAnimation;
//        ILauncher _launcher;
//        IInput _input;

//        Selector _selector;
//        class Selector : Tests.BT.Selector
//        {
//            public Selector(params IArmAction[] actions)
//            {
//                children.AddRange(actions);
//            }
//        }
//        public WeaponType Type => WeaponType.Launcher;

//        public IWeapon Weapon
//        {
//            get => _launcher;

//            set
//            {
//                if (value is ILauncher launcher)
//                {
//                    _launcher = launcher;
//                    _transition.Launcher = launcher;
//                }
//                else
//                    throw new InvalidCastException($"This weapon '{value.Name}' is not a launcher.");
//            }
//        }
//        public IInput Input
//        {
//            set
//            {
//                _aimer.Input = value;
//                //_transition.Input = value;
//            }
//        }

//        public bool Continuing => _aimer.Continuing;
//        public ITarget Target
//        {
//            get => _aimer.Target;
//            set => _aimer.Target = value;
//        }


//        public TaskState State => _selector.State;

//        void Awake()
//        {
//            _aimer = new(GetComponent<AimIK>());
//            Target = GetComponent<ITarget>();
//            _transition.Initialize(_aimer, _reloadAnimation);
//            //_selector = new Selector(_transition, _aimer);
//        }
//        public void OnUpdate()
//        {
//            if (!this.enabled)
//                return;
//            if (_input.Reload)
//            {
//                _transition.BStart();
//            }

//            _transition.OnUpdate();
//        }
//        void LateUpdate()
//        {

//            _aimer.OnUpdate();
//        }
//        public bool BEnd()
//        {
//            if (_transition.Continuing)
//                _transition.BEnd();
//            _aimer.BEnd();
//            enabled = false;
//            return true;
//        }

//        public bool BStart()
//        {
//            this.enabled = true;
//            return _aimer.BStart();
//        }

//        public TaskState Work()
//        {
//            if (!enabled)
//                return TaskState.Failure;
//            var state = _selector.Work();
//            Debug.Log(_selector.ToString());
//            return state;
//        }
//    }
//}
