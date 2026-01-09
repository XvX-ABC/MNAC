using System;
using Tests.Animations;
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
        #region internal classes
        class ArmAnimationPlayingState : StateBase
        {
            protected ArmAnimationCore core;
            protected IAnimationPlayablePartNode parentNode { get => core.node.PlayableParent; }
            public ArmAnimationPlayingState(string name, ArmAnimationCore core) : base($"arm_animation_core_{name}")
            {
                this.core = core ?? throw new ArgumentNullException(nameof(core));
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
                _playablePart.OutputSetting = core.OutputSetting;
                //core.OutputSetting.Weight = 1;
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
                _playablePart.OutputSetting = core.OutputSetting;
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
                _mixer.OutputSetting = core.OutputSetting;
                _mixer.OutputSetting.Weight = 1;

            }
            public override void OnExit()
            {
                _mixer.Node.RemoveChild(_switching.Node);
                _mixer.Node.RemoveChild(_armedWeapon.Node);
                parentNode.RemoveChild(_mixer.Node);
            }
        }
        class IdleState : ArmAnimationPlayingState
        {
            public IdleState(ArmAnimationCore core) : base("idle", core)
            {
            }
            public override void OnEnter()
            {
                core.OutputSetting.Weight = 0;
            }
        }


        class MixerPlayablePart : AnimationPlayablePartBase
        {
            public MixerPlayablePart(PlayableGraph graph) : base(graph)
            {
                playablePart = AnimationMixerPlayable.Create(graph, 2);
            }

        }
        internal class SwitchingPlayablePart : AnimationPlayablePartBase
        {
            IArmedWeaponArmDefinitions _definitions;
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
                    base.OutputSetting = value;
                }
            }
            public float Weight
            {
                get => _weight;
                set
                {
                    _weight = Mathf.Clamp01(value);
                    if (OutputSetting != null)
                        OutputSetting.Weight = _weight;
                }
            }
            public SwitchingPlayablePart(PlayableGraph graph, IArmedWeaponArmDefinitions definitions, IArmWeaponAnimationDefinitions animationDefinitions) : base(graph)
            {
                _definitions = definitions;
                _animationDefinitions = animationDefinitions;

                var clip = _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip");
                var length = clip.length;
                var c = AnimationClipPlayable.Create(graph, clip);
                var speed = _definitions.Switching.DurationTime > 0 ? length / _definitions.Switching.DurationTime : 1;
                c.SetSpeed(speed);
                playablePart = c;
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

        #endregion


        IArmedWeaponArmDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;

        MixerPlayablePart _mixer;
        internal SwitchingPlayablePart switching;
        internal IArmedWeaponArmAnimator armedAnimator;

        IdleState _idleState;
        SwitchingState _switchingState;
        ArmedWeaponState _armedState;
        BlendingState _blendingState;
        ArmAnimationPlayingState _playingState;

        byte _statusNum = 3;
        internal bool playing;

        public ArmAnimationCore(PlayableGraph graph, IArmedWeaponArmDefinitions weaponDefinitions, IArmWeaponAnimationDefinitions animationDefinitions, IArmedWeaponArmAnimator armedAnimator) : base(graph)
        {
            _definitions = weaponDefinitions ?? throw new ArgumentNullException(nameof(weaponDefinitions));
            _animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            this.armedAnimator = armedAnimator ?? throw new ArgumentNullException(nameof(armedAnimator));


            switching = new(graph, _definitions, _animationDefinitions);
            _mixer = new(graph);

            InitializeStates();
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
                if ((value == 1 || value == 2) && !CheckArmedAnimator())
                    value = 3;
                UpdatePlayingState(value);
                _statusNum = value;
            }
        }

        protected bool CheckArmedAnimator()
        {
            return armedAnimator.PlayablePart.animator != null;
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
