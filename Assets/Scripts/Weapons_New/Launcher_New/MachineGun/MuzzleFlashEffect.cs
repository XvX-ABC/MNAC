using System;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    [RequireComponent(typeof(Animator))]
    internal class MuzzleFlashEffect : LauncherEffectComponent
    {
        Animator _animator;
        [SerializeField]
        string _switchName;
        [SerializeField, Range(0f, 10f)]
        float _duration;
        [SerializeField]
        string _speedMultiplierName;
        [SerializeField]
        float _clipLength;
        public float Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                if (_animator != null)
                {
                    var m = _duration <= 0 ? 1 : _clipLength / _duration;
                    _animator.SetFloat(_speedMultiplierName, m);
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }
        private void OnEnable()
        {
            Duration = _duration;
        }
        private void Start()
        {
            Duration = _duration;
        }
        public void Play()
        {
            _animator.SetTrigger(_switchName);
        }
        public void Stop()
        {
            _animator.SetBool(_switchName, false);
        }
    }
}
