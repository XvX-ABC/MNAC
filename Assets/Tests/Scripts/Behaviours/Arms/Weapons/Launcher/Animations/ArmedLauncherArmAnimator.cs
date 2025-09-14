using System;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms.Weapons.Launchers;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using ArmAim = Tests.BodyBehaviour.Arms.ArmAim;

namespace Tests.Behaviours.Arms.Weapons
{

    internal class ArmedLauncherArmAnimator : IArmedWeaponArmAnimationPlayablePart
    {
        internal AimingAnimator aiming;
        internal ReloadAnimator reload;
        internal ArmAim aim;
        AnimationMixerPlayable _playable;
        float _aimingWeight;
        float _idleWeight;
        internal IOutputSetting outputSetting;
        internal bool enabled;
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
                _aimingWeight = v;
                UpdateWeight();
            }
        }

        public IOutputSetting OutputSetting
        {
            get => outputSetting;
            set
            {
                if (value != null)
                    value.Weight = 1 - _idleWeight;
                outputSetting = value;
            }
        }
        [Obsolete]

        public bool Enabled { get => enabled; set => enabled = value; }
        void UpdateWeight()
        {
            if (!_playable.Equals(default))
            {
                _playable.SetInputWeight(0, _aimingWeight);
                _playable.SetInputWeight(1, 1 - _aimingWeight);
            }
        }
        internal ArmedLauncherArmAnimator(IArmedLauncherArmBehaviourDefinitions definitions, ArmAim aim)
        {
            aiming = new(definitions?.AimingClip, definitions);
            reload = new(definitions?.ReloadClip);
            this.aim = aim;
            enabled = true;
        }
        public Playable GetPlayablePart(PlayableGraph graph)
        {
            if (_playable.IsNull())
            {
                var ap = aiming.GetPlayablePart(graph);
                var rp = reload.GetPlayablePart(graph);
                _playable = AnimationMixerPlayable.Create(graph, 2);
                _playable.ConnectInput(0, ap, 0, 0);
                _playable.ConnectInput(1, rp, 0, 0);
                //graph.Connect(ap, 0, _playable, 0);
                //graph.Connect(rp, 0, _playable, 1);
                //AimingWeight = _aimingWeight;
            }
            return _playable;
        }
    }
}
