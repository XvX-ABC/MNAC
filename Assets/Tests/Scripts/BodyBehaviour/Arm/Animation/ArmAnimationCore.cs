using System;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.Character;
using Tests.States;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static Tests.BodyBehaviour.Arm.Animations.ArmedWeaponArmAnimator_New;
namespace Tests.BodyBehaviour.Arm.Animations
{
    internal class ArmAnimationCore_New : AnimationPlayablePartBase
    {
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;
        MixerPlayablePart _mixer;
        internal SwitchingPlayablePart switching;
        internal ArmedWeaponArmAnimator_New armedAnimator;

        IdleState _idleState;
        SwitchingState _switchingState;
        ArmedWeaponState _armedState;
        BlendingState _blendingState;
        DynamicBlendingState _dynamicBlendingState;
        ArmAnimationPlayingState _playingState;

        byte _statusNum;
        internal bool playing;
        bool _initialized;

        class ArmAnimationPlayingState : StateBase
        {
            protected ArmAnimationCore_New core;
            protected IAnimationPlayablePartNode parentNode;
            public ArmAnimationPlayingState(string name, ArmAnimationCore_New core) : base($"arm_animation_core_{name}")
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
            public SwitchingState(ArmAnimationCore_New core) : base("switching", core)
            {
                _playablePart = core.switching;
            }
            public override void OnEnter()
            {
                parentNode.AddChild(_playablePart.Node);
                _playablePart.OutputSetting = core.outputSetting;
                core.outputSetting.Weight = 1;
            }
            public override void OnExit()
            {
                parentNode.RemoveChild(_playablePart.Node);
            }
        }
        class ArmedWeaponState : ArmAnimationPlayingState
        {
            ArmedWeaponPlayablePart _playablePart;
            public ArmedWeaponState(ArmAnimationCore_New core) : base("armed_weapon", core)
            {
                _playablePart = core.armedAnimator.playablePart;
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

            public BlendingState(ArmAnimationCore_New core) : base("blending_state", core)
            {
                _mixer = core._mixer;
                _switching = core.switching;
                _armedWeapon = core.armedAnimator.playablePart;
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
                core.outputSetting.Weight = 1;

            }
            public override void OnExit()
            {
                _mixer.Node.RemoveChild(_switching.Node);
                _mixer.Node.RemoveChild(_armedWeapon.Node);
                parentNode.RemoveChild(_mixer.Node);
            }
        }
        class DynamicBlendingState : ArmAnimationPlayingState
        {
            SwitchingState _switching;
            BlendingState _blending;
            ArmedWeaponArmAnimator_New _armedAnimator;
            ArmAnimationPlayingState _currentState;
            public DynamicBlendingState(ArmAnimationCore_New core, SwitchingState switching, ArmedWeaponState armed, BlendingState blending) : base("dynamic_blending_state", core)
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
                if (_armedAnimator.playablePart.Enabled)
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
            public IdleState(ArmAnimationCore_New core) : base("idle", core)
            {
            }
            public override void OnEnter()
            {
                core.outputSetting.Weight = 0;
            }
        }
        public float SwitchingWeight
        {
            get => switching.OutputSetting.Weight;
            set
            {
                var v = Mathf.Clamp01(value);
                if (switching.OutputSetting != null)
                {
                    switching.OutputSetting.Weight = v;
                }
                //if (armedAnimator.playablePart.OutputSetting != null)
                //{
                //    armedAnimator.playablePart.OutputSetting.Weight = 1 - v;
                //}
            }
        }
        public byte StatusNum
        {
            get => _statusNum;
            set
            {
                if (value == _statusNum)
                    return;
                if (!_initialized)
                    throw new Exception();
                //Debug.Log($"change status from '{_statusNum}' to '{value}' ");
                UpdatePlayingState(value);
                _statusNum = value;
            }
        }
        public ArmAnimationCore_New(ArmCore core)
        {
            _definitions = core.definitions.Weapon;
            _animationDefinitions = core.animationDefinitions.Weapon;
            armedAnimator = new(core.armedWeaponController);


            switching = new(_definitions, _animationDefinitions);
            _mixer = new();


            //_armedAnimator.stateAction += (b, p) =>
            //{
            //    var pnode = this.node.Parent;
            //    if (b)
            //    {
            //        pnode.RemoveChild(_switching.Node);
            //        pnode.AddChild(_mixer.Node);
            //        //Debug.Log("1 switching.count:" + _switching.PlayablePart.GetOutputCount());
            //        _mixer.Node.AddChild(_switching.Node);
            //        _mixer.Node.AddChild(_armedAnimator.playablePart.Node);
            //        _armedSetting = _armedAnimator.playablePart.OutputSetting;
            //        this.node = (AnimationPlayableNode)_mixer.Node;
            //        _mixer.OutputSetting.Weight = 1;

            //    }
            //    else
            //    {
            //        _mixer.Node.RemoveChild(_switching.Node);
            //        _mixer.Node.RemoveChild(_armedAnimator.playablePart.Node);
            //        pnode.RemoveChild(_mixer.Node);
            //        pnode.AddChild(_switching.Node);

            //        _armedSetting = null;
            //        this.node = (AnimationPlayableNode)_switching.Node;
            //    }
            //};
        }
        class MixerPlayablePart : AnimationPlayablePartBase
        {
            public override bool Initialize(PlayableGraph graph)
            {
                if (playablePart.IsNull())
                {
                    var mixer = AnimationMixerPlayable.Create(graph, 2);
                    playablePart = mixer;
                }
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
            internal PlayState State => playablePart.GetPlayState();
            public SwitchingPlayablePart(IArmWeaponDefinitions definitions, IArmWeaponAnimationDefinitions animationDefinitions) : base()
            {
                _definitions = definitions;
                _animationDefinitions = animationDefinitions;
                enabled = true;
            }

            public override bool Initialize(PlayableGraph graph)
            {
                if (playablePart.IsNull())
                {
                    var clip = _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip");
                    var length = clip.length;
                    var c = AnimationClipPlayable.Create(graph, clip);
                    var speed = _definitions.SwitchingDurationTime > 0 ? length / _definitions.SwitchingDurationTime : 1;
                    c.SetSpeed(speed);
                    this.playablePart = c;
                }
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
            //var newState = statusNum == 2 && !armedAnimator.playablePart.Enabled ? GetNewState(0) : GetNewState(statusNum);
            var newState = GetNewState(statusNum);
            newState.OnEnter();
            _playingState = newState;
            ArmAnimationPlayingState GetNewState(byte num) => num switch
            {
                0 => _switchingState,
                1 => _armedState,
                2 => _blendingState,
                //2 => _dynamicBlendingState,
                3 => _idleState,
                _ => throw new Exception()
            };
        }
        public void OnUpdate()
        {
            _playingState.OnUpdate();
            armedAnimator.OnUpdate();
        }
    }
    internal class ArmAnimationCore : IDynamicPlayablePart
    {
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;
        AnimationMixerPlayable _mixer;
        AnimationClipPlayable _switchingClip;
        IOutputSetting _outputSetting;

        internal bool playing;
        internal bool enabled;

        ArmedWeaponArmAnimator _armedAnimator;
        Action<Playable> _playablePartUpdateAction;
        public Playable PlayablePart
        {
            get => _mixer;
        }
        public float SwitchingWeight
        {
            get => _mixer.GetInputWeight(0);
            set
            {
                var v = Mathf.Clamp01(value);

                //_playablePart.SetInputWeight(0, v);
                //if (_playablePart.GetInputCount() > 1)
                //    _playablePart.SetInputWeight(1, 1 - v);


                //if (_armedAnimator.Enabled)
                //{

                //}
                //else
                //{
                _mixer.SetInputWeight(0, v);
                _outputSetting.Weight = v;
                //}
            }
        }
        public IOutputSetting OutputSetting
        {
            get => _outputSetting;
            set
            {
                _outputSetting = value;
            }
        }

        public bool Enabled { get => enabled; set => enabled = value; }
        public Action<Playable> UpdateAction { get => _playablePartUpdateAction; set => _playablePartUpdateAction = value; }

        public ArmAnimationCore(ArmCore core)
        {
            _definitions = core.definitions.Weapon;
            _animationDefinitions = core.animationDefinitions.Weapon;
            _armedAnimator = new(core.armedWeaponController);

            _armedAnimator.UpdateAction += p =>
            {
                var cp = Playable.Null;
                if (p.IsNull())
                {
                    _mixer.DisconnectInput(0);
                    cp = _switchingClip;
                    _armedAnimator.OutputSetting = this._outputSetting;
                }
                else
                {
                    _mixer.DisconnectInput(0);
                    _mixer.DisconnectInput(1);
                    _mixer.ConnectInput(0, _switchingClip, 0);
                    _mixer.ConnectInput(1, p, 0);
                    cp = _mixer;
                    _armedAnimator.OutputSetting = new OutputSetting(_mixer, 1);
                }
                _playablePartUpdateAction?.Invoke(cp);
            };

        }
        void InitializePlayablePart(PlayableGraph graph)
        {
            _mixer = AnimationMixerPlayable.Create(graph, 2);

            var clip = _animationDefinitions.Switching.Clip;
            var length = clip.length;
            _switchingClip = AnimationClipPlayable.Create(graph, _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip"));
            var speed = _definitions.SwitchingDurationTime > 0 ? length / _definitions.SwitchingDurationTime : 1;
            _switchingClip.SetSpeed(speed);

            _armedAnimator.GetPlayablePart(graph);
            //graph.Connect(_reloadClip, 0, _mixer, 0);
            //graph.Connect(_armedAnimator.GetPlayablePart(graph), 0, _playablePart, 1);

        }
        public Playable GetPlayablePart(PlayableGraph graph)
        {
            if (_switchingClip.IsNull())
                InitializePlayablePart(graph);
            return _switchingClip;
        }
        public void PlaySwitching()
        {
            Debug.Log("Play switching animation");
            _switchingClip.SetTime(0);
            _switchingClip.Play();
            playing = true;
        }
        public void StopSwitching()
        {
            _switchingClip.Pause();
            playing = false;
        }

        public void OnUpdate()
        {
            _armedAnimator.OnUpdate();
        }
    }
}
