using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Behaviours.Arm;
using Tests.Behaviours.Arm.Weapons;
using Tests.Character;
using Tests.Characters;
using Tests.Weapons.MultiMissileLauncher;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Tests.BodyBehaviour.Arm.Animations
{
    internal class ArmAnimationCore_New : PlayablePartBase
    {
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;
        MixerPlayablePart _mixer;
        SwitchingPlayablePart _switching;
        IOutputSetting _armedSetting;

        byte _statusNum = 1;
        ArmedWeaponArmAnimator_New _armedAnimator;
        internal bool playing;
        public float SwitchingWeight
        {
            get => _switching.OutputSetting.Weight;
            set
            {
                if (_statusNum != 2)
                    throw new Exception();
                var v = Mathf.Clamp01(value);
                _switching.OutputSetting.Weight = v;
                if (_armedSetting != null)
                    _armedSetting.Weight = 1 - v;
            }
        }
        public override IOutputSetting OutputSetting { get => _switching.OutputSetting; set => _switching.OutputSetting = value; }
        public byte StatusNum
        {
            get => _statusNum;
            set
            {
                if (value == _statusNum)
                    return;
                _statusNum = value;
                if (!_switching.inti)
                    throw new Exception();
                var pnode = this.node.Parent;
                switch (value)
                {
                    case 0:
                        _mixer.Node.RemoveChild(_switching.Node);
                        _mixer.Node.RemoveChild(_armedAnimator.playablePart.Node);
                        pnode.RemoveChild(_mixer.Node);
                        pnode.AddChild(_switching.Node);
                        this.node = (AnimationPlayableNode)_switching.Node;
                        _switching.OutputSetting.Weight = 1;
                        break;
                    case 1:
                        _mixer.Node.RemoveChild(_switching.Node);
                        _mixer.Node.RemoveChild(_armedAnimator.playablePart.Node);
                        pnode.RemoveChild(_mixer.Node);
                        pnode.AddChild(_armedAnimator.playablePart.Node);
                        this.node = (AnimationPlayableNode)_armedAnimator.playablePart.Node;
                        _armedSetting = _armedAnimator.playablePart.OutputSetting;
                        _armedSetting.Weight = 1;
                        break;
                    case 2:
                        pnode.RemoveChild(this.node);
                        pnode.AddChild(_mixer.Node);
                        _mixer.Node.AddChild(_switching.Node);
                        _mixer.Node.AddChild(_armedAnimator.playablePart.Node);
                        this.node = (AnimationPlayableNode)_mixer.Node;
                        _armedSetting = _armedAnimator.playablePart.OutputSetting;
                        break;
                }
            }
        }
        public ArmAnimationCore_New(ArmCore core)
        {
            _definitions = core.definitions.Weapon;
            _animationDefinitions = core.animationDefinitions.Weapon;
            _armedAnimator = new(core.armedWeaponController);


            _switching = new(_definitions, _animationDefinitions);
            _mixer = new();

            this.node = (AnimationPlayableNode)_switching.Node;


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
        class MixerPlayablePart : PlayablePartBase
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
        class SwitchingPlayablePart : PlayablePartBase
        {
            IArmWeaponDefinitions _definitions;
            IArmWeaponAnimationDefinitions _animationDefinitions;
            internal bool inti;

            public SwitchingPlayablePart(IArmWeaponDefinitions definitions, IArmWeaponAnimationDefinitions animationDefinitions) : base()
            {
                _definitions = definitions;
                _animationDefinitions = animationDefinitions;
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
                    this.inti = true;
                }
                return true;
            }
            public override void Dispose()
            {
            }
        }

        public override bool Initialize(PlayableGraph graph)
        {
            return false;
        }
        public void PlaySwitching()
        {
            _switching.PlayablePart.SetTime(0);
            _switching.PlayablePart.Play();
        }
        public void StopSwitching()
        {
            _switching.PlayablePart.Pause();
        }
        public void OnUpdate()
        {
            _armedAnimator.OnUpdate();
        }
    }
    internal class ArmAnimationCore : IDynamicPlayablePart
    {
        IArmWeaponDefinitions _definitions;
        IArmWeaponAnimationDefinitions _animationDefinitions;
        AnimationMixerPlayable _mixer;
        AnimationClipPlayable _switcingClip;
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
                    cp = _switcingClip;
                    _armedAnimator.OutputSetting = this._outputSetting;
                }
                else
                {
                    _mixer.DisconnectInput(0);
                    _mixer.DisconnectInput(1);
                    _mixer.ConnectInput(0, _switcingClip, 0);
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
            _switcingClip = AnimationClipPlayable.Create(graph, _animationDefinitions.Switching.Clip ?? throw new NullReferenceException("definitions.Switching.Clip"));
            var speed = _definitions.SwitchingDurationTime > 0 ? length / _definitions.SwitchingDurationTime : 1;
            _switcingClip.SetSpeed(speed);

            _armedAnimator.GetPlayablePart(graph);
            //graph.Connect(_reloadClip, 0, _mixer, 0);
            //graph.Connect(_armedAnimator.GetPlayablePart(graph), 0, _playablePart, 1);

        }
        public Playable GetPlayablePart(PlayableGraph graph)
        {
            if (_switcingClip.IsNull())
                InitializePlayablePart(graph);
            return _switcingClip;
        }
        public void PlaySwitching()
        {
            Debug.Log("Play switching animation");
            _switcingClip.SetTime(0);
            _switcingClip.Play();
            playing = true;
        }
        public void StopSwitching()
        {
            _switcingClip.Pause();
            playing = false;
        }

        public void OnUpdate()
        {
            _armedAnimator.OnUpdate();
        }
    }
}
