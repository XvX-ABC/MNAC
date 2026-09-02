using System;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    internal class MuzzleFlashEffector : LauncherEffector
    {
        [SerializeField]
        MuzzleFlashEffect[] _flashEffects;
        MuzzleFlashEffect _currentEffect;
        [Range(0, 1)]
        [SerializeField]
        float _durationProportionOnLaunch;
        float _launchInterval;
        Unity.Mathematics.Random _random;
        public float DurationProportionOnLaunch
        {
            get => _durationProportionOnLaunch;
            set
            {
                _durationProportionOnLaunch = Mathf.Clamp01(value);
                var duration = DurationProportionOnLaunch * _launchInterval;
                foreach (var effect in _flashEffects)
                {
                    if (effect == null)
                        continue;
                    effect.Duration = duration;
                }
            }
        }


        protected override void Awake()
        {
            base.Awake();
            _random = new((uint)gameObject.GetInstanceID());
            if (_flashEffects.Length == 0)
            {
                Debug.LogWarning($"This game object '{this.gameObject.name}' is disabled, because the flash effects is empty.");
                this.enabled = false;
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            foreach (var effect in _flashEffects)
            {
                effect?.Initialize(blackboard);
            }
            _launchInterval = owner.Definitions.LaunchingIntervalTime;
            DurationProportionOnLaunch = DurationProportionOnLaunch;

        }
        public override void Dispose()
        {
            base.Dispose();
            foreach (var effect in _flashEffects)
                effect?.Dispose();
        }
        void PlayFlame()
        {
            _currentEffect = _flashEffects[_random.NextInt(0, _flashEffects.Length)];
            _currentEffect.Play();
        }
        void StopFlame()
        {
            _currentEffect?.Stop();
        }
        protected override void WhenLaunch(ILauncher launcher)
        {
        }

        protected override void WhenReload(ILauncher launcher)
        {
            if (!enabled)
                return;
            StopFlame();
        }

        protected override void WhenLaunchBefore(ILauncher launcher)
        {
            if (!enabled)
                return;
            PlayFlame();
        }
    }
}
