using System;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Animations;
using Tests.States;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
namespace Tests.Behaviours.Arms.Animations
{
    internal class ArmAnimationCore : AnimationPlayablePartBase
    {
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;

        MixerPlayablePart _mixer;
        internal SwitchingPlayablePart switching;
        internal IArmedWeaponArmAnimator armedAnimator;

        IdleState _idleState;
        SwitchingState _switchingState;
        ArmedWeaponState _armedState;
        BlendingState _blendingState;
        DynamicBlendingState _dynamicBlendingState;
        ArmAnimationPlayingState _playingState;

        byte _statusNum = 3;
        internal bool playing;
        bool _initialized;

        class ArmAnimationPlayingState : StateBase
        {
            protected ArmAnimationCore core;
            protected IAnimationPlayablePartNode parentNode;
            public ArmAnimationPlayingState(string name, ArmAnimationCore core) : base($"arm_animation_core_{name}")
            {
                this.core = core ?? throw new ArgumentNullException(nameof(core));
                parentNode = core.node.PlayableParent;
            }

            public override void OnEnter()
            {

            }

            public override void OnExit()
            {

            }

            public override void OnUpdate()
            {

            }
        }
        class SwitchingState : ArmAnimationPlayingState
        {
            SwitchingPlayablePart _playablePart;
            public SwitchingState(ArmAnimationCore core) : base("switching", core)
            {
                _playablePart = core.switching;
            }
            public override void OnEnter()
            {
                parentNode.AddChild(_playablePart.Node);
                _playablePart.OutputSetting = core.outputSetting;
                //core.outputSetting.Weight = 1;
            }
            public override void OnExit()
            {
                parentNode.RemoveChild(_playablePart.Node);
            }
        }
        class ArmedWeaponState : ArmAnimationPlayingState
        {
            ArmedWeaponPlayablePart _playablePart;
            public ArmedWeaponState(ArmAnimationCore core) : base("armed_weapon", core)
            {
                _playablePart = core.armedAnimator.PlayablePart;
            }
            public override void OnEnter()
            {
                parentNode.AddChild(_playablePart.Node);
                _playablePart.OutputSetting = core.outputSetting;
            }
            public override void OnExit()
            {
                parentNode.RemoveChild(_playablePart.Node);
            }
        }
        class BlendingState : ArmAnimationPlayingState
        {
            internal MixerPlayablePart _mixer;
            SwitchingPlayablePart _switching;
            ArmedWeaponPlayablePart _armedWeapon;

            public BlendingState(ArmAnimationCore core) : base("blending_state", core)
            {
                _mixer = core._mixer;
                _switching = core.switching;
                _armedWeapon = core.armedAnimator.PlayablePart;
            }
            public override void OnEnter()
            {
                parentNode.AddChild(_mixer.Node);
                _mixer.Node.AddChild(_switching.Node);
                _mixer.Node.AddChild(_armedWeapon.Node);
                //Debug.Log("switching output settting weight: " + _switching.Node.Value.OutputSetting.Weight);
                //Debug.Log("armedWeapon output settting weight: " + _armedWeapon.Node.Value.OutputSetting.Weight);
                _mixer.OutputSetting = core.outputSetting;
                //_switching.Reset();
                //core.outputSetting.Weight = 1;

            }
            public override void OnExit()
            {
                _mixer.Node.RemoveChild(_switching.Node);
                _mixer.Node.RemoveChild(_armedWeapon.Node);
                parentNode.RemoveChild(_mixer.Node);
            }
        }
        [Obsolete]
        class DynamicBlendingState : ArmAnimationPlayingState
        {
            SwitchingState _switching;
            BlendingState _blending;
            IArmedWeaponArmAnimator _armedAnimator;
            ArmAnimationPlayingState _currentState;
            public DynamicBlendingState(ArmAnimationCore core, SwitchingState switching, ArmedWeaponState armed, BlendingState blending) : base("dynamic_blending_state", core)
            {
                _switching = switching;
                _blending = blending;
                _armedAnimator = core.armedAnimator;
            }
            void UpdateCurrentState(ArmAnimationPlayingState newState)
            {
                if (_currentState == newState)
                    return;
                _currentState?.OnExit();
                newState.OnEnter();
                _currentState = newState;
            }
            public override void OnEnter()
            {
                _currentState = _switching;
                _switching.OnEnter();
            }
            public override void OnUpdate()
            {
                var p = _blending._mixer.PlayablePart;
                //if (_currentState == _blending)
                //    Debug.Log($"blending output: 0 -> {p.GetInputWeight(0)},  1 -> {p.GetInputWeight(1)}");
                if (_armedAnimator.PlayablePart.Enabled)
                    UpdateCurrentState(_blending);
                else
                    UpdateCurrentState(_switching);
            }
            public override void OnExit()
            {
                _currentState?.OnExit();
            }
        }
        class IdleState : ArmAnimationPlayingState
        {
            public IdleState(ArmAnimationCore core) : base("idle", core)
            {
            }
            public override void OnEnter()
            {
                core.outputSetting.Weight = 0;
            }
        }
        public float SwitchingWeight
        {
            get => switching.Weight;
            set => switching.Weight = value;
        }
        public byte StatusNum
        {
            get => _statusNum;
            set
            {
                if (value == _statusNum)
                    return;
                //Debug.Log($"change  status num from {_statusNum} to {value}");
                if (!_initialized)
                    throw new Exception();
                UpdatePlayingState(value);
                _statusNum = value;
            }
        }
        //public ArmAnimationCore(ArmCore core, IArmedWeaponArmAnimator armedAnimator)
        //{
        //    _definitions = core.definitions.Weapon;
        //    _animationDefinitions = core.animationDefinitions.Weapon;
        //    this.armedAnimator = armedAnimator ?? throw new ArgumentNullException(nameof(armedAnimator));


        //    switching = new(_definitions, _animationDefinitions);
        //    _mixer = new();
        //}

        public ArmAnimationCore(PlayableGraph graph, IArmWeaponDefinitions weaponDefinitions, IArmWeaponAnimationDefinitions animationDefinitions, IArmedWeaponArmAnimator armedAnimator) : base(graph)
        {
            _definitions = weaponDefinitions ?? throw new ArgumentNullException(nameof(weaponDefinitions));
            _animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            this.armedAnimator = armedAnimator ?? throw new ArgumentNullException(nameof(armedAnimator));


            switching = new(graph, _definitions, _animationDefinitions);
            _mixer = new(graph);
        }
        class MixerPlayablePart : AnimationPlayablePartBase
        {
            public MixerPlayablePart(PlayableGraph graph) : base(graph)
            {
                var mixer = AnimationMixerPlayable.Create(graph, 2);
                playablePart = mixer;
            }

            public override bool Initialize(PlayableGraph graph)
            {
                return true;
            }
            public override void Dispose()
            {
            }
        }
        internal class SwitchingPlayablePart : AnimationPlayablePartBase
        {
            IArmWeaponDefinitions _definitions;
            IArmWeaponAnimationDefinitions _animationDefinitions;
            float _weight;
            internal PlayState State => playablePart.GetPlayState();

            public override IOutputSetting OutputSetting
            {
                get => base.OutputSetting;
                set
                {
                    if (value != null)
                        value.Weight = _weight;
                    outputSetting = value;
                }
            }
            public float Weight
            {
                get => _weight;
                set
                {
                    _weight = Mathf.Clamp01(value);
                    if (outputSetting != null)
                        outputSetting.Weight = _weight;
                }
            }
            public SwitchingPlayablePart(PlayableGraph graph, IArmWeaponDefinitions definitions, IArmWeaponAnimationDefinitions animationDefinitions) : base(graph)
            {
                _definitions = definitions;
                _animationDefinitions = animationDefinitions;
                enabled = true;

                var clip = _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip");
                var length = clip.length;
                var c = AnimationClipPlayable.Create(graph, clip);
                var speed = _definitions.SwitchingDurationTime > 0 ? length / _definitions.SwitchingDurationTime : 1;
                c.SetSpeed(speed);
                playablePart = c;
            }

            public override bool Initialize(PlayableGraph graph)
            {
                return true;
            }
            public override void Dispose()
            {
            }
            public void Reset()
            {
                if (playablePart.IsNull())
                    throw new NullReferenceException();
                playablePart.SetTime(0);
                playablePart.Pause();
            }
            public void Replay()
            {
                if (playablePart.IsNull())
                    throw new NullReferenceException();
                playablePart.SetTime(0);
                playablePart.Play();
            }
            public void Pause()
            {
                if (playablePart.IsNull())
                    throw new NullReferenceException();
                playablePart.Pause();
            }
        }
        public override bool Initialize(PlayableGraph graph)
        {
            _initialized = true;
            InitializeStates();
            return true;
        }
        public override void Dispose()
        {
            _initialized = false;
        }
        public void ReplaySwitching()
        {
            if (playing)
                return;
            switching.Replay();
            playing = true;
        }
        public void PauseSwitching()
        {
            if (!playing)
                return;
            switching.Pause();
            playing = false;
        }
        void InitializeStates()
        {
            _idleState = new(this);
            _switchingState = new(this);
            _armedState = new(this);
            _blendingState = new(this);
            _dynamicBlendingState = new(this, _switchingState, _armedState, _blendingState);
        }
        void UpdatePlayingState(byte statusNum)
        {
            _playingState?.OnExit();
            var newState = GetNewState(statusNum);
            newState.OnEnter();
            _playingState = newState;
            ArmAnimationPlayingState GetNewState(byte num) => num switch
            {
                0 => _switchingState,
                1 => _armedState,
                2 => _blendingState,
                3 => _idleState,
                _ => throw new Exception()
            };
        }
        public void OnUpdate()
        {
            _playingState?.OnUpdate();
            armedAnimator.OnUpdate();
        }
    }
}
